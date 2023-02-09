using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapPost("/", () => new {Name = "Lucas", Idade = 24});
app.MapGet("/addHeader", (HttpResponse response) => response.Headers.Add("Teste", "Lucas Celestino"));

app.MapGet("/product/{code}", ([FromRoute] string code) => {
    var product = ProductRepository.GetBy(code);

    if(product == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(product);

});

app.MapPost("/product", (Product product) => {
    ProductRepository.AddProduct(product);
    return Results.Created($"/product/{product.Code}", product.Code);
});

app.MapPut("/product", (Product productParam) => {
    var product = ProductRepository.GetBy(productParam.Code);
    product.Name = productParam.Name;
    return Results.Ok(product);
});

app.MapDelete("product/{code}", ([FromRoute] string code) => {
    var product = ProductRepository.GetBy(code);

    if(product == null)
    {
        return Results.NotFound();
    }

    ProductRepository.Remove(product);

    return Results.Ok();
});

app.Run();

public static class ProductRepository
{
    public static List<Product> Products {get;set;}

    public static void AddProduct(Product product)
    {
        if(Products == null)
        {
            Products = new List<Product>();
        }

        Products.Add(product);
    }

    public static Product GetBy(string code)
    {
        return Products.FirstOrDefault(p => p.Code == code);
    }

    public static void Remove(Product product)
    {
        Products.Remove(product);
    }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Product
{
    public int Id {get;set;}
    public string Code {get;set;}
    public string Name {get;set;}
    public string Description {get;set;}
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products {get;set;}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Product>().Property(p => p.Code).HasMaxLength(20).IsRequired();

        builder.Entity<Product>().Property(p => p.Name).HasMaxLength(120).IsRequired();

        builder.Entity<Product>().Property(p => p.Description).HasMaxLength(500).IsRequired(false);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer("Server=localhost;Database=Products;User Id=sa;Password=@8991Celestino;MultipleActiveResultSets=true;Encrypt=YES;TrustServerCertificate=YES");
    }
}