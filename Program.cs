using Scalar.AspNetCore;

//Initialise l'application ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);

// Services//Enregistre ProductService dans le conteneur DI avec une durée de vie Singleton.
builder.Services.AddSingleton<IProductService, ProductService>();
//Génère automatiquement la documentation OpenAPI de l'API.
builder.Services.AddOpenApi();
//Standardise les réponses d'erreur HTTP au format Problem Details.
builder.Services.AddProblemDetails();

//Construit l'application ASP.NET Core.
var app = builder.Build();
// Middleware
//Capture les exceptions non gérées et renvoie des erreurs HTTP propres.
app.UseExceptionHandler(); 
//Expose la spécification OpenAPI de l'API.
app.MapOpenApi();  
//Fournit une interface web moderne pour explorer et tester l'API à partir de la documentation OpenAPI.        
app.MapScalarApiReference();

// API Routes
var api = app.MapGroup("/api/v1");

api.MapGet("/products", async (
    IProductService service,
    int page = 1,
    int pageSize = 20
    ) =>
{
    pageSize = Math.Clamp(pageSize, 1, 100);
    var (products, totalCount) = await service.GetPagedAsync(page, pageSize);
    var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

    return Results.Ok(new
    {
        data = products.Select(p => new ProductResponse(
            p.Id, p.Name, p.Description, p.Price, p.Stock, p.Category, p.CreatedAt, p.UpdatedAt)),
        pagination = new
        {
            page,
            pageSize,
            totalPages,
            totalCount,
            hasNext = page < totalPages,
            hasPrevious = page > 1
        }
    });
})
.WithName("GetProducts")
.WithSummary("Get all products with pagination")
.WithTags("Products");

api.MapGet("/products/{id:int}", async (int id, IProductService service) =>
{
    var product = await service.GetByIdAsync(id);
    return product is null
        ? Results.NotFound(new { error = $"Product with ID {id} not found" })
        : Results.Ok(new ProductResponse(
            product.Id, product.Name, product.Description,
            product.Price, product.Stock, product.Category,
            product.CreatedAt, product.UpdatedAt));
})
.WithName("GetProductById")
.WithSummary("Get a product by ID")
.Produces<ProductResponse>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithTags("Products");

api.MapPost("/products", async (CreateProductRequest request, IProductService service) =>
{
    var product = await service.CreateAsync(request);
    var response = new ProductResponse(
        product.Id, product.Name, product.Description,
        product.Price, product.Stock, product.Category,
        product.CreatedAt, product.UpdatedAt);

    return Results.Created($"/api/v1/products/{product.Id}", response);
})
.WithName("CreateProduct")
.WithSummary("Create a new product")
.Produces<ProductResponse>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
.WithTags("Products");

api.MapPut("/products/{id:int}", async (int id, UpdateProductRequest request, IProductService service) =>
{
    var product = await service.UpdateAsync(id, request);
    return product is null
        ? Results.NotFound(new { error = $"Product with ID {id} not found" })
        : Results.NoContent();
})
.WithName("UpdateProduct")
.WithSummary("Update an existing product")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.WithTags("Products");

api.MapPatch("/products/{id:int}", async (int id, PatchProductRequest request, IProductService service) =>
{
    var product = await service.PatchAsync(id, request);
    return product is null
        ? Results.NotFound(new { error = $"Product with ID {id} not found" })
        : Results.NoContent();
})
.WithName("PatchProduct")
.WithSummary("Partially update a product")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.WithTags("Products");

api.MapDelete("/products/{id:int}", async (int id, IProductService service) =>
{
    var deleted = await service.DeleteAsync(id);
    return deleted
        ? Results.NoContent()
        : Results.NotFound(new { error = $"Product with ID {id} not found" });
})
.WithName("DeleteProduct")
.WithSummary("Delete a product")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.WithTags("Products");

app.Run();