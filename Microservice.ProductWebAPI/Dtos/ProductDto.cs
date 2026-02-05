namespace Microservice.ProductWebAPI.Dtos;

public sealed record ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
}
