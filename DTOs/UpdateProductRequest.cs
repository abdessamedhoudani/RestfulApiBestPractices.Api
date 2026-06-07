using System.ComponentModel.DataAnnotations;
public record UpdateProductRequest(
    [property: Required, StringLength(200)] string Name,
    [property: Required] string Description,
    [property: Range(0.01, 1000000)] decimal Price,
    [property: Range(0, int.MaxValue)] int Stock,
    string? Category
);

public record PatchProductRequest(
    string? Name,
    string? Description,
    decimal? Price,
    int? Stock,
    string? Category
);