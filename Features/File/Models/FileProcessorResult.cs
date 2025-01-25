namespace VisualSoft.Features.File.Models
{
    public class FileProcessorResult
    {
        public ValidationEnum ValidationEnum { get; set; }
        public string? ValidationMessage { get; set; }
        public FileResult? FileResult { get; set; }
    }
}
