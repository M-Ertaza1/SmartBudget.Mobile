using Microsoft.EntityFrameworkCore;
using SmartBudget.API.Common;
using SmartBudget.API.Data;
using SmartBudget.API.DTOs.Auth;
using SmartBudget.API.Helpers;
using SmartBudget.API.Models;

namespace SmartBudget.API.Services;

public interface IAuthService
{
    Task<ApiResponse<RegisterResultDto>> RegisterAsync(RegisterDto dto);
    Task<ApiResponse<bool>> VerifyEmailAsync(VerifyEmailDto dto);
    Task<ApiResponse<TokenResponseDto>> LoginAsync(LoginDto dto);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IJwtTokenGenerator _jwt;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext db, IJwtTokenGenerator jwt, ILogger<AuthService> logger)
    {
        _db = db;
        _jwt = jwt;
        _logger = logger;
    }

    public async Task<ApiResponse<RegisterResultDto>> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        if (await _db.Users.AnyAsync(u => u.Email == email))
            return ApiResponse<RegisterResultDto>.Fail("Email is already registered.");

        var now = DateTimeOffset.UtcNow;
        var code = Random.Shared.Next(100000, 999999).ToString();

        var user = new User
        {
            UserId = Guid.NewGuid(),
            FullName = dto.FullName.Trim(),
            Email = email,
            Mobile = dto.Mobile.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 12),
            SalaryDay = dto.SalaryDay,
            IsEmailVerified = false,
            VerificationToken = code,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        var sub = new Subscription
        {
            SubscriptionId = Guid.NewGuid(),
            UserId = user.UserId,
            Plan = "free",
            Status = "active",
            StartedAt = now,
            CreatedAt = now
        };

        var (start, end) = CycleCalculator.GetCycleDates(
            dto.SalaryDay, DateOnly.FromDateTime(DateTime.UtcNow));

        var cycle = new BudgetCycle
        {
            CycleId = Guid.NewGuid(),
            UserId = user.UserId,
            StartDate = start,
            EndDate = end,
            IsActive = true,
            CreatedAt = now
        };

        _db.Users.Add(user);
        _db.Subscriptions.Add(sub);
        _db.BudgetCycles.Add(cycle);
        await _db.SaveChangesAsync();

        // No SMTP yet — log the code so you can grab it during development.
        _logger.LogWarning("DEV VERIFICATION CODE for {Email}: {Code}", email, code);

        return ApiResponse<RegisterResultDto>.Ok(
            new RegisterResultDto { UserId = user.UserId, Message = "Verification code sent." });
    }

    public async Task<ApiResponse<bool>> VerifyEmailAsync(VerifyEmailDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user is null) return ApiResponse<bool>.Fail("Account not found.");
        if (user.IsEmailVerified) return ApiResponse<bool>.Ok(true, "Email already verified.");
        if (user.VerificationToken != dto.Code.Trim())
            return ApiResponse<bool>.Fail("Invalid or expired code.");

        user.IsEmailVerified = true;
        user.VerificationToken = null;
        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Email verified.");
    }

    public async Task<ApiResponse<TokenResponseDto>> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _db.Users
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return ApiResponse<TokenResponseDto>.Fail("Invalid credentials.");

        if (!user.IsEmailVerified)
            return ApiResponse<TokenResponseDto>.Fail("Please verify your email before logging in.");

        var plan = user.Subscription?.Plan ?? "free";
        var (token, expiresAt) = _jwt.Generate(user, plan);

        return ApiResponse<TokenResponseDto>.Ok(new TokenResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserSummaryDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Plan = plan,
                SalaryDay = user.SalaryDay
            }
        });
    }
}