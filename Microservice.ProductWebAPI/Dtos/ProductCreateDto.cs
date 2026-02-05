namespace Microservice.ProductWebAPI.Dtos;

public sealed record ProductCreateDto(
    string Name,
    Guid CategoryId);