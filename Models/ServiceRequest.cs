namespace PROG7311Part2.Models
{
    public class ServiceRequest
    {
        public int ServiceRequestId { get; set; }

        public string Description { get; set; }
        public decimal Cost { get; set; }
        public string? Status { get; set; }

        // Foreign Key
        public int ContractId { get; set; }
        public Contract? Contract { get; set; }

        public string? PDFAttachmentFilePath { get; set; }
    }
}
