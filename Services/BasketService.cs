using Pronia.Abstraction;
using Pronia.Context;
using System.Security.Claims;

namespace Pronia.Services;

public class BasketService(IHttpContextAccessor _accessor,AppDbContext _context):IBasketService
{
   public async Task<List<BasketItem>> GetBasketItemsAsync()
    {
        var userId = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var isExistUser = await _context.Users.AnyAsync(x => x.Id == userId);
        if (!isExistUser) return [];

        var basketItems = await _context.BasketItems
            .Where(x => x.AppUserId == userId)
            .Include(x => x.Product)
            .ToListAsync();
        return basketItems;

    }
}
