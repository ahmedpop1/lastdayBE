using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceProject.models;

namespace EcommerceProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetialsController : ControllerBase
    {
        private readonly EcommerceContext _context;

        public OrderDetialsController(EcommerceContext context)
        {
            _context = context;
        }

        // GET: api/OrderDetials
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetials>>> GetOrderDetials()
        {
            return await _context.OrderDetials.ToListAsync();
        }

        // GET: api/OrderDetials/5
        [HttpGet("{orderid}")]
        public async Task<ActionResult<OrderDetials>> GetOrderDetials(int orderid)
        {
            var order =  _context.Orders.Include(d=>d.OrderDetials).FirstOrDefault(d=>d.Id== orderid);

            if (order == null)
            {
                return NotFound();
            }
            else { return Ok( order.OrderDetials) ; }
        }


        [HttpGet("getorderdetail")]
        public async Task<ActionResult<OrderDetials>> GetOrderDetial(int orderid,int productid)
        {
            var orderdetails = _context.OrderDetials.FirstOrDefault(d => d.OrderId == orderid && d.ProductId == productid);
            if (orderdetails == null)
            {
                return NotFound();
            }


            return orderdetails;

        }






            // PUT: api/OrderDetials/5
            // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
            [HttpPut]
        public async Task<IActionResult> PutOrderDetials( OrderDetials orderDetials)
        {


            _context.Entry(orderDetials).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.OrderDetials.FirstOrDefault(d => d.OrderId == orderDetials.OrderId && d.ProductId == orderDetials.ProductId) == null)
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

        // POST: api/OrderDetials
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderDetials>> PostOrderDetials(OrderDetials orderDetials)
        {
            _context.OrderDetials.Add(orderDetials);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OrderDetialsExists(orderDetials.OrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOrderDetials", new { id = orderDetials.OrderId }, orderDetials);
        }

        // DELETE: api/OrderDetials/5
        [HttpDelete]
        public async Task<IActionResult> DeleteOrderDetials(int orderid, int productid)
        {
            var orderdetails = _context.OrderDetials.FirstOrDefault(d => d.OrderId == orderid && d.ProductId == productid);
            if (orderdetails == null)
            {
                return NotFound();
            }

            _context.OrderDetials.Remove(orderdetails);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderDetialsExists(int id)
        {
            return _context.OrderDetials.Any(e => e.OrderId == id);
        }
    }
}
