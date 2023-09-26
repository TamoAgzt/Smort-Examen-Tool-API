using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace VistaExamenPlanner.Handler
{
    public class RegexAndTextHandler
    {
        public static string GetStudentNumber(string studentEmail)
        {
            return studentEmail.Split('@')[0];
        }
        public static bool IsStudent(string student)
        {
            int output = 0;
            if (Int32.TryParse(student, out output))
            {
                return true;
            }
            return false;

        }
    }
}
