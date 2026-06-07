using System.ComponentModel.DataAnnotations;
public record CreateProductRequest(
    [property: Required, StringLength(200)] string Name,
    [property: Required] string Description,
    [property: Range(0.01, 1000000)] decimal Price,
    [property: Range(0, int.MaxValue)] int Stock,
    string? Category
);