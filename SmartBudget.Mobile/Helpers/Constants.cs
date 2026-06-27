using System;
using System.Collections.Generic;
using System.Text;

namespace SmartBudget.Mobile.Helpers;

public static class Constants
{
#if DEBUG
    public const string BaseUrl = "https://10.0.2.2:5001/api/v1/";
#else
    public const string BaseUrl = "https://api.smartbudget.app/api/v1/";
#endif

    public const string AuthTokenKey = "auth_token";
    public const string UserIdKey = "user_id";

    public const string SalaryDayKey = "salary_day";
    public const string CurrencySymbol = "Rs ";   // change to "$", "₹", "£" etc. as you like
}
