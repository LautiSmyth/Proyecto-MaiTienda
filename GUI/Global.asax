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
        
        try
        {
            new BLL.BLLIntegridad().ValidarIntegridadGlobal();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("ERROR CRITICO DE INTEGRIDAD DE BASE DE DATOS: " + ex.Message, ex);
        }
    }

    void Application_End(object sender, EventArgs e)
    {
        if (_timerBackup != null)
        {
            _timerBackup.Dispose();
            _timerBackup = null;
        }
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
