using BE;
using SERVICIOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLLBitacora
    {
        private readonly ServicioBitacora _servicioBitacora = new ServicioBitacora();

        public List<BEBitacora> ListarBitacora()
        {
            return _servicioBitacora.ListarBitacora();
        }
    }
}
