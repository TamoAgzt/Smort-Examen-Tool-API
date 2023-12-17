using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistaExamenPlanner.Objecten
{
    public class LoginReturnData
    {
        public string Token { get; set; } = "";
        public int Rol { get; set; }
        public bool isLoggedIn { get; set; }
    }
}