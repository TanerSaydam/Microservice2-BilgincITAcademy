namespace Microservice.CategoryWebAPI.Models;

public sealed class Category
{
    public Category()
    {
        Id = Guid.CreateVersion7(); // .NET 9 
        //Id = Guid.NewGuid(); // .NET 8 ve eskisi
    }
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}