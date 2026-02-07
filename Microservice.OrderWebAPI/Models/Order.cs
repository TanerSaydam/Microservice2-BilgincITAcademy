namespace Microservice.OrderWebAPI.Models;

public sealed class Order
{
    public Order()
    {
        Id = Guid.CreateVersion7();
    }
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public Status Status { get; set; } = Status.Pending;
}

public enum Status
{
    Pending,
    Completed = 1,
    Fail = 2,
}