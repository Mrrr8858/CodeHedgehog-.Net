using Backend_2.Exeptions;
using Backend_2.Models;
using Backend_2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_2.Controllers
{

    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService userService;
        private ITokenService tokenService;
        public AuthController(IUserService service, ITokenService service1)
        {
            userService = service;
            tokenService = service1;
        }

        [Route("login")]
        [HttpPost]
        public Task<IActionResult> Token([FromBody] UserLoginDto model)
        {
            if (User.Identity.IsAuthenticated)
            {
                throw new ForbiddenExeption("You should be unauthorized");
            }

            return tokenService.GetToken(model);
        }
       

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                throw new ValidationExeption("Invalid client request");
            }

            return await tokenService.RefreshToken(tokenModel);
        }


        

        [Authorize]
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Revoke()
        {
            await userService.Logout(User.Identity.Name);
            return Ok();
        }


        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Post(UserRegDto model)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationExeption("Вы ввели некорректные данные");
            }
            if (userService.CheckUsername(model.Username))
            {
                throw new ValidationExeption("Пользователь с таким username уже существует");
            }
            if (!User.Identity.IsAuthenticated)
            {
                await userService.Add(model);
                return Ok(await tokenService.GetToken(new UserLoginDto { Password = model.Password, Username = model.Username}));
            }
            else
            {
                throw new ForbiddenExeption("Вы не должны быть авторизованы");
            }

        }


    }
}
