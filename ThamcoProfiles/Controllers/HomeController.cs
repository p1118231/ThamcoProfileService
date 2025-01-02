using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ThamcoProfiles.Models;
using System;
using ThamcoProfiles.Services.ProductRepo;
using Microsoft.AspNetCore.Authorization;

namespace ThamcoProfiles.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;

    public HomeController(ILogger<HomeController> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    //display the products from the product service 

    [HttpGet("Products")]
        [Authorize]
        public async Task<IActionResult> Products()
        {
            IEnumerable<ProductDto> products = null!;

            try{

                products = await _productService.GetProductsAsync();

            }
            catch{

                _logger.LogWarning("failure to access undercutters service ");
                products= Array.Empty<ProductDto>();

            }

            return View(products);

        }
    //display products 
    public async Task<IActionResult> Index()
    {
        IEnumerable<ProductDto> products = null!;

            try{

                products = await _productService.GetProductsAsync();

            }
            catch (Exception ex){

                _logger.LogWarning($"failure to access product service : {ex.Message}");
                products= Array.Empty<ProductDto>();

            }

            return View(products);
    }


    //search for products 
    public async Task<IActionResult> Search(string query)
    
        {
            ViewBag.Message = "Please enter a search term.";
            var allProducts = await _productService.GetProductsAsync();
            var filteredProducts = allProducts.Where(p => (p.Name??"").Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
            
            return View("Index", filteredProducts); // Return filtered products to the Index view
        }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
