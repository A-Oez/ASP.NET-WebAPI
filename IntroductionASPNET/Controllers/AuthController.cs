namespace IntroductionASPNET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJWTService _authService;
        private readonly IConfiguration _configuration;
        public static User user = new User();

        public AuthController(IJWTService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDTO request)
        {
            return Ok(_authService.RegisterUser(request, user));
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDTO request)
        {
            var token = _configuration.GetSection("AppSettings:Token").Value;
            var value = _authService.UserLogin(request, user, token);

            if (value.Equals("User not found"))
                return BadRequest(value);

            return Ok(value.Result);
        }
    }
}
