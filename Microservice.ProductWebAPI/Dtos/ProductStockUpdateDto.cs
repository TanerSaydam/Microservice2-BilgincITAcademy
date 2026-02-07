namespace Microservice.ProductWebAPI.Dtos;

public sealed record ProductStockUpdateDto(
    Guid ProductId,
    int Quantity);