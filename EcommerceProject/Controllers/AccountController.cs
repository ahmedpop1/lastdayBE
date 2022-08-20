using EcommerceProject.DTO;
using EcommerceProject.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        #region Register
        // /api/account/register
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromForm] RegistrUserDto userDto)
        {
            string ImageName = await SaveImage(userDto.ImageFile);
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();

                user.Email = userDto.Email;
                user.UserName = userDto.Email;
                user.Name = userDto.Name;
                user.PhoneNumber = userDto.PhoneNumber;
                user.Address = userDto.Address;
                user.ImageFile = userDto.ImageFile;
                user.ImageName = ImageName;
                user.ImageSrc = userDto.ImageSrc;


                IdentityResult result = await _userManager.CreateAsync(user, userDto.Password);

                if (result.Succeeded)
                {
                    return Ok("Account Added Successfully !"); // status code 200
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }

            }
            return BadRequest(ModelState); //status code 400
        }
        #endregion

        #region Login
        // /api/account/login
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromForm] LoginUserDto userDto)

        {
            if (ModelState.IsValid == true)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(userDto.Email);

                if (user != null)
                {
                    var found = await _signInManager.CheckPasswordSignInAsync(user, userDto.Password, false);
                    if (!found.Succeeded)
                    {
                        return BadRequest(found);
                    }
                    else
                    {
                        //creat custom list of claims as token
                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Email, user.Email));
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                        // get role 
                        var roles = await _userManager.GetRolesAsync(user);
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWt:Key"]));

                        SigningCredentials signCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                        // Created token 
                        JwtSecurityToken Mytoken = new JwtSecurityToken(
                            issuer: _configuration["JWt:Issuer"],               //url web api provider
                            audience: _configuration["JWt:Audience"],            // url consumer Angular
                            claims: claims,
                            expires: DateTime.Now.AddDays(30),
                            signingCredentials: signCredentials
                            );

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(Mytoken),
                            expiration = Mytoken.ValidTo
                        });
                    }
                }


                return Unauthorized();
            }
            return Unauthorized();
        }
        #endregion

        #region LogOff
        //// /api/account/logOff
        //[Authorize]
        //[HttpGet("LogOff")]
        //public async Task<IActionResult> LogOffAsync()
        //{
        //    await _signInManager.SignOutAsync();
        //    return Ok();
        //}

        #endregion

        #region All Users


        // /api/account/GetAllUsers
        //[Authorize]
        [HttpGet("GetUsers")]
        public IActionResult GetAllUsers()
        {
            var url = HttpContext.Request;

            if (ModelState.IsValid)
            {
                var users = from x in _userManager.Users
                            select new ApplicationUser
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Email = x.Email,
                                PhoneNumber = x.PhoneNumber,
                                Address = x.Address,
                                ImageSrc = string.Format("{0}://{1}{2}/Images/{3}", url.Scheme, url.Host, url.PathBase, x.ImageName),
                                PasswordHash = x.PasswordHash,

                            };
                return Ok(users);

            }
            else
            {
                return BadRequest(ModelState);
            }




        }
        #endregion

        #region Get User

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser(string id)
        {
            var url = HttpContext.Request;

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            try
            {
                ApplicationUser user1 = new ApplicationUser()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    PasswordHash = user.PasswordHash,
                    ImageSrc = string.Format("{0}://{1}{2}/Images/{3}", url.Scheme, url.Host, url.PathBase, user.ImageName),
                };
                return Ok(user1);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
            return BadRequest("some properties invalid");
        }
        #endregion

        #region update User

        [HttpPut("updateUser")]
        public async Task<IActionResult> updateUser([FromRoute] string id, [FromBody] IdentityUser identity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(identity.Id);
                    user.UserName = identity.UserName;
                    user.Email = identity.Email;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Delete User

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        return Ok("Deleted Successfully !");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        #endregion


        #region -------------------------------- noActions Methods-------------------------------

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile)
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return imageName;
        }





        [NonAction]
        public void DeleteImage(string imageName)
        {
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }

        #endregion

    }
}