using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceProject.models;
using EcommerceProject.DTO;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace EcommerceProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class usersController : ControllerBase
    {
        private readonly EcommerceContext _context;
        private readonly IConfiguration _configuration;

        public usersController(EcommerceContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<user>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<user>> Getuser(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putuser(string id, user user)
        {
            if (id != user.email)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!userExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<user>> Postuser(user user)
        {
            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (userExists(user.email))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Getuser", new { id = user.email }, user);
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteuser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpPost("Register")]
        public async Task<ActionResult<user>> Registeruser(RegistrUserDto inputuser)

        {
            CreatePasswordHash(inputuser.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user savinguser = new user();


            savinguser.email = inputuser.Email;
            savinguser.fullname = inputuser.Name;
            savinguser.phonenumber = inputuser.PhoneNumber;
            savinguser.address = inputuser.Address;
            //savinguser.image = inputuser.Image;
            savinguser.type = "user";
            savinguser.passwordHash = passwordHash;
            savinguser.passwordSalt = passwordSalt;


            _context.Users.Add(savinguser);
            Cart usercart = new Cart() { username= inputuser.Email };
            _context.Add(usercart);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (_context.Users.FirstOrDefault(d => d.email == inputuser.Email) == null)
                {
                    return Conflict();
                }
                else
                {
                    return BadRequest();
                }


            }
            return Ok(savinguser);
        }
        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginUserDto request)
        {
            user user = _context.Users.FirstOrDefault(d => d.email == request.Email);
            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            if (!VerifyPasswordHash(request.Password, user.passwordHash, user.passwordSalt))
            {
                return Unauthorized("wrong password");
            }
            string token = CreateToken(user);
            user.token = token;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
            LoginRespondDTO respond = new LoginRespondDTO() { token =token,type= user.type, fullname = user.fullname };

            return Ok(respond);
        }
        [HttpPost("usertoken")]
        public async Task<ActionResult<userTypeNameDTO>> user([FromBody] CartDTO CartDTO)
        {

            var user = _context.Users.FirstOrDefault(d => d.token == CartDTO.token);
            userTypeNameDTO usersend = new userTypeNameDTO() { username= user.email, fullname=user.fullname, type=user.type };
            if (user != null) { return usersend; }

            else { return BadRequest(); }
        }

            private string CreateToken(user user)
        {
            List<Claim> claims = new List<Claim>{
            new Claim(ClaimTypes.Name,user.email)

            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(

                _configuration.GetSection("AppSetting:Token").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: cred);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);


            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {

            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);


            }
        }
        // /api/account/GetAllUsers
        //[Authorize]
        [HttpGet("GetUsers")]
        public IActionResult GetAllUsers()
        {
            var url = HttpContext.Request;

            if (ModelState.IsValid)
            {
                var users = from x in _context.Users
                            select new user
                            {
                                fullname = x.fullname,
                                email = x.email,
                                passwordHash = x.passwordHash,
                                passwordSalt = x.passwordSalt,
                                phonenumber = x.phonenumber,
                                address = x.address,
                                isloggedin = x.isloggedin,
                                cart = x.cart,
                                image = x.image,
                                OrderDetials = x.OrderDetials,
                                token = x.token,
                                type = "user"

                            };
                return Ok(users);

            }
            else
            {
                return BadRequest(ModelState);
            }




        }



        private bool userExists(string id)
        {
            return _context.Users.Any(e => e.email == id);
        }

    }
}
