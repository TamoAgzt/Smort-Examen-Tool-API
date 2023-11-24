using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistaExamenPlanner.Objecten
{
    public class Examen
    {
        public int Toezichthouders_Id { get; set; }
        public string? Naam_Examen { get; set; }
        public string? Vak_Examen { get; set; }
    }
}