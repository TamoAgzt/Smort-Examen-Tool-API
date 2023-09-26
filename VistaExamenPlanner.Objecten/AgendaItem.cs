using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistaExamenPlanner.Objecten
{
    public class AgendaItem
    {
        public int Klas_Id { get; set; }
        public int Examen_Id { get; set; }
        public int Lokaal_Id { get; set; }
        public DateTime BeginTijd { get; set; }
        public DateTime EindTijd { get; set; }
    }
}