namespace APIBlob.Model
{
    public class FileRecord
    {
        public string FileName { get; set; } = string.Empty;
        public string BlobUri { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}
