using Microsoft.FeatureManagement;
using sqlapp.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Endpoint=https://newresourcename.azconfig.io;Id=QrGi-l8-s0:iIBr6DXEmRw1o2LfyXL7;Secret=PndhdCBdlF6JDcD8TiPxJ+g/oLlNtGiyDqQp9r0lCSc=";

builder.Host.ConfigureAppConfiguration(app =>
{
    app.AddAzureAppConfiguration(options=>
    options.Connect(connectionString).UseFeatureFlags()
    );
});

builder.Services.AddTransient<IProductService, ProductService>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddFeatureManagement();

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

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
