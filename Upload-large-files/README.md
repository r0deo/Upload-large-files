# Upload Large Files API

This project is an ASP.NET Core minimal API for uploading large files using chunked uploads.

## Purpose

- Support large file uploads by splitting files into smaller chunks
- Allow resumable and reliable uploads
- Track upload progress and missing chunks
- Finalize uploads after all chunks are received

## Endpoints

- `POST /upload/init` - start a new upload session
- `POST /upload/chunk` - upload a chunk for an existing session
- `POST /upload/finalize` - complete the upload and save the file
- `GET /upload/missing-chunks/{uploadId}` - get missing chunk indexes

## Implementation

- `UploadService` manages upload state and chunk storage
- `Api.cs` defines the minimal API endpoints and request handling
- Uses multipart form data for chunk uploads

## Notes

- Validate `uploadId` and `chunkIndex` on chunk upload
- Handle invalid requests with appropriate HTTP responses
- Save final file on upload completion