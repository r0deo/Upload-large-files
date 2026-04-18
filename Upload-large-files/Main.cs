 
using MyApp.Abstractions;

namespace UploadLargeFiles;

public class UploadService
{
    private readonly Dictionary<Guid, FileMetadata> _uploadMetadata = new();
    private readonly Dictionary<Guid, Dictionary<int, byte[]>> _chunkStore = new();

    public Guid InitializeUpload(FileMetadata metadata)
    {
        if (string.IsNullOrEmpty(metadata.FileName) || metadata.FileSize <= 0 || metadata.TotalChunks <= 0)
        {
            throw new ArgumentException("Invalid file metadata");
        }

        var uploadId = Guid.NewGuid();
        _uploadMetadata[uploadId] = metadata;
        _chunkStore[uploadId] = new Dictionary<int, byte[]>();
        return uploadId;
    }

    public void UploadChunk(Guid uploadId, int chunkIndex, byte[] chunkData)
    {
        EdgeCases(chunkIndex, uploadId);

        if (!_chunkStore.ContainsKey(uploadId))
        {
            throw new ArgumentException("Invalid upload ID");
        }

        HandleOutOfOrderChunks(uploadId, chunkIndex);

        _chunkStore[uploadId][chunkIndex] = chunkData;
    }

    public List<int> GetMissingChunks(Guid uploadId)
    {
        if (!_uploadMetadata.ContainsKey(uploadId))
        {
            throw new ArgumentException("Invalid upload ID");
        }

        var totalChunks = _uploadMetadata[uploadId].TotalChunks;
        var receivedChunks = _chunkStore[uploadId].Keys.ToHashSet();
        var missing = new List<int>();

        for (int i = 0; i < totalChunks; i++)
        {
            if (!receivedChunks.Contains(i))
            {
                missing.Add(i);
            }
        }

        return missing;
    }

    public void FinalizeUpload(Guid uploadId)
    {
        if (!_uploadMetadata.ContainsKey(uploadId))
        {
            throw new ArgumentException("Invalid upload ID");
        }

        var metadata = _uploadMetadata[uploadId];
        var missing = GetMissingChunks(uploadId);
        if (missing.Any())
        {
            throw new InvalidOperationException($"Upload incomplete. Missing chunks: {string.Join(", ", missing)}");
        }

        // Assemble file
        using var ms = new MemoryStream();
        for (int i = 0; i < metadata.TotalChunks; i++)
        {
            ms.Write(_chunkStore[uploadId][i]);
        }

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), metadata.FileName);
        File.WriteAllBytes(filePath, ms.ToArray());

        // Cleanup
        _uploadMetadata.Remove(uploadId);
        _chunkStore.Remove(uploadId);
    }

    public int GetProgress(Guid uploadId)
    {
        if (!_uploadMetadata.ContainsKey(uploadId))
        {
            return 0;
        }

        var totalChunks = _uploadMetadata[uploadId].TotalChunks;
        var receivedChunks = _chunkStore[uploadId].Count;
        return (int)((double)receivedChunks / totalChunks * 100);
    }

    private void EdgeCases(int chunkIndex, Guid uploadId)
    {
        if (chunkIndex < 0)
        {
            throw new ArgumentException("Chunk index must be non-negative");
        }
        if (uploadId == Guid.Empty)
        {
            throw new ArgumentException("Upload ID must be a valid GUID");
        }
    }

    private void HandleOutOfOrderChunks(Guid uploadId, int chunkIndex)
    {
        if (_chunkStore[uploadId].ContainsKey(chunkIndex))
        {
            throw new InvalidOperationException("Chunk index already received for this upload ID");
        }
    }
}

