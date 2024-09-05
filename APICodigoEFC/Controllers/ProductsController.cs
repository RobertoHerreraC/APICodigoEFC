using APICodigoEFC.Models;
using APICodigoEFC.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICodigoEFC.Controllers
{
    [Route("api/[controller]/[action]")]
    //[Authorize]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CodigoContext _context;

        public ProductsController(CodigoContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public List<Product> GetByFilters(string? name)
        {
            IQueryable<Product> query = _context.Products.Where(x => x.IsActive);

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.Name.Contains(name));

            return query.OrderBy(x=>x.ProductID) .ToList();
        }

        [HttpPost]       
        public void Insert([FromBody] ProductInsertRequest request)
        {
            //Convertir el request => Model (Serializar)

            Product product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            _context.Products.Add(product);//Un Modelo
            _context.SaveChanges();
        }
        [HttpPut]
        public void Update([FromBody] Product Product)
        {
            _context.Entry(Product).State = EntityState.Modified;
            _context.SaveChanges();
        }

        [HttpPut]
        public void UpdatePrice([FromBody] ProductUpdateRequest request)
        {

            var product = _context.Products.Find(request.Id);
            product.Price = request.Price;
            _context.Entry(product).State = EntityState.Modified;         
           
            _context.SaveChanges();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var Product = _context.Products.Find(id);
            Product.IsActive = false;
            _context.Entry(Product).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
