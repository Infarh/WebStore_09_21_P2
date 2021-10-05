using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using WebStore.Interfaces.Services;
using WebStore.Services.Mapping;

namespace WebStore.Controllers
{
    //[Controller]
    public class HomeController : Controller
    {
        private readonly IConfiguration _Configuration;

        public HomeController(IConfiguration Configuration) => _Configuration = Configuration;

        public IActionResult Index([FromServices] IProductData ProductData)
        {
            ViewBag.Products = ProductData.GetProducts(new() { Page = 1, PageSize = 9 }).Products.ToView();
            return View();
        }

        public IActionResult SecondAction()
        {
            return Content(_Configuration["Greetings"]);
            //return View("Index");
        }

        public IActionResult Blog() => View();
        //public IActionResult BlogSingle() => View();
    }
}
