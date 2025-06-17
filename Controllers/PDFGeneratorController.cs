using Microsoft.AspNetCore.Mvc;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDFGenerator.Model.Helpers;
using QuestPDFGenerator.Model.ViewModels;

namespace QuestPDFGenerator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PDFGeneratorController : ControllerBase
    {
        [HttpGet]
        public IActionResult GeneratePDF()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var document = CreateDocument();
            var pdfBytes = document.GeneratePdf();
            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                return NotFound("PDF generation failed or returned empty content.");
            }
            return File(pdfBytes, "application/pdf", "generated");
        }

        private static IDocument CreateDocument()
        {
            var fontDir = Path.Combine(AppContext.BaseDirectory, "Resources", "Fonts");
            foreach (var fontPath in Directory.GetFiles(fontDir, "*.ttf"))
            {
                using var stream = System.IO.File.OpenRead(fontPath);
                FontManager.RegisterFont(stream);
            }
            var logoPath = Path.Combine(AppContext.BaseDirectory, "Resources", "logo.png");
            var partnersPath = Path.Combine(AppContext.BaseDirectory, "Resources", "partners.png");
            var model = new BadgeDocumentViewModel
            {
                Name = "Jane Doe",
                Organization = "Kenya Sugar Board",
                AttendanceCode = "#TUSONGE25-XYZ1",
                Role = "Judge",
                LogoPath = logoPath,
                AttendanceUrl = "https://sugarinnovation.go.ke/attendance?code=SIISTUSONGE25-XYZ1",
                ConceptNoteUrl = "https://sugarinnovation.go.ke/resources/Call-for-Papers-2024.pdf",
                VenueDirectionsUrl = "https://maps.google.com/?q=Grand+Royal+Swiss+Hotel,+Kisumu",
                PartnersPath = partnersPath,
                ExhibitorHandbookUrl = "https://sugarinnovation.go.ke/resources/Exhibitor-Handbook.pdf",
                FloorPlanUrl = "https://sugarinnovation.go.ke/resources/Exhibition-Floor-Plan.pdf"
            };

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(16));

                    page.Content().Layers(layers =>
                    {
                        // Layer for fold guides
                        layers.Layer().Element(container => container
                                        .Svg(size => $@"
                                                <svg width='{size.Width}' height='{size.Height}' xmlns='http://www.w3.org/2000/svg'>
                                                    <line x1='0' y1='{size.Height / 2}' x2='{size.Width}' y2='{size.Height / 2}'
                                                        stroke='gray' stroke-width='4' stroke-dasharray='18,10'/>
                                                    <line x1='{size.Width / 2}' y1='0' x2='{size.Width / 2}' y2='{size.Height}'
                                                        stroke='gray' stroke-width='4' stroke-dasharray='18,10'/>
                                                </svg>
                                                ")
                                            );

                        // Primary content layer
                        layers.PrimaryLayer().Column(stack =>
                        {
                            stack.Item().Column(col =>
                            {
                                col.Item().Row(row =>
                                {
                                    row.RelativeItem().Element(q => Quarter1(q, model));
                                    row.RelativeItem().Element(q => Quarter2(q, model));
                                });
                                col.Item().Row(row =>
                                {
                                    row.RelativeItem().Element(q => Quarter3(q, model));
                                    row.RelativeItem().Element(q => Quarter4(q, model));
                                });
                            });
                        });
                    });
                });
            });
        }

        // Quarter 1: Logo, Name, Organization, Attendance Code, QR Code, Role
        private static void Quarter1(IContainer container, BadgeDocumentViewModel model)
        {
            container.Padding(5).Border(0).Column(col =>
            {
                if (!string.IsNullOrWhiteSpace(model.LogoPath))
                {
                    col.Item().AlignCenter().ScaleToFit().Image(model.LogoPath);
                }

                col.Item().Text(model.Name).AlignCenter().ExtraBlack().FontSize(18).FontColor("#333").FontFamily("Poppins");
                col.Item().PaddingBottom(5).Text(model.Organization).FontSize(16).AlignCenter().Italic().FontFamily("Poppins");
                col.Item().PaddingBottom(5).Text($"Attendance Code: {model.AttendanceCode}").FontSize(12).SemiBold().AlignCenter().FontFamily("Poppins");

                var qrCode = QRHelper.GenerateQRCode(model.AttendanceUrl);
                col.Item().AlignCenter().Width(200).Image(qrCode);

                col.Item().Padding(5).Border(1).BorderColor("#3B9E55").Background("#3B9E55").AlignCenter()
                    .Text(model.Role).Bold().FontColor("#FFFFFF").FontSize(26).FontFamily("Poppins");
            });
        }

        // Quarter 2: Concept Note QR
        private static void Quarter2(IContainer container, BadgeDocumentViewModel model)
        {
            container.Padding(10).Border(0).Column(col =>
            {
                col.Item().PaddingTop(30).Text("Scan to Download the Concept Note").AlignCenter().FontSize(15).Black().FontFamily("Poppins");

                var qrCode = QRHelper.GenerateQRCode(model.ConceptNoteUrl);
                col.Item().PaddingTop(40).AlignCenter().Width(200).Image(qrCode);

                col.Item().PaddingTop(8).Text("See you @ the Grand Royal Swiss Hotel - Kisumu").AlignEnd().FontSize(10).FontFamily("Poppins");
                col.Item().PaddingTop(2).Text("2nd - 4th December 2025").AlignEnd().FontSize(12).SemiBold().FontFamily("Poppins");
            });
        }

        // Quarter 3: Venue Directions QR
        private static void Quarter3(IContainer container, BadgeDocumentViewModel model)
        {
            container.Padding(1).Border(0).Column(col =>
            {

                col.Item().AlignCenter().ScaleToFit().Image(model.PartnersPath);
            });
        }

        // Quarter 4: Exhibitor Handbook and Floor Plan QRs
        private static void Quarter4(IContainer container, BadgeDocumentViewModel model)
        {
            container.Padding(10).Border(0).Column(col =>
            {
                col.Item().PaddingTop(30).Text("Scan for the Exhibitor Handbook").AlignCenter().FontSize(12).Black().FontFamily("Poppins");

                var handbookQr = QRHelper.GenerateQRCode(model.ExhibitorHandbookUrl);
                col.Item().AlignCenter().Width(130).Image(handbookQr);

                col.Item().PaddingTop(5);
                col.Item().Text("Scan for the Floor Plan").FontSize(12).AlignCenter().Black().FontFamily("Poppins");

                var floorPlanQr = QRHelper.GenerateQRCode(model.FloorPlanUrl);
                col.Item().AlignCenter().Width(130).Image(floorPlanQr);
                col.Item().PaddingTop(5).PaddingLeft(40).Text("📞 :   +254 703 802247").FontSize(11).AlignLeft().SemiBold().FontColor("#3B9E55").FontFamily("Poppins");
                col.Item().PaddingTop(5).PaddingLeft(40).Text("📧 :   info@sugarinnovation.go.ke").FontSize(11).AlignLeft().SemiBold().FontColor("#3B9E55").FontFamily("Poppins");
            });
        }
    }
}
