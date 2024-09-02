using MvcShop.Domain.Models;
using MvcShop.Infrastructure.Data;
using MvcShop.Infrastructure.Repositories;
using MvcShop.Web.Constraints;
using MvcShop.Web.Transformer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
    options.ConstraintMap["validateSlug"] = typeof(SlugConstraint);
    options.ConstraintMap["slugTransform"] = typeof(SlugParameterTransformer);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MvcShopContext>(ServiceLifetime.Scoped);
builder.Services.AddTransient<IRepository<Customer>, CustomerRepository>();
builder.Services.AddTransient<IRepository<Product>, ProductRepository>();
builder.Services.AddTransient<IRepository<Order>, OrderRepository>();
builder.Services.AddTransient<IRepository<Cart>, CartRepository>();
builder.Services.AddTransient<ICartRepository, CartRepository>();

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

//app.MapControllerRoute(
//    name: "TicketDetailsRoute",
//    defaults: new { action = "TicketDetails", controller = "Home" },
//    pattern: "/details/{productId}/{slug?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MvcShopContext>();

    MvcShopContext.CreateInitialDatabase(context);
}

app.Run();
