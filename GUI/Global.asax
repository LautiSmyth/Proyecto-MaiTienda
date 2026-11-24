<%@ Application Language="C#" %>
<%@ Import Namespace="Proyecto_MaiTienda" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="BLL" %>

<script runat="server">

    private static System.Threading.Timer _timerBackup;

    void Application_Start(object sender, EventArgs e)
    {
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);
        IniciarBackupAutomatico();
    }

    void Application_BeginRequest(object sender, EventArgs e)
    {
        string path = Request.AppRelativeCurrentExecutionFilePath.ToLower();

        bool esRecursoEstatico =
            path.EndsWith(".axd") || path.EndsWith(".ico") ||
            path.StartsWith("~/scripts/") || path.StartsWith("~/content/") ||
            path.StartsWith("~/assets/") || path.StartsWith("~/bundles/");

        if (!esRecursoEstatico)
            EstadoIntegridad.VerificarSiCorresponde();
    }

    void Application_PostAcquireRequestState(object sender, EventArgs e)
    {
        if (Context.Session != null)
            EstadoIntegridad.CerrarSesionSiQuedoVieja(Context.Session);
    }

    void Application_Error(object sender, EventArgs e)
    {
        Exception ex = Server.GetLastError();
        while (ex != null && !(ex is IntegridadException) && ex.InnerException != null)
            ex = ex.InnerException;
        if (!(ex is IntegridadException))
            return;

        string path = Request.AppRelativeCurrentExecutionFilePath.ToLower();
        if (path == "~/login.aspx")
            return;

        Server.ClearError();
        EstadoIntegridad.Verificar();
        if (Context.Session != null)
            EstadoIntegridad.CerrarSesionSiQuedoVieja(Context.Session);
        Response.Redirect("~/Login.aspx", false);
        Context.ApplicationInstance.CompleteRequest();
    }

    void Application_End(object sender, EventArgs e)
    {
        if (_timerBackup != null) { _timerBackup.Dispose(); _timerBackup = null; }
    }

    private void IniciarBackupAutomatico()
    {
        DateTime ahora = DateTime.Now;
        DateTime proximaEjecucion = ahora.Date.AddDays(1).Add(ahora.TimeOfDay);
        TimeSpan demora = proximaEjecucion - ahora;

        Application["BackupProximaEjecucion"] = proximaEjecucion;

        _timerBackup = new System.Threading.Timer(EjecutarBackupAutomatico, null, demora, TimeSpan.FromHours(24));
    }

    private void EjecutarBackupAutomatico(object state)
    {
        try
        {
            new BLLBackup().GenerarBackupAutomatico();
        }
        catch (Exception ex)
        {
            try
            {
                new SERVICIOS.ServicioBitacora().RegistrarEventoSistema("Error en el backup automatico: " + ex.Message);
            }
            catch (Exception)
            {
            }
        }
        finally
        {
            Application["BackupProximaEjecucion"] = DateTime.Now.AddHours(24);
        }
    }

</script>
