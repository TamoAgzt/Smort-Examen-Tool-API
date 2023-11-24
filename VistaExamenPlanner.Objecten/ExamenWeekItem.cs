using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VistaExamenPlanner.Objecten;

namespace VistaExamenPlanner.Extensions
{
     public class ExamenWeekItem
     {

        public AgendaItem agendaItem { get; set; }

        public Examen examenItem { get; set; }

        public Gebruikers[] Toezichterhouders { get; set; }

    }
}
