namespace QuestPDFGenerator.Model.ViewModels
{
    public class InvoiceDocumentViewModel
    {
        public string HeaderImageFile { get; set; }

        public string InvoiceNumber { get; set; }

        public string InvoiceDate { get; set; }

        public string InvoiceTo { get; set; }

        public string Organization { get; set; }

        public string OrganizationAddress { get; set; }

        public string OrganizationPostalAddress { get; set; }

        public string ContactImageFile { get; set; }

        public string Status { get; set; }

        public string ExhibitionPackageTitle { get; set; }

        public string BoothSize { get; set; }

        public string Currency { get; set; }

        public string Cost { get; set; }

        public string ServiceDescriptionImageFile { get; set; }

        public string Discount { get; set; }

        public string ToTalDue { get; set; }

        public string FooterImageFile { get; set; }
    }
}
