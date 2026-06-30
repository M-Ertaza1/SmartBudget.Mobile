using System;
using System.Collections.Generic;
using System.Text;

namespace SmartBudget.Mobile.Helpers;

public static class Constants
{
#if DEBUG
    // Windows desktop talks to the API directly over localhost (http avoids dev-cert hassle).
    // Android emulator would use http://10.0.2.2:5283/ instead.
    public const string BaseUrl = "http://localhost:5283/api/v1/";
#else
    public const string BaseUrl = "https://api.smartbudget.app/api/v1/";
#endif

    public const string AuthTokenKey = "auth_token";
    public const string UserIdKey = "user_id";
    public const string SalaryDayKey = "salary_day";
    public const string CurrencySymbol = "Rs ";
}
