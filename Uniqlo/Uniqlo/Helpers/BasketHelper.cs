using System.Text.Json;
using Uniqlo.ViewModels.Baskets;

namespace Uniqlo.Helpers;

public class BasketHelper
{
    public static List<BasketCookieItemVM> GetBasket(HttpRequest request)
    {
        string? value = request.Cookies["Basket"];
        if (value is null) return new();
        
        return JsonSerializer.Deserialize<List<BasketCookieItemVM>>(value) ?? new();
    }
}