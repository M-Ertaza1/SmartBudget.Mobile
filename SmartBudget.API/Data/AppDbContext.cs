using Microsoft.EntityFrameworkCore;
using SmartBudget.API.Models;

namespace SmartBudget.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<BudgetCycle> BudgetCycles => Set<BudgetCycle>();
    public DbSet<Income> Income => Set<Income>();
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // ---------- users ----------
        b.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(x => x.UserId);
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.FullName).HasColumnName("full_name");
            e.Property(x => x.Email).HasColumnName("email");
            e.Property(x => x.PasswordHash).HasColumnName("password_hash");
            e.Property(x => x.Mobile).HasColumnName("mobile");
            e.Property(x => x.SalaryDay).HasColumnName("salary_day");
            e.Property(x => x.IsEmailVerified).HasColumnName("is_email_verified");
            e.Property(x => x.VerificationToken).HasColumnName("verification_token");
            e.Property(x => x.ResetToken).HasColumnName("reset_token");
            e.Property(x => x.ResetExpires).HasColumnName("reset_expires");
            e.Property(x => x.AvatarUrl).HasColumnName("avatar_url");
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.Email).IsUnique();
        });

        // ---------- subscriptions ----------
        b.Entity<Subscription>(e =>
        {
            e.ToTable("subscriptions");
            e.HasKey(x => x.SubscriptionId);
            e.Property(x => x.SubscriptionId).HasColumnName("subscription_id");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.Plan).HasColumnName("plan");
            e.Property(x => x.Status).HasColumnName("status");
            e.Property(x => x.PaymentRef).HasColumnName("payment_ref");
            e.Property(x => x.StartedAt).HasColumnName("started_at");
            e.Property(x => x.ExpiresAt).HasColumnName("expires_at");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasOne(x => x.User).WithOne(x => x.Subscription)
                .HasForeignKey<Subscription>(x => x.UserId);
        });

        // ---------- budget_cycles ----------
        b.Entity<BudgetCycle>(e =>
        {
            e.ToTable("budget_cycles");
            e.HasKey(x => x.CycleId);
            e.Property(x => x.CycleId).HasColumnName("cycle_id");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.StartDate).HasColumnName("start_date");
            e.Property(x => x.EndDate).HasColumnName("end_date");
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.Notes).HasColumnName("notes");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasOne(x => x.User).WithMany(x => x.BudgetCycles)
                .HasForeignKey(x => x.UserId);
        });

        // ---------- income ----------
        b.Entity<Income>(e =>
        {
            e.ToTable("income");
            e.HasKey(x => x.IncomeId);
            e.Property(x => x.IncomeId).HasColumnName("income_id");
            e.Property(x => x.CycleId).HasColumnName("cycle_id");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.Amount).HasColumnName("amount").HasColumnType("decimal(18,2)");
            e.Property(x => x.Source).HasColumnName("source");
            e.Property(x => x.IncomeDate).HasColumnName("income_date");
            e.Property(x => x.Notes).HasColumnName("notes");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasOne(x => x.Cycle).WithMany(x => x.Incomes)
                .HasForeignKey(x => x.CycleId);
        });

        // ---------- expense_categories ----------
        b.Entity<ExpenseCategory>(e =>
        {
            e.ToTable("expense_categories");
            e.HasKey(x => x.CategoryId);
            e.Property(x => x.CategoryId).HasColumnName("category_id");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.Name).HasColumnName("name");
            e.Property(x => x.Icon).HasColumnName("icon");
            e.Property(x => x.Color).HasColumnName("color");
            e.Property(x => x.IsSystem).HasColumnName("is_system");
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        // ---------- expenses ----------
        b.Entity<Expense>(e =>
        {
            e.ToTable("expenses");
            e.HasKey(x => x.ExpenseId);
            e.Property(x => x.ExpenseId).HasColumnName("expense_id");
            e.Property(x => x.CycleId).HasColumnName("cycle_id");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.CategoryId).HasColumnName("category_id");
            e.Property(x => x.Amount).HasColumnName("amount").HasColumnType("decimal(18,2)");
            e.Property(x => x.Description).HasColumnName("description");
            e.Property(x => x.ExpenseDate).HasColumnName("expense_date");
            e.Property(x => x.ReceiptUrl).HasColumnName("receipt_url");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(x => x.Cycle).WithMany(x => x.Expenses)
                .HasForeignKey(x => x.CycleId);
            e.HasOne(x => x.Category).WithMany(x => x.Expenses)
                .HasForeignKey(x => x.CategoryId);
        });
    }
}