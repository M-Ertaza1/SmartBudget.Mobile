-- ============================================================
--  Smart Budget Tracker & Family Expense Manager
--  PostgreSQL Schema  (Phase 3)
--
--  HOW TO RUN:
--   1. Create the database first (run this line alone, connected
--      to the default 'postgres' database):
--          CREATE DATABASE smartbudget;
--   2. Switch your query tool to the 'smartbudget' database.
--   3. Run THIS entire file inside 'smartbudget'.
--
--  Notes:
--   - salary_day is NOT NULL (captured at registration) but is
--     fully editable later via UPDATE (Profile screen / API).
--   - mobile column added (required at registration in the app).
--   - All money is DECIMAL(18,2). All IDs are UUID.
-- ============================================================

-- ---------- Extensions ----------
CREATE EXTENSION IF NOT EXISTS "pgcrypto";  -- gen_random_uuid()
CREATE EXTENSION IF NOT EXISTS "pg_trgm";   -- fast text search

-- ============================================================
--  CORE
-- ============================================================

-- ---------- users ----------
CREATE TABLE users (
    user_id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    full_name          VARCHAR(100) NOT NULL,
    email              VARCHAR(255) NOT NULL UNIQUE,
    password_hash      TEXT NOT NULL,
    mobile             VARCHAR(20),
    salary_day         INT NOT NULL CHECK (salary_day BETWEEN 1 AND 31),
    is_email_verified  BOOLEAN NOT NULL DEFAULT FALSE,
    verification_token TEXT,
    reset_token        TEXT,
    reset_expires      TIMESTAMP WITH TIME ZONE,
    avatar_url         TEXT,
    is_active          BOOLEAN NOT NULL DEFAULT TRUE,
    created_at         TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at         TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
CREATE UNIQUE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_salary_day ON users(salary_day);

-- ---------- subscriptions ----------
CREATE TABLE subscriptions (
    subscription_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id         UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    plan            VARCHAR(20) NOT NULL DEFAULT 'free'
                        CHECK (plan IN ('free','premium')),
    status          VARCHAR(20) NOT NULL DEFAULT 'active'
                        CHECK (status IN ('active','expired','cancelled')),
    payment_ref     TEXT,
    started_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    expires_at      TIMESTAMP WITH TIME ZONE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_subs_user_id ON subscriptions(user_id);
CREATE INDEX idx_subs_status  ON subscriptions(status);

-- ============================================================
--  FINANCE
-- ============================================================

-- ---------- budget_cycles ----------
CREATE TABLE budget_cycles (
    cycle_id   UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id    UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    start_date DATE NOT NULL,
    end_date   DATE NOT NULL,
    is_active  BOOLEAN NOT NULL DEFAULT TRUE,
    notes      TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    CONSTRAINT chk_cycle_dates CHECK (end_date > start_date)
);
CREATE INDEX idx_cycles_user_id ON budget_cycles(user_id);
CREATE INDEX idx_cycles_dates   ON budget_cycles(user_id, start_date, end_date);
CREATE INDEX idx_cycles_active  ON budget_cycles(user_id, is_active);

-- ---------- income ----------
CREATE TABLE income (
    income_id   UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    cycle_id    UUID NOT NULL REFERENCES budget_cycles(cycle_id) ON DELETE CASCADE,
    user_id     UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    amount      DECIMAL(18,2) NOT NULL CHECK (amount > 0),
    source      VARCHAR(100) NOT NULL,
    income_date DATE NOT NULL,
    notes       TEXT,
    created_at  TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_income_cycle_id ON income(cycle_id);
CREATE INDEX idx_income_user_id  ON income(user_id);

-- ---------- expense_categories ----------
CREATE TABLE expense_categories (
    category_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id     UUID REFERENCES users(user_id) ON DELETE CASCADE,  -- NULL for system categories
    name        VARCHAR(80) NOT NULL,
    icon        VARCHAR(50),
    color       VARCHAR(7),
    is_system   BOOLEAN NOT NULL DEFAULT FALSE,
    is_active   BOOLEAN NOT NULL DEFAULT TRUE,
    created_at  TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_cats_user_id   ON expense_categories(user_id);
CREATE INDEX idx_cats_is_system ON expense_categories(is_system);

-- Seed the 10 system categories
INSERT INTO expense_categories (name, icon, color, is_system) VALUES
('Food & Dining', 'fork-knife',     '#FF6B35', TRUE),
('Transport',     'car',            '#4ECDC4', TRUE),
('Utilities',     'zap',            '#45B7D1', TRUE),
('Healthcare',    'heart',          '#96CEB4', TRUE),
('Entertainment', 'film',           '#FFEAA7', TRUE),
('Education',     'book',           '#DDA0DD', TRUE),
('Clothing',      'shopping-bag',   '#F0A500', TRUE),
('Personal Care', 'user',           '#A8E6CF', TRUE),
('Savings',       'piggy-bank',     '#88D8B0', TRUE),
('Other',         'more-horizontal','#BDC3C7', TRUE);

-- ---------- expenses ----------
CREATE TABLE expenses (
    expense_id   UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    cycle_id     UUID NOT NULL REFERENCES budget_cycles(cycle_id) ON DELETE CASCADE,
    user_id      UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    category_id  UUID NOT NULL REFERENCES expense_categories(category_id),
    amount       DECIMAL(18,2) NOT NULL CHECK (amount > 0),
    description  TEXT,
    expense_date DATE NOT NULL,
    receipt_url  TEXT,
    created_at   TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at   TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_exp_cycle_id    ON expenses(cycle_id);
CREATE INDEX idx_exp_user_id     ON expenses(user_id);
CREATE INDEX idx_exp_category_id ON expenses(category_id);
CREATE INDEX idx_exp_date        ON expenses(expense_date);

-- ---------- fixed_expenses ----------
CREATE TABLE fixed_expenses (
    fixed_id    UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id     UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    category_id UUID NOT NULL REFERENCES expense_categories(category_id),
    name        VARCHAR(100) NOT NULL,
    amount      DECIMAL(18,2) NOT NULL CHECK (amount > 0),
    due_day     INT NOT NULL CHECK (due_day BETWEEN 1 AND 31),
    is_paid     BOOLEAN NOT NULL DEFAULT FALSE,
    is_active   BOOLEAN NOT NULL DEFAULT TRUE,
    created_at  TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fixed_user_id ON fixed_expenses(user_id);
CREATE INDEX idx_fixed_active  ON fixed_expenses(user_id, is_active);

-- ---------- bill_reminders ----------
CREATE TABLE bill_reminders (
    reminder_id        UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id            UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    name               VARCHAR(100) NOT NULL,
    amount             DECIMAL(18,2),
    due_date           DATE NOT NULL,
    remind_days_before INT NOT NULL DEFAULT 3,
    repeat             VARCHAR(20) NOT NULL DEFAULT 'once'
                          CHECK (repeat IN ('once','monthly','yearly')),
    is_paid            BOOLEAN NOT NULL DEFAULT FALSE,
    notified           BOOLEAN NOT NULL DEFAULT FALSE,
    created_at         TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_bills_user_id  ON bill_reminders(user_id);
CREATE INDEX idx_bills_due_date ON bill_reminders(due_date);
CREATE INDEX idx_bills_notified ON bill_reminders(notified, due_date);

-- ============================================================
--  GROCERY
-- ============================================================

-- ---------- grocery_lists ----------
CREATE TABLE grocery_lists (
    list_id    UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    cycle_id   UUID NOT NULL REFERENCES budget_cycles(cycle_id) ON DELETE CASCADE,
    user_id    UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    name       VARCHAR(100) NOT NULL,
    is_active  BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_glists_cycle_id ON grocery_lists(cycle_id);
CREATE INDEX idx_glists_user_id  ON grocery_lists(user_id);

-- ---------- grocery_items ----------
CREATE TABLE grocery_items (
    item_id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    list_id         UUID NOT NULL REFERENCES grocery_lists(list_id) ON DELETE CASCADE,
    name            VARCHAR(150) NOT NULL,
    estimated_price DECIMAL(18,2),
    actual_price    DECIMAL(18,2),
    quantity        INT NOT NULL DEFAULT 1 CHECK (quantity > 0),
    unit            VARCHAR(20),
    is_purchased    BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_gitems_list_id ON grocery_items(list_id);

-- ============================================================
--  FAMILY / PREMIUM
-- ============================================================

-- ---------- family_members ----------
CREATE TABLE family_members (
    member_id    UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id      UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    full_name    VARCHAR(100) NOT NULL,
    relationship VARCHAR(50) NOT NULL,
    salary       DECIMAL(18,2),  -- informational ONLY, never added to budget
    phone        VARCHAR(20),
    email        VARCHAR(255),
    is_active    BOOLEAN NOT NULL DEFAULT TRUE,
    created_at   TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fam_members_user_id ON family_members(user_id);

-- ---------- contribution_history ----------
CREATE TABLE contribution_history (
    contribution_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    cycle_id        UUID NOT NULL REFERENCES budget_cycles(cycle_id) ON DELETE CASCADE,
    member_id       UUID NOT NULL REFERENCES family_members(member_id),
    user_id         UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    amount          DECIMAL(18,2) NOT NULL CHECK (amount > 0),  -- contribution, NOT salary
    contributed_at  TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    notes           TEXT,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_contrib_cycle_id  ON contribution_history(cycle_id);
CREATE INDEX idx_contrib_member_id ON contribution_history(member_id);

-- ============================================================
--  SYSTEM
-- ============================================================

-- ---------- notifications ----------
CREATE TABLE notifications (
    notification_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id         UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    title           VARCHAR(200) NOT NULL,
    body            TEXT NOT NULL,
    type            VARCHAR(50) NOT NULL
                       CHECK (type IN ('bill','cycle','balance','system')),
    reference_id    UUID,
    is_read         BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_notif_user_id    ON notifications(user_id);
CREATE INDEX idx_notif_is_read    ON notifications(user_id, is_read);
CREATE INDEX idx_notif_created_at ON notifications(created_at);

-- ---------- audit_logs ----------
CREATE TABLE audit_logs (
    log_id      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id     UUID REFERENCES users(user_id),  -- NULL for system actions
    table_name  VARCHAR(100) NOT NULL,
    record_id   UUID NOT NULL,
    action      VARCHAR(10) NOT NULL
                   CHECK (action IN ('INSERT','UPDATE','DELETE')),
    old_values  JSONB,
    new_values  JSONB,
    ip_address  VARCHAR(45),
    created_at  TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_audit_user_id    ON audit_logs(user_id);
CREATE INDEX idx_audit_table      ON audit_logs(table_name);
CREATE INDEX idx_audit_created_at ON audit_logs(created_at);

-- ============================================================
--  FUNCTIONS
-- ============================================================

-- Salary cycle date calculator
-- Example: fn_get_cycle_dates(5, '2026-06-20')  ->  2026-06-05 .. 2026-07-04
CREATE OR REPLACE FUNCTION fn_get_cycle_dates(
    p_salary_day     INT,
    p_reference_date DATE DEFAULT CURRENT_DATE
) RETURNS TABLE(cycle_start DATE, cycle_end DATE) AS $$
DECLARE v_start DATE; v_end DATE;
BEGIN
    IF EXTRACT(DAY FROM p_reference_date) >= p_salary_day THEN
        v_start := DATE_TRUNC('month', p_reference_date)
                   + (p_salary_day - 1) * INTERVAL '1 day';
    ELSE
        v_start := DATE_TRUNC('month', p_reference_date - INTERVAL '1 month')
                   + (p_salary_day - 1) * INTERVAL '1 day';
    END IF;
    v_end := (v_start + INTERVAL '1 month') - INTERVAL '1 day';
    RETURN QUERY SELECT v_start::DATE, v_end::DATE;
END; $$ LANGUAGE plpgsql;

-- ============================================================
--  Quick verification (run after to confirm everything exists)
-- ============================================================
-- SELECT table_name FROM information_schema.tables
--   WHERE table_schema = 'public' ORDER BY table_name;
-- SELECT * FROM expense_categories WHERE is_system = TRUE;
-- SELECT * FROM fn_get_cycle_dates(5, '2026-06-20');
