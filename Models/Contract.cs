namespace PROG7311Part2.Models
{
    public class Contract
    {
        public int ContractId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string ServiceLevel { get; set; }

        public string? PDFAgreementFilePath { get; set; }

        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public ICollection<ServiceRequest>? ServiceRequests { get; set; }
    }
}
