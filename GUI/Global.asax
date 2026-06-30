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
        {
            DateTime? ultima = Application["IntegridadUltimaVerificacion"] as DateTime?;
            if (!ultima.HasValue || (DateTime.Now - ultima.Value).TotalSeconds >= 30)
            {
                Application["IntegridadUltimaVerificacion"] = DateTime.Now;
                try
                {
                    new BLL.BLLIntegridad().ValidarIntegridadGlobal();
                    Application["IntegridadFallida"] = false;
                    Application["MensajeIntegridad"] = null;
                }
                catch (Exception ex)
                {
                    Application["IntegridadFallida"] = true;
                    Application["MensajeIntegridad"] = ex.Message;
                }
            }
        }

        bool integridadFallida = Application["IntegridadFallida"] as bool? ?? false;
        if (!integridadFallida) return;

        if (path == "~/mantenimiento.aspx" ||
            path == "~/gestionintegridad.aspx" ||
            esRecursoEstatico)
            return;

        Response.Redirect("~/Mantenimiento.aspx");
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
        catch
        {
        }
        finally
        {
            Application["BackupProximaEjecucion"] = DateTime.Now.AddHours(24);
        }
    }

</script>
