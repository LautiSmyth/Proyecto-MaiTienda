using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using Microsoft.Ajax.Utilities;
using SERVICIOS;

public partial class Bitacora : Page
{
    private BLLBitacora _bllBitacora = new BLLBitacora();

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
        } else if (perfilActual != "WebMaster")
        {
            Response.Redirect("~/respuesta.aspx");
            return;
        }

    }

    private void CargarBitacora()
    {
        pnlError.Visible = false;

        if (!string.IsNullOrEmpty(txtDesde.Text) && !string.IsNullOrEmpty(txtHasta.Text))
        {
            if (DateTime.TryParse(txtDesde.Text, out DateTime fechaDesde) && 
                DateTime.TryParse(txtHasta.Text, out DateTime fechaHasta))
            {
                if (fechaHasta < fechaDesde)
                {
                    pnlError.Visible = true;
                    litError.Text = "La fecha de fin (Fecha Hasta) no puede ser anterior a la fecha de inicio (Fecha Desde).";
                    gvBitacora.DataSource = null;
                    gvBitacora.DataBind();
                    return;
                }
            }
        }

        try
        {
            List<BEBitacora> listadoBitacora = _bllBitacora.ListarBitacora();

            gvBitacora.DataSource = AplicarFiltros(listadoBitacora);
            gvBitacora.DataBind();
        }
        catch (Exception ex)
        {
            pnlError.Visible = true;
            litError.Text = "Ocurrió un error al cargar la bitácora: " + ex.Message;
        }
    }

    private object AplicarFiltros(List<BEBitacora> listadoBitacora)
    {
        if (!string.IsNullOrEmpty(txtDesde.Text))
        {
            if (DateTime.TryParse(txtDesde.Text, out DateTime fechaDesde))
            {
                listadoBitacora = listadoBitacora.Where(b => b.Fecha >= fechaDesde).ToList();
            }
        }

        if (!string.IsNullOrEmpty(txtHasta.Text))
        {
            if (DateTime.TryParse(txtHasta.Text, out DateTime fechaHasta))
            {
                listadoBitacora = listadoBitacora.Where(b => b.Fecha <= fechaHasta).ToList();
            }
        }

        if (!string.IsNullOrEmpty(txtUsuario.Text))
        {
            listadoBitacora = listadoBitacora.Where(b => b.NombreUsuario.IndexOf(txtUsuario.Text, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        if (ddlPerfil.SelectedValue != "todos")
        {
            listadoBitacora = listadoBitacora.Where(b => b.Perfil.Equals(ddlPerfil.SelectedValue, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (ddlAccion.SelectedValue != "todos")
        {
            listadoBitacora = listadoBitacora.Where(b => b.Accion.IndexOf(ddlAccion.SelectedValue, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        return listadoBitacora;
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
        txtUsuario.Text = "";
        ddlPerfil.SelectedValue = "todos";
        ddlAccion.SelectedValue = "todos";
        gvBitacora.PageIndex = 0;
        CargarBitacora();
    }
}