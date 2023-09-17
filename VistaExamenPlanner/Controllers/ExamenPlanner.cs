using Microsoft.AspNetCore.Mvc;
using VistaExamenPlanner.Handler;

namespace VistaExamenPlanner.Controllers
{
    [ApiController]
    public class ExamenPlanner : ControllerBase
    {
        private readonly ILogger<ExamenPlanner> _logger;

        public ExamenPlanner(ILogger<ExamenPlanner> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetExamensList")]
        public void GetExamens()
        {
            using (DatabaseHandler database = new())
            {

            }
        }

    }
}