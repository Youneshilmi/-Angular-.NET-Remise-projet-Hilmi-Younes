namespace Lensrock.Core.Entities;

public sealed class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = Roles.Customer;
    public DateTime CreatedAt { get; set; }
}

public static class Roles
{
    public const string Admin = "Admin";
    public const string Customer = "Customer";
}
