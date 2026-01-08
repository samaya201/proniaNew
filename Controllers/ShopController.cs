using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pronia.Abstraction;
using Pronia.Context;
using Pronia.Models;
using Pronia.ViewModels.ProductViewModels;

namespace Pronia.Controllers;

[Authorize]
public class ShopController : Controller
{
 
    AppDbContext context;
   
    private readonly IEmailService _service;
    public ShopController(AppDbContext _context, IEmailService service)
    {
        context = _context;
        _service = service;
    }

    public IActionResult Index()
    {
        var products = context.Products
            .Select(p => new ProductGetVM
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                SKU = p.SKU,
                Rating = p.Rating,
                CategoryName = p.Category.Name,
                MainImageUrl = p.MainImageUrl,
                HoverImageUrl = p.HoverImageUrl
            })
            .ToList();
        return View(products);
    }
    public async Task<IActionResult> Test()
    {
        await _service.SendEmailAsync("aliyevasamaya33@gmail.com", "MPA101", "Email service is created");
        return Ok("Ok");
    }

}