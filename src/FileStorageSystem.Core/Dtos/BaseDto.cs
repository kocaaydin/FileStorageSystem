namespace FileStorageSystem.Core.Dtos;

public class BaseDto
{
    public Guid Id { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdateDate { get; set; }
}
