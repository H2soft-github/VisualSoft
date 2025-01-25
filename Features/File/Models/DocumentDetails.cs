using Microsoft.Extensions.Logging.Abstractions;

namespace VisualSoft.Features.File.Models
{
    public class DocumentDetails
    {
        public string CodeBA { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string DocumentNumber { get; set; } = null!;
        public DateTime OperationData { get; set; }
        public int DocumentDayNumber {get; set; }
        public string ContractCode { get; set; } = null!;
        public string ContractName { get; set; } = null!;
        public string ExternalDocumentNumber { get; set; } = null!;
        public DateTime ExternalDocumentDate { get; set; }
        public decimal Net { get; set; }
        public decimal Vat { get; set; }
        public decimal Brutto { get; set; }
        public decimal F1 { get; set; }
        public decimal F2 { get; set; }
        public decimal F3 { get; set; }
        public CommentDetails CommentDetails { get; set; } = null!;
        public IEnumerable<PositionDetails> PositionDetails { get; set; } = new List<PositionDetails>();
    }
}
