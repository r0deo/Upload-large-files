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