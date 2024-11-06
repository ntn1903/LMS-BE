using LMS.Application.Interfaces;
using LMS.Application.Models;
using LMS.Application.Models.Users;
using LMS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost("createUser")]
        public async Task<ResponseApi<RegisterModel>> Register([FromBody] RegisterModel user)
        {
            try
            {
                var userExist = await _userManager.FindByEmailAsync(user.Email);
                if (userExist != null)
                    return new ResponseApi<RegisterModel>() { IsSuccess = false, Data = null, Message = "Email already exists!" };

                if (!ModelState.IsValid)
                    return new ResponseApi<RegisterModel>() { IsSuccess = false, Data = null, Message = ModelState.ToDictionary().ToString() };

                var userInfo = new User //IdentityUser
                {
                    UserName = user.UserName,
                    Email = user.Email
                };

                var roleExist = await _roleManager.RoleExistsAsync(user.Role);
                if (!roleExist)
                    return new ResponseApi<RegisterModel>() { IsSuccess = false, Data = null, Message = "This role doesn't exist!" };

                var result = await _userManager.CreateAsync(userInfo, user.Password);
                if (!result.Succeeded)
                    return new ResponseApi<RegisterModel>() { IsSuccess = false, Data = null, Message = result.ToString() };

                var addRoleResult = await _userManager.AddToRoleAsync(userInfo, user.Role);
                if (!addRoleResult.Succeeded)
                    return new ResponseApi<RegisterModel>() { IsSuccess = false, Data = null, Message = addRoleResult.ToString() };

                var aa = new UserModel { UserName = user.UserName, Email = user.Email, Token = _tokenService.Create(userInfo) };

                return new ResponseApi<RegisterModel>() { IsSuccess = true, Data = null, Message = "Register successful!" };
            }
            catch (Exception ex)
            {
                return new ResponseApi<RegisterModel>() { IsSuccess = false, Data = null, Message = ex.Message };
            }
        }

        [HttpPost("login")]
        public async Task<ResponseApi<UserModel>> Login(LoginModel loginUser)
        {

            if (!ModelState.IsValid)
                return new ResponseApi<UserModel>() { IsSuccess = false, Data = null, Message = "" };

            var user = await _userManager.Users.FirstOrDefaultAsync(m => m.UserName == loginUser.Username);

            if (user == null)
                return new ResponseApi<UserModel>() { IsSuccess = false, Data = null, Message = "Invalid Username!" };

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginUser.Password, false);

            if (!result.Succeeded)
                return new ResponseApi<UserModel>() { IsSuccess = false, Data = null, Message = "Password incorrect!" };

            return new ResponseApi<UserModel>() { IsSuccess = true, Data = new UserModel { Id = user.Id, UserName = user.UserName, Email = user.Email, Token = _tokenService.Create(user) }, Message = "Successful!" };
        }

        [HttpPost("createRole")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                    return Ok(result);
                return BadRequest(result.Errors);
            }

            return BadRequest("Role already exist!");
        }
    }
}
