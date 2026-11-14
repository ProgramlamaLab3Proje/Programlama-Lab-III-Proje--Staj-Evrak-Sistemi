using StajEvrakSistemi.Services;
using StajEvrakSistemi.Data;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<Context>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<AdminSeeder>();

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{          options.LoginPath = "/Account/Login";
           options.Events.OnRedirectToAccessDenied = context =>
           {
               context.Response.StatusCode = StatusCodes.Status403Forbidden; //!!
               return Task.CompletedTask;
           };
           options.Cookie.HttpOnly = true; // js tarafından okunmasını engeller
           options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // istek ne türde ise (http/https) o türde oluşturur cookieyi
});

var app = builder.Build();

using (var scope = app.Services.CreateScope()) // create scope geçici erişim alanı oluşturur
{
    var db = scope.ServiceProvider.GetRequiredService<Context>(); // context nesnesi alınıyor
    db.Database.Migrate(); // veritabanı yoksa oluşturuyor, varsa güncelliyor
    scope.ServiceProvider.GetRequiredService<AdminSeeder>().Seed(); // adminseeder nesnesi alınıyor ve seed metodu çağrılıyor
}

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
