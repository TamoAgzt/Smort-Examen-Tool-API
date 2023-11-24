using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistaExamenPlanner.Objecten
{
    public class ExamenAgendaItem
    {
        public int Toezichthouder_Id { get; set; }
        public string? Naam_Examen { get; set; }
        public string? Vak_Examen { get; set; }
        public int Klas_Id { get; set; }
        public int Examen_Id { get; set; }
        public int Lokaal_Id { get; set; }
        public DateTime BeginTijd { get; set; }
        public DateTime EindTijd { get; set; }
    }
}
