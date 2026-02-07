namespace Microservice.OrderWebAPI.Dtos;

public sealed record OrderCreateDto(
    Guid ProductId,
    int Quantity);
