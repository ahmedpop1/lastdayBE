using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceProject.models;
using EcommerceProject.DTO;

namespace EcommerceProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly EcommerceContext _context;

        public CartItemsController(EcommerceContext context)
        {
            _context = context;
        }

        // GET: api/CartItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItems>>> GetCartItems()
        {
            return await _context.CartItems.ToListAsync();
        }

        // GET: api/CartItems/5
        [HttpGet("{cartid}")]
        public async Task<ActionResult<CartItems>> GetCartItems(int cartid)
        {
            var cart =  _context.Carts.Include(d=>d.Items).FirstOrDefault(d=>d.id==cartid);

            if (cart == null)
            {
                return NotFound();
            }

            return Ok(cart.Items);
        }

        [HttpGet("getcartitem")]
        public async Task<ActionResult<CartItems>> GetCartItem(int cartid, int productid)
        {
            var cartitems = _context.CartItems.FirstOrDefault(d => d.CartId == cartid && d.productID == productid);
            if (cartitems == null)
            {
                return NotFound();
            }


            return cartitems;

        }


        // PUT: api/CartItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutCartItems( CartItems cartItems)
        {
           

            _context.Entry(cartItems).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.CartItems.FirstOrDefault(d=>d.CartId== cartItems.CartId&&d.productID== cartItems.productID)==null)
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

        // POST: api/CartItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CartItems>> PostCartItems(CartItems cartItems)
        {
            _context.CartItems.Add(cartItems);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CartItemsExists(cartItems.CartId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCartItems", new { id = cartItems.CartId }, cartItems);
        }

        // DELETE: api/CartItems/5
        [HttpDelete]
        public async Task<IActionResult> DeleteCartItemsint (int cartid, int productid)
        {
            var cartitems = _context.CartItems.FirstOrDefault(d => d.CartId == cartid && d.productID == productid);
            if (cartitems == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(cartitems);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost("AddToCart")]
        public async Task<ActionResult<CartItems>> PostCartItems(CartDTO CartDTO)
        {
            if (CartDTO.token=="") {return BadRequest(); }
            string username= getusername(CartDTO.token);
            if (username == "") { return BadRequest(); }
            else {
                int cartid = getcartnumber(username);
                CartItems newitem= new CartItems() { CartId= cartid, productID= (int)CartDTO.prdId, Quantity =1};

                _context.CartItems.Add(newitem);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                   
                        return Conflict();
                    
                  
                }
                return Ok();


            }



        }

        private string getusername(string token  ) {

            var user = _context.Users.FirstOrDefault(d => d.token == token);
            if (user != null) { return user.email; }
            return "";
        }
        private int getcartnumber(string username)
        {
            var cart = _context.Carts.FirstOrDefault(d=>d.username== username);
            return cart.id;


        }

            private bool CartItemsExists(int id)
        {
            return _context.CartItems.Any(e => e.CartId == id);
        }
    }
}
