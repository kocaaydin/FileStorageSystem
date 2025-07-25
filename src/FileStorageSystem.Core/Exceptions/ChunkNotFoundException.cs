namespace FileStorageSystem.Core.Exceptions;

public class ChunkNotFoundException: Exception
{
    public ChunkNotFoundException(string message) : base(message)
    {
    }

    public ChunkNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}