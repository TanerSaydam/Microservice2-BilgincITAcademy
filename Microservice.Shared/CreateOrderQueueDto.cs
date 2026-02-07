namespace Microservice.Shared;

public sealed record OrderCreateQueueDto(
    Guid OrderId,
    Guid ProductId,
    int Quantity);

public sealed record ProductResultQueueDto(
    Guid OrderId,
    bool Result);