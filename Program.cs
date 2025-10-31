
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeerReview.MvcHotel.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.AddHttpClient();
builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<PeerReview.MvcHotel.Services.ApiClient>();
builder.Services.AddScoped<PeerReview.MvcHotel.Services.AuthService>();
builder.Services.AddScoped<PeerReview.MvcHotel.Services.UsersService>();
builder.Services.AddScoped<PeerReview.MvcHotel.Services.QuestionsService>();
builder.Services.AddScoped<PeerReview.MvcHotel.Services.AssignmentsService>();
builder.Services.AddScoped<PeerReview.MvcHotel.Services.AnswersService>();
builder.Services.AddScoped<PeerReview.MvcHotel.Services.LookupsService>();
builder.Services.AddScoped<IQuestionsService, FileQuestionsService>();

var app = builder.Build();

var supportedCultures = new[]
{
    new CultureInfo("ar"), // ثقافة كاملة (لغة + منطقة)
    new CultureInfo("en")
};

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ar"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

// ترتيب مزوّدات تحديد الثقافة (من الأعلى أولوية إلى الأدنى)
localizationOptions.RequestCultureProviders = new IRequestCultureProvider[]
{
    new QueryStringRequestCultureProvider(), // ?culture=ar-SA&ui-culture=ar-SA
    new CookieRequestCultureProvider(),      // من الكوكيز
    new AcceptLanguageHeaderRequestCultureProvider() // من الهيدر
};

app.UseRequestLocalization(localizationOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
