namespace SmartBudget.Mobile.Models;

// Matches the API's response envelope: { success, data, error, message }
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResult
{
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public UserSummary? User { get; set; }
}

public class UserSummary
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Plan { get; set; } = "free";
}