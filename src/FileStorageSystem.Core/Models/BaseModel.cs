namespace FileStorageSystem.Core.Models;

public class BaseModel
{
    public Guid Id { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdateDate { get; set; }
}