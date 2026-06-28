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
      