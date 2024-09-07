using Microsoft.AspNetCore.Authentication.Cookies;
using MvcShop.Domain.Models;
using MvcShop.Infrastructure.Data;
using MvcShop.Infrastructure.Repositories;
using MvcShop.Web.Constraints;
using MvcShop.Web.Repositories;
using MvcShop.Web.Transformer;
using MvcShop.Web.ValueProviders;

var builder = WebApplication.CreateBuilder(args);

/// Example of setting a more scalable Cache solution (Redis)
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration.GetConnectionString("MyRedisConStr");
//    options.InstanceName = "SampleInstance";
//});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.LoginPath = "/Login";
    });

builder.Services.AddRouting(options =>
{
    options.ConstraintMap["validateSlug"] = typeof(SlugConstraint);
    options.ConstraintMap["slugTransform"] = typeof(SlugParameterTransformer);
});

builder.Services.AddControllersWithViews(options =>
    options.ValueProviderFactories.Add(new SessionValueProviderFactory())
);

builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<MvcShopContext>(ServiceLifetime.Scoped);

builder.Services.AddTransient<IStateRepository, SessionStateRepository>();
builder.Services.AddTransient<IRepository<Customer>, CustomerRepository>();
builder.Services.AddTransient<IRepository<Product>, ProductRepository>();
builder.Services.AddTransient<IRepository<Order>, OrderRepository>();
builder.Services.AddTransient<IRepository<Cart>, CartRepository>();
builder.Services.AddTransient<ICartRepository, CartRepository>();

// Replaced by TimerFilter Attribute
//builder.Services.AddScoped<TimerFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

//app.MapControllerRoute(
//    name: "TicketDetailsRoute",
//    defaults: new { action = "TicketDetails", controller = "Home" },
//    pattern: "/details/{productId}/{slug?}");

app.MapControllerRoute(
    name: "administrationDefault",
    defaults: new { controller = "Home" },
    pattern: "{area:exists}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "administration",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MvcShopContext>();

    MvcShopContext.CreateInitialDatabase(context);
}

app.Run();