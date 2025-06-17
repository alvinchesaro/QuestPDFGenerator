namespace QuestPDFGenerator.Model.ViewModels
{
    public class BadgeDocumentViewModel
    {
        public string Name { get; set; }
        public string Organization { get; set; }
        public string OrganizationAddress { get; set; }
        public string AttendanceCode { get; set; }
        public string Role { get; set; } // "Attendee", "Exhibitor", "Innovator", "Sponsor"
        public string AttendanceUrl { get; set; }
        public string ConceptNoteUrl { get; set; }
        public string VenueDirectionsUrl { get; set; }
        public string PartnersPath { get; set; }
        public string ExhibitorHandbookUrl { get; set; }
        public string FloorPlanUrl { get; set; }
        public string LogoPath { get; set; } // Path to the logo image
    }
}
