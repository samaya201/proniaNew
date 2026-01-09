using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pronia.Context;
using System.Security.Claims;

namespace Pronia.Controllers;

public class BasketController(AppDbContext _context) : Controller
{
    [Authorize]
    public async Task<IActionResult> AddToBasket(int productid)
    {

        var isExistProduct = await _context.Products.AnyAsync(x=>x.Id == productid);
        if (!isExistProduct) return NotFound();

       var userId= User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isExistUser = await _context.Users.AnyAsync(x => x.Id == userId);
        if (!isExistUser) return BadRequest();

        var existBasketItem = await _context.BasketItems
            .FirstOrDefaultAsync(x => x.ProductId == productid && x.AppUserId == userId);
        if (existBasketItem is not null)
        {
            existBasketItem.Count++;
            _context.BasketItems.Update(existBasketItem);
        }
        else
        {

            BasketItem basketItem = new()
            {
                ProductId = productid,
                Count = 1,
                AppUserId = userId!
            };
            await _context.BasketItems.AddAsync(basketItem);

        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index","Shop");
    }
    [Authorize]
    public async Task<IActionResult> RemoveFromBasketAsync( int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(x => x.Id == productId);
        if (!isExistProduct) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isExistUser = await _context.Users.AnyAsync(x => x.Id == userId);
        if (!isExistUser) return BadRequest();
        
        var existBasketItem = await _context.BasketItems
            .FirstOrDefaultAsync(x => x.ProductId == productId && x.AppUserId == userId);
        if (existBasketItem is null) return NotFound();

        _context.BasketItems.Remove(existBasketItem);
        await _context.SaveChangesAsync();


       var returnUrl= Request.Headers["Referer"];

        if (!string.IsNullOrWhiteSpace(returnUrl))
            return Redirect(returnUrl!); 

            return RedirectToAction("Index", "Shop");

    }
}
