namespace SmartBudget.API.DTOs.Auth;

public record RegisterDto(string FullName, string Email, string Mobile,
                          string Password, string ConfirmPassword, int SalaryDay);

public record LoginDto(string Email, string Password);

public record VerifyEmailDto(string Email, string Code);

public class TokenResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public UserSummaryDto User { get; set; } = null!;
}

public class UserSummaryDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Plan { get; set; } = "free";
    public int SalaryDay { get; set; }
}

public class RegisterResultDto
{
    public Guid UserId { get; set; }
    public string Message { get; set; } = string.Empty;
}