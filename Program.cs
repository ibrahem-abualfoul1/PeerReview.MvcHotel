using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeerReview.MvcHotel.Services;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using PeerReview.MvcHotel.Middleware;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Services (قبل Build) --------------------
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services
    .AddControllersWithViews(options =>
    {
        // جعل الموقع محمي افتراضياً: يتطلب مستخدم مُسجّل الدخول
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        options.Filters.Add(new AuthorizeFilter(policy));
    })
    .AddRazorRuntimeCompilation()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.AddHttpClient();
builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// DI لخدماتك
builder.Services.AddScoped<PeerReview.MvcHotel.Services.ApiClient>();
builder.Services.AddScoped<PeerReview.MvcHotel.Services.AuthService>();
builder.Services.AddScoped<PeerReview.MvcHotel.Services.UsersService>();
builder.Services.AddScoped<PeerReview.MvcHotel.Services.RolesService>();
builder.Services.AddScoped<PeerReview.MvcHotel.Services.AnswerScoringService>();

builder.Services.AddScoped<PeerReview.MvcHotel.Services.QuestionsService>();
builder.Services.AddScoped<PeerReview.MvcHotel.Services.AssignmentsService>();
builder.Services.AddScoped<PeerReview.MvcHotel.Services.AnswersService>();
builder.Services.AddScoped<PeerReview.MvcHotel.Services.LookupsService>();
builder.Services.AddScoped<IQuestionsService, FileQuestionsService>();

// المصادقة والتفويض (كوكي أوث)
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

// إعدادات اللغات المدعومة والـ RequestLocalizationOptions
var supportedCultures = new[]
{
    //new CultureInfo("ar"),
    new CultureInfo("en")
};

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

// ترتيب مزوّدات تحديد الثقافة (من الأعلى أولوية إلى الأدنى)
localizationOptions.RequestCultureProviders = new IRequestCultureProvider[]
{
    new QueryStringRequestCultureProvider(),          // ?culture=ar&ui-culture=ar
    new CookieRequestCultureProvider(),               // من الكوكيز
    new AcceptLanguageHeaderRequestCultureProvider()  // من الهيدر
};

builder.WebHost.ConfigureKestrel(options =>
{
    // 100 MB مثلاً
    options.Limits.MaxRequestBodySize = 157_286_400;
});
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 157_286_400; // 150 MB
});
// -------------------- Build --------------------
var app = builder.Build();

// -------------------- Middleware Pipeline --------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// يُفضَّل وضع اللوقَلَزة قبل الراوتينغ ليتم اختيار الثقافة مبكراً
app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// الجلسات قبل الأوثORIZATION وبعد الراوتينغ
app.UseSession();

app.UseMiddleware<SessionValidationMiddleware>();

// ترتيب الأوثنتكيشن ثم الأوثورايزيشن مهم
app.UseAuthentication();
app.UseAuthorization();

// -------------------- Endpoints --------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
