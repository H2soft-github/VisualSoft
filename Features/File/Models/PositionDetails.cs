namespace VisualSoft.Features.File.Models
{
    public class PositionDetails
    {
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public decimal Quantity { get; set; }
        public decimal NetPrice { get; set; }
        public decimal NetValue { get; set; }
        public decimal VatValue { get; set; }
        public decimal QuantityBefore { get; set; }
        public decimal AvgBefore { get; set; }
        public decimal QuantityAfter { get; set; }
        public decimal AvgAfter { get; set; }
        public string Group { get; set; } = null!;
    }
}
