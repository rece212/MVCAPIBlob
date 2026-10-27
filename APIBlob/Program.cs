using APIBlob.Model;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
namespace APIBlob
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            #region swagger
            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddSwaggerGen();//added this
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (11==11)
            {//added bellow
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Blob API v1");
                    c.RoutePrefix = string.Empty;
                });
            }
            #endregion
            app.UseHttpsRedirection();
            app.UseAuthorization();

            var connectionString = app.Configuration.GetConnectionString("AzureBlobStorage")!;
            var containerName = app.Configuration["BlobContainerName"];

            var options = new BlobClientOptions(BlobClientOptions.ServiceVersion.V2023_11_03);
            var blobServiceClient = new BlobServiceClient(connectionString, options);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            // GET: /api/files
            app.MapGet("/api/files", async () =>
            {
                var records = new List<FileRecord>();

                await foreach (var blob in containerClient.GetBlobsAsync())
                {
                    var client = containerClient.GetBlobClient(blob.Name);
                    records.Add(new FileRecord
                    {
                        FileName = blob.Name,
                        BlobUri = client.Uri.ToString(),
                        UploadedAt = blob.Properties.CreatedOn?.UtcDateTime ?? DateTime.UtcNow
                    });
                }

                return Results.Ok(records);
            })
            .WithName("GetStoredFiles")
            .WithSummary("Retrieves all file records from Azurite");
            // POST: /api/files/upload
            app.MapPost("/api/files/upload", async (IFormFile? file) =>
            {
                if (file == null || file.Length == 0)
                {
                    return Results.BadRequest("No file uploaded.");
                }
                var GUID = Guid.NewGuid();

                var safeBlobName = $"{DateTime.UtcNow:yyyyMMddHHmmss}-{GUID.ToString()}-{file.FileName}";

                var blobClient = containerClient.GetBlobClient(safeBlobName);
                
                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, overwrite: true);

                var record = new FileRecord
                {
                    FileName = safeBlobName,
                    BlobUri = blobClient.Uri.ToString(),
                    UploadedAt = DateTime.UtcNow
                };

                return Results.Ok(record);
            })
            .Accepts<IFormFile>("multipart/form-data")
            .DisableAntiforgery()
            .WithName("UploadFile")
            .WithSummary("Uploads a single file to Azurite and returns its URI");

            await app.RunAsync();
        }
    }
}
