using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using Microsoft.Ajax.Utilities;
using SERVICIOS;

public partial class Bitacora : System.Web.UI.Page
{
    private ServicioBitacora _servicioBitacora = new ServicioBitacora();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarBitacora();
        }

        string perfilActual = Session["perfil"] as string;

        if (string.IsNullOrEmpty(perfilActual))
        {
            Response.Redirect("~/Default.aspx");
            return;
        } else if (perfilActual != "WebMaster" && perfilActual != "Administrador")
        {
            Response.Redirect("~/respuesta.aspx");
            return;
        }

    }

    private void CargarBitacora()
    {
        try
        {
            List<BEBitacora> listadoBitacora = _servicioBitacora.ListarBitacora();

            // Filtrar por fecha desde
            if (!string.IsNullOrEmpty(txtDesde.Text))
            {
                if (DateTime.TryParse(txtDesde.Text, out DateTime fechaDesde))
                {
                    listadoBitacora = listadoBitacora.Where(b => b.Fecha >= fechaDesde).ToList();
                }
            }

            // Filtrar por fecha hasta
            if (!string.IsNullOrEmpty(txtHasta.Text))
            {
                if (DateTime.TryParse(txtHasta.Text, out DateTime fechaHasta))
                {
                    listadoBitacora = listadoBitacora.Where(b => b.Fecha <= fechaHasta).ToList();
                }
            }

            // Filtrar por ID de Usuario (coincidencia parcial/total de texto para mayor flexibilidad)
            if (!string.IsNullOrEmpty(txtIdUsuario.Text))
            {
                listadoBitacora = listadoBitacora.Where(b => b.IdUsuario.ToString().Contains(txtIdUsuario.Text)).ToList();
            }

            // Filtrar por Perfil (coincidencia parcial insensible a mayúsculas/minúsculas)
            if (!string.IsNullOrEmpty(txtPerfil.Text))
            {
                listadoBitacora = listadoBitacora.Where(b => b.Perfil.IndexOf(txtPerfil.Text, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            // Filtrar por Acción (coincidencia parcial insensible a mayúsculas/minúsculas)
            if (!string.IsNullOrEmpty(txtAccion.Text))
            {
                listadoBitacora = listadoBitacora.Where(b => b.Accion.IndexOf(txtAccion.Text, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            gvBitacora.DataSource = listadoBitacora;
            gvBitacora.DataBind();
        }
        catch (Exception ex)
        {
            // Opcional: Manejar el error de forma visual si así lo deseas
        }
    }

    protected void gvBitacora_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvBitacora.PageIndex = e.NewPageIndex;
        CargarBitacora();
    }

    protected void ddlPreset_SelectedIndexChanged(object sender, EventArgs e)
    {
        string preset = ddlPreset.SelectedValue;
        DateTime today = DateTime.Today;

        if (preset == "todos")
        {
            txtDesde.Text = "";
            txtHasta.Text = "";
        }
        else if (preset == "hoy")
        {
            txtDesde.Text = today.ToString("yyyy-MM-ddT00:00");
            txtHasta.Text = today.ToString("yyyy-MM-ddT23:59");
        }
        else if (preset == "ayer")
        {
            DateTime yesterday = today.AddDays(-1);
            txtDesde.Text = yesterday.ToString("yyyy-MM-ddT00:00");
            txtHasta.Text = yesterday.ToString("yyyy-MM-ddT23:59");
        }
        else if (preset == "semana")
        {
            DateTime lastWeek = today.AddDays(-7);
            txtDesde.Text = lastWeek.ToString("yyyy-MM-ddT00:00");
            txtHasta.Text = today.ToString("yyyy-MM-ddT23:59");
        }
        else if (preset == "mes")
        {
            DateTime lastMonth = today.AddMonths(-1);
            txtDesde.Text = lastMonth.ToString("yyyy-MM-ddT00:00");
            txtHasta.Text = today.ToString("yyyy-MM-ddT23:59");
        }

        gvBitacora.PageIndex = 0;
        CargarBitacora();
    }

    protected void btnFiltrar_Click(object sender, EventArgs e)
    {
        if (ddlPreset.SelectedValue != "todos" && ddlPreset.SelectedValue != "personalizado")
        {
            ddlPreset.SelectedValue = "personalizado";
        }
        gvBitacora.PageIndex = 0;
        CargarBitacora();
    }

    protected void btnLimpiar_Click(object sender, EventArgs e)
    {
        ddlPreset.SelectedValue = "todos";
        txtDesde.Text = "";
        txtHasta.Text = "";
        txtIdUsuario.Text = "";
        txtPerfil.Text = "";
        txtAccion.Text = "";
        gvBitacora.PageIndex = 0;
        CargarBitacora();
    }
}