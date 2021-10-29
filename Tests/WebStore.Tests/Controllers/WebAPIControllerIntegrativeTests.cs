using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebStore.Domain;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;
using WebStore.Interfaces.TestAPI;
using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers
{
    [TestClass]
    public class WebAPIControllerIntegrativeTests
    {
        private string[] _ExpectedValues = Enumerable.Range(1, 10).Select(i => $"Value - {i}").ToArray();
        private WebApplicationFactory<Startup> _Host;

        [TestInitialize]
        public void Inititlize()
        {

            var value_service_mock = new Mock<IValuesService>();
            value_service_mock.Setup(c => c.GetAll()).Returns(_ExpectedValues);

            var products_service_mock = new Mock<IProductData>();
            products_service_mock
               .Setup(c => c.GetProducts(It.IsAny<ProductFilter>()))
               .Returns(new ProductsPage(Enumerable.Empty<Product>(), 0));

            _Host = new WebApplicationFactory<Startup>()
               .WithWebHostBuilder(host => host
               .ConfigureServices(services =>
                {
                    services.AddSingleton(value_service_mock.Object);
                    services.AddSingleton(products_service_mock.Object);
                })
            );
        }

        [TestMethod]
        public async Task GetValues()
        {
            var client = _Host.CreateClient();
            var response = await client.GetAsync("/WebAPI");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var response_stream = await response.Content.ReadAsStreamAsync();

            var parser = new HtmlParser();
            var html = parser.ParseDocument(response_stream);

            var items = html.QuerySelectorAll(".container table.table tbody tr td:last-child");

            Assert.Equal(_ExpectedValues, items.Select(i => i.Text()));
        }
    }
}
