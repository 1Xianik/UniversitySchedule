using Microsoft.AspNetCore.Mvc;
using UniversitySchedule.API.Models;
using UniversitySchedule.API.Services;

namespace UniversitySchedule.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseService _db;
        private readonly JwtService _jwt;

        public AuthController(DatabaseService db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        /// <summary>
        /// Логин: возвращает JWT-токен
        /// </summary>
        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Логин и пароль обязательны");

            var user = _db.AuthenticateUser(request.Login, request.Password);
            if (user == null)
                return Unauthorized("Неверный логин или пароль");

            var token = _jwt.GenerateToken(user);

            return Ok(new LoginResponse
            {
                Token = token,
                TeacherID = user.TeacherID,
                FullName = user.FullName,
                Role = user.Role
            });
        }
    }
}