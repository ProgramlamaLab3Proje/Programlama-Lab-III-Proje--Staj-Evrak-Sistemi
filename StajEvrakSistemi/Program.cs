using StajEvrakSistemi.Services;
using StajEvrakSistemi.Data;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<Context>();
builder.Services.AddScoped<PasswordService>();

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{          options.LoginPath = "/Account/Login";
           options.Events.OnRedirectToAccessDenied = context =>
           {
               context.Response.StatusCode = StatusCodes.Status403Forbidden; //!!
               return Task.CompletedTask;
           };
           options.Cookie.HttpOnly = true; // js tarafından okunmasını engeller
           options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // yalnızca https üzerinden gönderilmesini sağlar
           options.ExpireTimeSpan = TimeSpan.FromMinutes(30);  // giriş cookiesinin geçerlilik süresini belirler
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting(); // hangi sayfaya gidildiğini belirler

app.UseAuthentication(); // isteği yapan kullanıcının kim olduğunu belirler

app.UseAuthorization(); // kullanıcının sayfaya erişim yetkisini kontrol eder

//app.UseStatusCodePagesWithReExecute("/Account/AccessDenied"); // 403 hatasında açılacak sayfa yolu

app.MapRazorPages();

app.Run();
