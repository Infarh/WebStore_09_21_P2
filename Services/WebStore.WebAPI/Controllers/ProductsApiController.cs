using Microsoft.AspNetCore.Mvc;
using WebStore.Domain;
using WebStore.Domain.DTO;
using WebStore.Interfaces;
using WebStore.Interfaces.Services;

namespace WebStore.WebAPI.Controllers
{
    [ApiController]
    [Route(WebAPIAddresses.Products)]
    public class ProductsApiController : ControllerBase
    {
        private readonly IProductData _ProductData;

        public ProductsApiController(IProductData ProductData) => _ProductData = ProductData;

        [HttpGet("sections")]
        public IActionResult GetSections()
        {
            var sections = _ProductData.GetSections();
            return Ok(sections.ToDTO());
        }

        [HttpGet("sections/{id:int}")]
        public IActionResult GetSection(int id)
        {
            var section = _ProductData.GetSection(id);
            if (section is null)
                return NotFound(id);

            return Ok(section.ToDTO());
        }

        [HttpGet("brands")]
        public IActionResult GetBrands()
        {
            var brands = _ProductData.GetBrands();
            return Ok(brands.ToDTO());
        }

        [HttpGet("brands/{id:int}")]
        public IActionResult GetBrand(int id)
        {
            var brand = _ProductData.GetBrand(id);
            if (brand is null)
                return NotFound(id);
            return Ok(brand.ToDTO());
        }

        [HttpPost]
        public IActionResult GetProducts(ProductFilter Filter = null)
        {
            var products = _ProductData.GetProducts(Filter ?? new());
            return Ok(products.ToDTO());
        }

        [HttpGet("{id:int}")]
        public IActionResult GetProduct(int id)
        {
            var product = _ProductData.GetProductById(id);
            if (product is null)
                return NotFound(id);
            return Ok(product.ToDTO());
        }
    }
}
