using Microsoft.AspNetCore.Mvc;

namespace VistaExamenPlanner.Controllers
{
    public class Users : Controller
    {
        private readonly ILogger<Users> _logger;

        public Users(ILogger<Users> logger)
        {
            _logger = logger;
        }

        [HttpGet("ÇreateUser")]
        public void ÇreateUser()
        {
        }

        [HttpGet("Login")]
        public void LoginUser()
        {
        }

    }
}
