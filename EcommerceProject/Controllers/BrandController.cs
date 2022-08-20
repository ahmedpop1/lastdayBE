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
    public class BrandsController : ControllerBase
    {
        private readonly EcommerceContext _context;

        public BrandsController(EcommerceContext context)
        {
            _context = context;
        }

        #region Get Brands
        // GET: api/Brands
        [HttpGet]
        public ActionResult GetBrands()
        {
            List<Brand> brands = _context.Brands.Include(p => p.Products).ToList();


            return Ok(brands);
        }
        #endregion

        #region Get Brand

        [HttpGet("{id:int}", Name = "BrandDetialsRoute")]
        public ActionResult GetBrand(int id)
        {
            var brand = _context.Brands.Include(p => p.Products).FirstOrDefault(d => d.id == id);

            if (brand == null)
            {
                return NotFound();
            }

            BrandWithProductsDTO BrandDTO = new BrandWithProductsDTO();
            BrandDTO.ID = brand.id;
            BrandDTO.Name = brand.BName;
            foreach (var product in brand.Products)
            {
                BrandDTO.products.Add(new ProductBrandAndCategoryDto
                {
                    ID = product.ID,
                    Name = product.Name,
                    image = product.ImageSrc,
                    Description = product.Description,
                    Price = product.Price,
                    Quantity = product.Quantity,
                    Availability = product.Availability,
                    discountPercentage = product.discountPercentage,


                });
            }
            return Ok(BrandDTO);
        }
        #endregion

        #region update Brand
        // PUT: api/Brands/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBrand([FromRoute] int id, [FromBody] Brand brand)
        {
            if (id != brand.id)
            {
                return BadRequest();
            }

            _context.Entry(brand).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(id))
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

        #endregion

        #region Create Brand

        // POST: api/Brands
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Brand>> PostBrand([FromForm] BrandDto brand)
        {
            BrandDto brandDto = new BrandDto(); 
            if (ModelState.IsValid)
            {
                Brand b = new Brand();
                b.BName = brand.BrandName;
                _context.Brands.Add(b);
                await _context.SaveChangesAsync();
                string url = Url.Link("BrandDetialsRoute", new { id = b.id });

                return Created(url, brand);
            }
            return BadRequest(ModelState);

        }
        #endregion

        #region Delete Brand

        // DELETE: api/Brands/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return Ok("Dleted Sussessfully !");
        }
        #endregion

        #region Brand Exists
        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.id == id);
        }
        #endregion
    }

}