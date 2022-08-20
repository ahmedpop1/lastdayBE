using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceProject.models;
using EcommerceProject.DTO;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace EcommerceProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly EcommerceContext _context;

        public ProductController(IWebHostEnvironment hostEnvironment, EcommerceContext context)
        {
            _hostEnvironment = hostEnvironment;
            _context = context;
        }

        #region Get Products

        // GET: api/Product
        [HttpGet]
        public ActionResult GetProducts()
        {
            var url = HttpContext.Request;
            var products = from x in _context.Products
                           select new ProductDTO
                           {
                               ID = x.ID,
                               Name = x.Name,
                               Description = x.Description,
                               ImageSrc = string.Format("{0}://{1}{2}/Images/{3}", url.Scheme, url.Host, url.PathBase, x.ImageName),
                               Price = x.Price,
                               Availability = x.Availability,
                               discountPercentage = x.discountPercentage,
                               Category = x.Category.CatName,
                               Brand = x.Brand.BName

                           };
            return Ok(products);
        }

        #endregion

        #region Get Product

        // api/product/5
        [HttpGet("{id:int}", Name = "ProductDetialsRoute")]
        public ActionResult GetProduct(int id)
        {
            var url = HttpContext.Request;
            var product = _context.Products.Include(ww => ww.Category).Include(b => b.Brand).Select(x => new ProductDTO()
            {
                ID = x.ID,
                Name = x.Name,
                Description = x.Description,
                ImageSrc = string.Format("{0}://{1}{2}/Images/{3}", url.Scheme, url.Host, url.PathBase, x.ImageName),
                Price = x.Price,
                Availability = x.Availability,
                discountPercentage = x.discountPercentage,
                Category = x.Category.CatName,
                Brand = x.Brand.BName

            }).SingleOrDefault(b => b.ID == id);
            if (product == null)
            {
                return NotFound();
            }


            return Ok(product);

        }
        #endregion

        #region Update Products

        // PUT: api/Product/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] int id, [FromBody] Product product)
        {
            if (id != product.ID)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        #region Create Product
        // POST: api/Product
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromForm] ProductDtoo product)
        {
            ProductDtoo productDtoo = new ProductDtoo();

            string ImageName = await SaveImage(product.ImageFile);
            if (ModelState.IsValid)
            {
                Product p = new Product();
                p.Name = product.Name;
                p.Price = product.Price;
                p.Description = product.Description;
                p.BrandID = product.BrandId;
                p.CategoryId = product.CategoryId;
                p.discountPercentage = product.discountPercentage;
                p.Availability = product.Availability;
                p.ImageFile = product.ImageFile;
                p.ImageName = ImageName;
                p.ImageSrc = product.ImageSrc;

                _context.Products.Add(p);
                await _context.SaveChangesAsync();
                string url = Url.Link("ProductDetialsRoute", new { id = p.ID });

                return Created(url, product);
            }
            return BadRequest(ModelState);
        }
        #endregion

        #region Delete Product

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok("Deleted Successfully !");
        }
        #endregion

        #region Product Exist
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
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
