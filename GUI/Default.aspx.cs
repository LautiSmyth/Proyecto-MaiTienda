using BE;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : Page
{
    private readonly BLLProducto _bllProducto = new BLLProducto();
    private HashSet<int> _idsEnCarrito = new HashSet<int>();
    private Dictionary<int, int> _cantidadesEnCarrito = new Dictionary<int, int>();

    protected bool IsEnCarrito(object idProducto)
    {
        return _idsEnCarrito.Contains(Convert.ToInt32(idProducto));
    }

    protected bool IsLogueado()
    {
        return SERVICIOS.SessionManager.GetInstance().Usuario != null;
    }

    protected int GetCantidadEnCarrito(object idProducto)
    {
        int id = Convert.ToInt32(idProducto);
        int qty;
        return _cantidadesEnCarrito.TryGetValue(id, out qty) ? qty : 0;
    }

    public int PaginaActual
    {
        get
        {
            object obj = ViewState["PaginaActual"];
            return obj == null ? 0 : (int)obj;
        }
        set
        {
            ViewState["PaginaActual"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        var usuario = SERVICIOS.SessionManager.GetInstance().Usuario;
        if (usuario != null)
        {
            try
            {
                BLLCarrito bllCarrito = new BLLCarrito();
                _cantidadesEnCarrito = bllCarrito.ObtenerCantidadesProductos(usuario.IdUsuario);
                _idsEnCarrito = new HashSet<int>(_cantidadesEnCarrito.Keys);
            }
            catch
            {
                _idsEnCarrito = new HashSet<int>();
                _cantidadesEnCarrito = new Dictionary<int, int>();
            }
        }
        else
        {
            var carritoAnon = Session["CarritoAnon"] as Dictionary<int, int>;
            if (carritoAnon != null)
            {
                _cantidadesEnCarrito = carritoAnon;
                _idsEnCarrito = new HashSet<int>(carritoAnon.Keys);
            }
        }

        if (!IsPostBack)
        {
            Session["CategoriaSeleccionada"] = "Todos";
            PaginaActual = 0;
            CargarCategorias();
            CargarProductos();
        }
    }

    private void CargarCategorias()
    {
        List<string> categorias = _bllProducto.ObtenerCategorias();
        repCategorias.DataSource = categorias;
        repCategorias.DataBind();
    }

    private void CargarProductos()
    {
        string categoria = Session["CategoriaSeleccionada"] as string ?? "Todos";
        string busqueda = txtBuscar.Text.Trim();
        string orden = ddlOrden.SelectedValue;
        string marca = ddlMarca.SelectedValue;

        decimal? precioMin = null;
        if (decimal.TryParse(txtPrecioMin.Text, out decimal pMin))
        {
            precioMin = pMin;
        }

        decimal? precioMax = null;
        if (decimal.TryParse(txtPrecioMax.Text, out decimal pMax))
        {
            precioMax = pMax;
        }

        try
        {
            List<BEProducto> productos = _bllProducto.ObtenerProductos(categoria, busqueda, orden, precioMin, precioMax, marca);

            PagedDataSource pds = new PagedDataSource();
            pds.DataSource = productos;
            pds.AllowPaging = true;
            pds.PageSize = 24;

            if (PaginaActual >= pds.PageCount)
            {
                PaginaActual = 0;
            }
            pds.CurrentPageIndex = PaginaActual;

            repProductos.DataSource = pds;
            repProductos.DataBind();

            ConfigurarPaginadores(pds.PageCount);

            lblSinResultados.Visible = !productos.Any();
        }
        catch (Exception ex)
        {
            lblSinResultados.Text = $"<div class='no-results-title'>Error de Conexión</div><div class='no-results-text'>{ex.Message}</div>";
            lblSinResultados.Visible = true;
            pnlPaginadorSup.Visible = false;
            pnlPaginadorInf.Visible = false;
        }
    }

    private void ConfigurarPaginadores(int totalPaginas)
    {
        bool visible = totalPaginas > 1;
        pnlPaginadorSup.Visible = visible;
        pnlPaginadorInf.Visible = visible;

        if (!visible) return;

        List<int> paginas = Enumerable.Range(0, totalPaginas).ToList();

        repPaginasSup.DataSource = paginas;
        repPaginasSup.DataBind();

        repPaginasInf.DataSource = paginas;
        repPaginasInf.DataBind();

        btnAntSup.Enabled = PaginaActual > 0;
        btnAntInf.Enabled = PaginaActual > 0;
        btnAntSup.CssClass = PaginaActual > 0 ? "g-btn-page" : "g-btn-page disabled";
        btnAntInf.CssClass = PaginaActual > 0 ? "g-btn-page" : "g-btn-page disabled";

        btnSigSup.Enabled = PaginaActual < totalPaginas - 1;
        btnSigInf.Enabled = PaginaActual < totalPaginas - 1;
        btnSigSup.CssClass = PaginaActual < totalPaginas - 1 ? "g-btn-page" : "g-btn-page disabled";
        btnSigInf.CssClass = PaginaActual < totalPaginas - 1 ? "g-btn-page" : "g-btn-page disabled";
    }

    protected void btnAnterior_Click(object sender, EventArgs e)
    {
        if (PaginaActual > 0)
        {
            PaginaActual--;
            CargarProductos();
        }
    }

    protected void btnSiguiente_Click(object sender, EventArgs e)
    {
        PaginaActual++;
        CargarProductos();
    }

    protected void repPaginas_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        PaginaActual = Convert.ToInt32(e.CommandArgument);
        CargarProductos();
    }

    protected void Categoria_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        string cat = btn.CommandArgument;
        Session["CategoriaSeleccionada"] = cat;
        PaginaActual = 0;

        ResetearEstiloFiltros();
        btn.CssClass = "g-btn-filter active";

        CargarProductos();
    }

    protected void repCategorias_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "FiltrarCategoria")
        {
            string cat = e.CommandArgument.ToString();
            Session["CategoriaSeleccionada"] = cat;
            PaginaActual = 0;

            ResetearEstiloFiltros();
            
            LinkButton btn = e.Item.FindControl("btnCatDinamico") as LinkButton;
            if (btn != null)
            {
                btn.CssClass = "g-btn-filter active";
            }

            CargarProductos();
        }
    }

    protected void txtBuscar_TextChanged(object sender, EventArgs e)
    {
        PaginaActual = 0;
        CargarProductos();
    }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        PaginaActual = 0;
        CargarProductos();
    }

    protected void ddlOrden_SelectedIndexChanged(object sender, EventArgs e)
    {
        PaginaActual = 0;
        CargarProductos();
    }

    protected void btnAplicarPrecio_Click(object sender, EventArgs e)
    {
        PaginaActual = 0;
        CargarProductos();
    }

    protected void ddlMarca_SelectedIndexChanged(object sender, EventArgs e)
    {
        PaginaActual = 0;
        CargarProductos();
    }

    protected void AgregarAlCarrito_Click(object sender, CommandEventArgs e)
    {
        string[] args = e.CommandArgument.ToString().Split('|');
        int productoId = Convert.ToInt32(args[0]);
        int stock = args.Length > 1 ? Convert.ToInt32(args[1]) : int.MaxValue;

        var usuario = SERVICIOS.SessionManager.GetInstance().Usuario;

        if (usuario != null)
        {
            int cantidadActual;
            _cantidadesEnCarrito.TryGetValue(productoId, out cantidadActual);

            if (cantidadActual < stock)
            {
                BLLCarrito bllCarrito = new BLLCarrito();
                bllCarrito.AgregarProducto(usuario.IdUsuario, productoId, 1);
                _cantidadesEnCarrito = bllCarrito.ObtenerCantidadesProductos(usuario.IdUsuario);
                _idsEnCarrito = new HashSet<int>(_cantidadesEnCarrito.Keys);
            }
        }
        else
        {
            var carrito = Session["CarritoAnon"] as Dictionary<int, int> ?? new Dictionary<int, int>();
            int cantidadActual;
            carrito.TryGetValue(productoId, out cantidadActual);
            if (cantidadActual < stock)
            {
                carrito[productoId] = cantidadActual + 1;
                Session["CarritoAnon"] = carrito;
                _cantidadesEnCarrito = carrito;
                _idsEnCarrito = new HashSet<int>(carrito.Keys);
            }
        }

        var master = Master as SiteMaster;
        if (master != null) master.ActualizarCarritoContador();

        CargarProductos();
    }

    protected void CambiarCantidad_Click(object sender, CommandEventArgs e)
    {
        string[] args = e.CommandArgument.ToString().Split('|');
        int productoId = Convert.ToInt32(args[0]);
        int stock = args.Length > 1 ? Convert.ToInt32(args[1]) : int.MaxValue;

        var usuario = SERVICIOS.SessionManager.GetInstance().Usuario;

        int cantidadActual;
        _cantidadesEnCarrito.TryGetValue(productoId, out cantidadActual);
        int nuevaCantidad = e.CommandName == "Incrementar" ? cantidadActual + 1 : cantidadActual - 1;

        if (usuario != null)
        {
            BLLCarrito bllCarrito = new BLLCarrito();
            bllCarrito.ActualizarCantidad(usuario.IdUsuario, productoId, nuevaCantidad, stock);
            _cantidadesEnCarrito = bllCarrito.ObtenerCantidadesProductos(usuario.IdUsuario);
            _idsEnCarrito = new HashSet<int>(_cantidadesEnCarrito.Keys);
        }
        else
        {
            var carrito = Session["CarritoAnon"] as Dictionary<int, int> ?? new Dictionary<int, int>();
            if (nuevaCantidad <= 0)
                carrito.Remove(productoId);
            else
                carrito[productoId] = Math.Min(nuevaCantidad, stock);
            Session["CarritoAnon"] = carrito;
            _cantidadesEnCarrito = carrito;
            _idsEnCarrito = new HashSet<int>(carrito.Keys);
        }

        var master = Master as SiteMaster;
        if (master != null) master.ActualizarCarritoContador();

        CargarProductos();
    }

    private void ResetearEstiloFiltros()
    {
        btnCatTodos.CssClass = "g-btn-filter";
        foreach (RepeaterItem item in repCategorias.Items)
        {
            LinkButton btn = item.FindControl("btnCatDinamico") as LinkButton;
            if (btn != null)
            {
                btn.CssClass = "g-btn-filter";
            }
        }
    }

    protected void btnLimpiarFiltros_Click(object sender, EventArgs e)
    {
        txtBuscar.Text = string.Empty;
        ddlOrden.SelectedValue = "None";
        txtPrecioMin.Text = string.Empty;
        txtPrecioMax.Text = string.Empty;
        ddlMarca.SelectedValue = "Todas";
        Session["CategoriaSeleccionada"] = "Todos";
        PaginaActual = 0;

        ResetearEstiloFiltros();
        btnCatTodos.CssClass = "g-btn-filter active";

        CargarProductos();
    }

    [System.Web.Services.WebMethod(EnableSession = true)]
    public static bool SincronizarCarritoTemporal(List<int> productoIds)
    {
        try
        {
            var usuario = SERVICIOS.SessionManager.GetInstance().Usuario;
            if (usuario != null && productoIds != null && productoIds.Count > 0)
            {
                BLLCarrito bllCarrito = new BLLCarrito();
                bllCarrito.SincronizarCarrito(usuario.IdUsuario, productoIds);
                return true;
            }
        }
        catch
        {
        }
        return false;
    }
}