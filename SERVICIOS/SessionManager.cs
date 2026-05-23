using BE;

namespace SERVICIOS
{
    public sealed class SessionManager
    {
        private static volatile SessionManager _instance;
        private static readonly object _lock = new object();

        public BEUsuario Usuario { get; private set; }

        private SessionManager()
        { }

        public static SessionManager GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new SessionManager();
                }
            }
            return _instance;
        }

        public void Login(BEUsuario usuario)
        {
            Usuario = usuario;
        }

        public void Logout()
        {
            Usuario = null;
            _instance = null;
        }
    }
}