var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// a post endpoint that accepts a file metadata (name, size,total chunks) and returns a unique upload id

app.MapPost("/upload/init", (FileMetadata metadata)=> {
    var uploadId = Guid.NewGuid();

    return Results.Ok(new { uploadId });
} );

app.MapPost("/upload/chunk", (ChunkMetadata metadata) => {
    return Results.Ok(new { message = "Chunk received" });
});

app.MapPost("/upload/finalize", (FinalizeMetadata metadata) => {
    return Results.Ok(new { message = "Upload finalized" });
});

app.MapGet("/upload/missing-chunks/{uploadId}", (Guid uploadId ) => {
    return Results.Ok(new { missingChunks = new List<missingChunks>() });
});