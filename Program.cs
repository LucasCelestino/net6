using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);
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

app.MapPost("/product", (ProductRequest productRequest, ApplicationDbContext context) => {
    var category = context.Category.Where(c => c.Id == productRequest.CategoryId).First();

    var product = new Product{
        Code = productRequest.Code,
        Name = productRequest.Name,
        Description = productRequest.Description,
        Category = category
    };
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
