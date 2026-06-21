namespace Lensrock.Core.Models;

public sealed record RegisterRequest(string FullName, string Email, string Password);
public sealed record LoginRequest(string Email, string Password);
public sealed record ServiceInput(
    int CategoryId,
    string Name,
    string Description,
    int DurationMinutes,
    decimal Price,
    string ImageUrl,
    bool IsActive = true);
public sealed record AddCartItemRequest(int ServiceId, int Quantity = 1);
public sealed record UpdateCartItemRequest(int Quantity);
public sealed record CheckoutRequest(DateTime BookingDate, string Notes);
public sealed record UpdateOrderStatusRequest(string Status);

public sealed record DashboardSummary(
    int UserCount,
    int ActiveServiceCount,
    int OrderCount,
    decimal Revenue);
