using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Stock.Data;
using Stock.Model;
using Stock.WebAPI.Services.Abstraction;
using Stock.WebAPI.ViewModels;

namespace Stock.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IAuthService authService;
        private readonly StockContext _db;

        public AuthController(IAuthService authService, StockContext db)
        {
            this.authService = authService;
            this._db = db;
        }

        [HttpPost("login")]
        public ActionResult<AuthData> Post([FromBody]LoginViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _db.Users.FirstOrDefault(u => u.Email == model.Email);

            if (user == null)
            {
                return BadRequest(new { email = "no user with this email" });
            }

            var passwordValid = authService.VerifyPassword(model.Password, user.Password);
            if (!passwordValid)
            {
                return BadRequest(new { password = "invalid password" });
            }

            if (user.ExpiredDate < DateTime.Now)
            {
                return BadRequest(new { info = "当前用户没有激活，因此不能登录" });
            }

            return authService.GetAuthData(user.Id, user.RoleName);
        }

        [HttpPost("register")]
        public ActionResult<AuthData> Post([FromBody]RegisterViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var emailExist = _db.Users.Any(u => u.Email == model.Email);
            if (emailExist) return BadRequest(new { email = "user with this email already exists" });
            var usernameExist = _db.Users.Any(u => u.Username == model.Username);

            if (usernameExist) return BadRequest(new { username = "user with this name already exists" });

            var id = Guid.NewGuid().ToString();
            var user = new User
            {
                Id = id,
                Username = model.Username,
                Email = model.Email,
                Password = authService.HashPassword(model.Password)
            };
            _db.Users.Add(user);
            _db.SaveChanges();

            if (user.ExpiredDate < DateTime.Now)
            {
                return BadRequest(new { info = "注册成功，但需要激活后才能登录" });
            }


            return authService.GetAuthData(id, user.RoleName);
        }

    }
}