using StajEvrakSistemi.Services;
using StajEvrakSistemi.Data;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<Context>();
builder.Services.AddScoped<PasswordService>();

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie();

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

app.MapRazorPages();

app.Run();
