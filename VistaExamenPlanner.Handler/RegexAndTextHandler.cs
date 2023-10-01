using System.Text.RegularExpressions;

namespace VistaExamenPlanner.Handler
{
    public class RegexAndTextHandler
    {
        public static bool IsVistaEmail(string studentEmail)
        {
            string pattern = @"[\w._-]+@vistacollege.nl";

            RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;

            foreach (Match match in Regex.Matches(studentEmail, pattern, options))
            {
                if (match.Success) 
                    return true;
            }
            return false;
        }

        public static string GetStudentNumber(string studentEmail)
        {
            return studentEmail.Split('@')[0];
        }

        public static bool IsStudent(string studentEmail)
        {
            string student = GetStudentNumber(studentEmail);
            int output = 0;
            if (Int32.TryParse(student, out output))
            {
                return true;
            }
            return false;

        }
    }
}
