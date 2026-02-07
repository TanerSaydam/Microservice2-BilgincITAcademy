namespace Microservice.OrderWebAPI.Models;

public sealed class Idempotency
{
    public Idempotency()
    {
        Id = Guid.CreateVersion7();
        CreatedAt = DateTimeOffset.Now;
    }
    public Guid Id { get; set; }
    public int Key { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
