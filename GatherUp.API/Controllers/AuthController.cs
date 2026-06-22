using GatherUp.API.DTOs;
using GatherUp.API.Services;
using GatherUp.core.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatherUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        public AuthController(IAuthService authService, ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _authService.Login(request.Email, request.Password);
            if (user == null)
                return Unauthorized(new { error = "Invalid email or password." });
            var token = _tokenService.GenerateToken(user);
            return Ok(new { token, user.Id, user.Name, user.Email, role = user.Role.ToString() });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var user = _authService.Register(request.Name, request.Email, request.Phone, request.Password);
            return Ok(new { user.Id, user.Name, user.Email });
        }
    }
}
