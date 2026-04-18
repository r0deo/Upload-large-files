namespace MyApp.Abstractions;

public interface IChunkMetadata
{
    Guid UploadId { get; set; }
    int ChunkIndex { get; set; }
    long ChunkSize { get; set; }
}

public interface IFinalizeMetadata
{
    Guid UploadId { get; set; }
    string FileName { get; set; }
    long FileSize { get; set; }
    int TotalChunks { get; set; }
}

public interface IFileMetadata
{
    string FileName { get; set; }
    long FileSize { get; set; }
}

public interface IMissingChunks
{
    int ChunkIndex { get; set; }
    long ChunkSize { get; set; }
}

public class FileMetadata : IFileMetadata
{
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int TotalChunks { get; set; }
}

public class ChunkMetadata : IChunkMetadata
{
    public Guid UploadId { get; set; }
    public int ChunkIndex { get; set; }
    public long ChunkSize { get; set; }
}

public class FinalizeMetadata : IFinalizeMetadata
{
    public Guid UploadId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int TotalChunks { get; set; }
}

public class MissingChunks : IMissingChunks
{
    public int ChunkIndex { get; set; }
    public long ChunkSize { get; set; }
}