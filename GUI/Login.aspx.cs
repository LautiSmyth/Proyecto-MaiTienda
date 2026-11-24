using BE;
using BE.Enums;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;

public partial class Login : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        pnlMantenimiento.Visible = EstadoIntegridad.HayAlteraciones;

        if (!IsPostBack)
        {
            bool conectado = new BLLUsuario().VerificarConexion();
            if (conectado)
            {
                lblConexion.Text = "<span class='g-status-dot'></span> DB conectada";
            }
            else
            {
                lblConexion.Text = "&#9888; Sin conexi&#243;n a BD";
                lblConexion.Style["color"] = "#e53e3e";
                btnIngresar.Enabled = false;
            }
        }
    }

    protected void btnIngresar_Click(object sender, EventArgs e)
    {
        string nombreUsuario = txtUsuario.Text.Trim();
        string password = txtPassword.Text;

        if (nombreUsuario.Equals("WebMaster", StringComparison.OrdinalIgnoreCase))
        {
            IngresarComoWebMaster(password);
            return;
        }

        if (EstadoIntegridad.HayAlteraciones)
        {
            MostrarNoDisponible();
            return;
        }

        try
        {
            var usuario = new BLLUsuario().ValidarCredenciales(nombreUsuario, password, Request.UserHostAddress);

            EstadoIntegridad.IniciarSesion(Session, usuario.NombreUsuario, usuario.Perfil.ToString(), false);

            MigrarCarritoLocal(usuario.IdUsuario);

            Response.Redirect("~/Bienvenida.aspx");
        }
        catch (UnauthorizedAccessException ex)
        {
            pnlError.Visible = true;
            litError.Text = ex.Message;
        }
        catch (IntegridadException)
        {
            EstadoIntegridad.Verificar();
            pnlMantenimiento.Visible = EstadoIntegridad.HayAlteraciones;
            MostrarNoDisponible();
        }
        catch (Exception)
        {
            pnlError.Visible = true;
            litError.Text = "Ocurrio un error al intentar iniciar sesion. Intente nuevamente.";
        }
    }

    private void IngresarComoWebMaster(string clave)
    {
        bool hayAlteraciones = EstadoIntegridad.HayAlteraciones;
        BEUsuario cuenta = null;
        ResultadoPasskey resultado;

        try
        {
            resultado = hayAlteraciones
                ? new BLLUsuario().ValidarConPasskey(clave, Request.UserHostAddress)
                : new BLLUsuario().ValidarWebMaster(clave, Request.UserHostAddress, out cuenta);
        }
        catch (IntegridadException)
        {
            EstadoIntegridad.Verificar();
            pnlMantenimiento.Visible = EstadoIntegridad.HayAlteraciones;
            pnlError.Visible = true;
            litError.Text = "Servidor en mantenimiento: ingrese con el passkey de emergencia.";
            return;
        }

        if (resultado == ResultadoPasskey.Valido)
        {
            if (cuenta != null)
                EstadoIntegridad.IniciarSesion(Session, cuenta.NombreUsuario, cuenta.Perfil.ToString(), false);
            else
                EstadoIntegridad.IniciarSesion(Session, "WebMaster", "WebMaster", true);

            Response.Redirect(hayAlteraciones ? "~/GestionIntegridad.aspx" : "~/Backup.aspx");
            return;
        }

        pnlError.Visible = true;
        if (resultado == ResultadoPasskey.Bloqueado)
            litError.Text = "Demasiados intentos fallidos. Intente nuevamente más tarde.";
        else
            litError.Text = hayAlteraciones ? "Passkey incorrecto." : "Contraseña o passkey incorrectos.";
    }

    private void MostrarNoDisponible()
    {
        pnlError.Visible = true;
        litError.Text = "No es posible iniciar sesión mientras el servidor está en mantenimiento.";
    }

    private void MigrarCarritoLocal(int idUsuario)
    {
        string carritoJson = hfCarritoLocal.Value;
        if (string.IsNullOrEmpty(carritoJson) || carritoJson == "[]")
            return;

        try
        {
            var ids = new JavaScriptSerializer().Deserialize<List<int>>(carritoJson);
            if (ids == null || ids.Count == 0)
                return;

            var bllCarrito = new BLLCarrito();
            foreach (var entry in ids.GroupBy(id => id).ToDictionary(g => g.Key, g => g.Count()))
                bllCarrito.AgregarProducto(idUsuario, entry.Key, entry.Value);

            Session["LimpiarCarritoLocal"] = true;
        }
        catch { }
    }
}
