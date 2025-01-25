namespace VisualSoft.Features.File.Models
{
    public class FileResult
    {
        public IEnumerable<DocumentDetails>? Documents { get; set; } = new List<DocumentDetails>();
        public int LineCount { get; set; }
        public int CharCount { get; set; }
        public int Sum { get; set; }
        public int Xcount { get; set; }
        public string ProductsWithMaxNetValue { get; set; } = null!;
    }
}
