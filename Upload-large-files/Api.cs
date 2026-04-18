using MyApp.Abstractions;
using UploadLargeFiles;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<UploadService>();
var app = builder.Build();

app.MapPost("/upload/init", (FileMetadata metadata, UploadService service) =>
{
    try
    {
        var uploadId = service.InitializeUpload(metadata);
        return Results.Ok(new { uploadId });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPost("/upload/chunk", async (HttpContext context, UploadService service) =>
{
    try
    {
        var form = await context.Request.ReadFormAsync();
        var uploadIdStr = form["uploadId"].ToString();
        var chunkIndexStr = form["chunkIndex"].ToString();

        if (!Guid.TryParse(uploadIdStr, out var uploadId) || !int.TryParse(chunkIndexStr, out var chunkIndex))
        {
            return Results.BadRequest("Invalid uploadId or chunkIndex");
        }

        var file = form.Files["chunk"];
        if (file == null)
        {
            return Results.BadRequest("No chunk file provided");
        }

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var chunkData = ms.ToArray();

        service.UploadChunk(uploadId, chunkIndex, chunkData);
        var progress = service.GetProgress(uploadId);

        return Results.Ok(new { message = "Chunk received", progress });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(ex.Message);
    }
});

app.MapPost("/upload/finalize", (FinalizeMetadata metadata, UploadService service) =>
{
    try
    {
        service.FinalizeUpload(metadata.UploadId);
        return Results.Ok(new { message = "Upload finalized and file saved" });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (InvalidOperationException ex)
    {
        return Results.UnprocessableEntity(ex.Message);
    }
});

app.MapGet("/upload/missing-chunks/{uploadId}", (Guid uploadId, UploadService service) =>
{
    try
    {
        var missing = service.GetMissingChunks(uploadId);
        return Results.Ok(new { missingChunks = missing });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.Run();