using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

public static class EstadoIntegridad
{
    public const string MensajeMantenimiento = "Servidor en mantenimiento. Intente nuevamente más tarde.";

    private const int SegundosEntreVerificaciones = 30;
    private const int MilisegundosConfirmacion = 1500;

    private const string ClaveEpocaSesion = "EpocaSesion";
    private const string ClaveSesionCerrada = "SesionCerradaPorMantenimiento";
    public const string ClavePasskeyVerificado = "PasskeyVerificado";

    private static readonly object _bloqueo = new object();
    private static HashSet<string> _tablasAlteradas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private static int _epoca;
    private static DateTime? _ultimaVerificacion;
    private static bool _verificando;

    public static bool HayAlteraciones
    {
        get { lock (_bloqueo) return _tablasAlteradas.Count > 0; }
    }

    public static bool TablaAlterada(string tabla)
    {
        lock (_bloqueo) return _tablasAlteradas.Contains(tabla);
    }

    public static bool AlgunaAlterada(params string[] tablas)
    {
        lock (_bloqueo) return tablas.Any(t => _tablasAlteradas.Contains(t));
    }

    public static List<string> TablasAlteradas()
    {
        lock (_bloqueo) return _tablasAlteradas.OrderBy(t => t).ToList();
    }

    public static void VerificarSiCorresponde()
    {
        lock (_bloqueo)
        {
            if (_verificando)
                return;
            if (_ultimaVerificacion.HasValue && (DateTime.Now - _ultimaVerificacion.Value).TotalSeconds < SegundosEntreVerificaciones)
                return;
            _ultimaVerificacion = DateTime.Now;
        }
        Verificar();
    }

    public static void Verificar()
    {
        lock (_bloqueo)
        {
            if (_verificando)
                return;
            _verificando = true;
            _ultimaVerificacion = DateTime.Now;
        }
        try
        {
            BLLIntegridad bll = new BLLIntegridad();
            List<string> alteradas = bll.ObtenerTablasAlteradas();
            if (alteradas.Count > 0)
            {
                System.Threading.Thread.Sleep(MilisegundosConfirmacion);
                alteradas = alteradas.Intersect(bll.ObtenerTablasAlteradas()).ToList();
            }
            Actualizar(alteradas);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError("No se pudo verificar la integridad: " + ex);
        }
        finally
        {
            lock (_bloqueo) _verificando = false;
        }
    }

    private static void Actualizar(List<string> alteradas)
    {
        List<string> nuevas;
        lock (_bloqueo)
        {
            nuevas = alteradas.Where(t => !_tablasAlteradas.Contains(t)).ToList();
            _tablasAlteradas = new HashSet<string>(alteradas, StringComparer.OrdinalIgnoreCase);
            if (nuevas.Count > 0)
                _epoca++;
        }
        if (nuevas.Count > 0)
        {
            RegistrarEnBitacora("Integridad de datos alterada en: " + string.Join(", ", nuevas) +
                ". Se cerraron todas las sesiones y esas secciones quedan en mantenimiento.");
        }
    }

    public static void IniciarSesion(HttpSessionState sesion, string nombreUsuario, string perfil, bool conPasskey)
    {
        sesion["nombreUsuario"] = nombreUsuario;
        sesion["perfil"] = perfil;
        if (conPasskey)
            sesion[ClavePasskeyVerificado] = true;
        lock (_bloqueo) sesion[ClaveEpocaSesion] = _epoca;
    }

    public static void CerrarSesionSiQuedoVieja(HttpSessionState sesion)
    {
        if (sesion == null || sesion["perfil"] == null)
            return;
        int epocaSesion = sesion[ClaveEpocaSesion] as int? ?? 0;
        int epocaActual;
        lock (_bloqueo) epocaActual = _epoca;
        if (epocaSesion == epocaActual)
            return;
        sesion.Clear();
        sesion[ClaveSesionCerrada] = true;
    }

    public static bool ConsumirAvisoSesionCerrada(HttpSessionState sesion)
    {
        if (sesion == null || sesion[ClaveSesionCerrada] as bool? != true)
            return false;
        sesion.Remove(ClaveSesionCerrada);
        return true;
    }

    public static bool EsWebMasterConPasskey(HttpSessionState sesion)
    {
        return sesion != null && sesion["perfil"] as string == "WebMaster" && sesion[ClavePasskeyVerificado] as bool? == true;
    }

    private static void RegistrarEnBitacora(string accion)
    {
        try
        {
            new SERVICIOS.ServicioBitacora().RegistrarEventoSistema(accion.Length <= 255 ? accion : accion.Substring(0, 255));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError(ex.ToString());
        }
    }
}
