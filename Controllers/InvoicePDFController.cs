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
    public class InvoicePDFController : ControllerBase
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

            // Register Regular
            using (var regular = System.IO.File.OpenRead(Path.Combine(fontDir, "Poppins-Regular.ttf")))
            {
                FontManager.RegisterFont(regular);
            }

            using (var bold = System.IO.File.OpenRead(Path.Combine(fontDir, "Poppins-Bold.ttf")))
            {
                FontManager.RegisterFont(bold);
            }

            using (var extraBold = System.IO.File.OpenRead(Path.Combine(fontDir, "Poppins-ExtraBold.ttf")))
            {
                FontManager.RegisterFont(extraBold);
            }

            using (var black = System.IO.File.OpenRead(Path.Combine(fontDir, "Poppins-Black.ttf")))
            {
                FontManager.RegisterFont(black);
            }

            using (var semiBold = System.IO.File.OpenRead(Path.Combine(fontDir, "Poppins-SemiBold.ttf")))
            {
                FontManager.RegisterFont(semiBold);
            }

            var headerPath = Path.Combine(AppContext.BaseDirectory, "Resources", "invoice_header.png");
            var contactPath = Path.Combine(AppContext.BaseDirectory, "Resources", "invoice_contact.png");
            var serviceDescriptionPath = Path.Combine(AppContext.BaseDirectory, "Resources", "service_description.png");
            var footerPath = Path.Combine(AppContext.BaseDirectory, "Resources", "invoice_footer.png");
            var model = new InvoiceDocumentViewModel
            {
                HeaderImageFile = headerPath,
                ContactImageFile = contactPath,
                InvoiceDate = "May 20, 2025",
                InvoiceNumber = "1305106",
                Organization = "ICEA Lion Life Assurance Company Limited",
                OrganizationAddress = "ICEA Lion Centre, Riverside Park, Westlands",
                OrganizationPostalAddress = "46143 - 00100 Nairobi",
                ExhibitionPackageTitle = "Elevated Exhibition Package",
                BoothSize = "19 Metre`",
                Currency = "KES. ",
                Cost = "123,200.00",
                ServiceDescriptionImageFile = serviceDescriptionPath,
                FooterImageFile = footerPath
            };

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(0);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(16));

                    page.Content().Layers(layers =>
                    {
                        layers.PrimaryLayer().Column(stack =>
                        {
                            stack.Item().Column(col =>
                            {
                                col.Item().Row(row =>
                                {
                                    row.RelativeItem().Element(q => PDFInvoiceBody(q, model));
                                   
                                });
                                
                            });
                        });
                    });
                });
            });
        }

        // Quarter 1: Logo, Name, Organization, Attendance Code, QR Code, Role
        private static void PDFInvoiceBody(IContainer container, InvoiceDocumentViewModel model)
        {
            container.Padding(0).Border(0).Column(col =>
            {               
                col.Item().AlignCenter().Image(model.HeaderImageFile);
                col.Item().PaddingTop(-95).PaddingLeft(42).Text($"{model.InvoiceDate}").AlignCenter().FontSize(12).FontColor("#FFFFFF").FontFamily("Poppins");

                col.Item().PaddingTop(1).PaddingLeft(42).Text($"Invoice # {model.InvoiceNumber}").AlignCenter().FontSize(12).FontColor("#FFFFFF").FontFamily("Poppins");
                col.Item().PaddingTop(60).PaddingLeft(30).Text("INVOICE TO:").AlignLeft().FontSize(22).Black().FontFamily("Poppins").FontColor("#3b9E55");
                col.Item().PaddingTop(10).PaddingLeft(30).Text($"{model.Organization}").AlignLeft().FontSize(12).SemiBold().FontFamily("Poppins");
                col.Item().PaddingTop(5).PaddingLeft(30).Text($"{model.OrganizationAddress}").AlignLeft().FontSize(12).SemiBold().FontFamily("Poppins");
                col.Item().PaddingTop(5).PaddingLeft(30).Text($"P.O. BOX {model.OrganizationPostalAddress}").AlignLeft().FontSize(12).SemiBold().FontFamily("Poppins");
                col.Item().PaddingTop(-60).AlignRight().ScaleToFit().Width(240).PaddingTop(-30).AlignRight().Image(model.ContactImageFile);

                col.Item().PaddingTop(1).PaddingRight(170).Text($"Status :").AlignRight().FontSize(15).SemiBold().FontColor("#000000").FontFamily("Poppins");
                col.Item().PaddingTop(-22).PaddingRight(30).Text($"Pending Payment").AlignRight().FontSize(15).FontColor("#FF0000").FontFamily("Poppins");

                col.Item().AlignRight().ScaleToFit().Image(model.ServiceDescriptionImageFile);
                col.Item().PaddingTop(-216).PaddingLeft(36).Text($"{model.ExhibitionPackageTitle}").AlignLeft().FontSize(15).Bold().FontFamily("Poppins");
                col.Item().PaddingTop(-4).PaddingLeft(32).Text($"{model.BoothSize} Exhibition Space at the Sugar Industry Innovation" +
                    $"\n Symposium and Expo 2025").AlignLeft().FontSize(12).FontFamily("Poppins");

                col.Item().PaddingTop(-32).PaddingRight(125).Text($"{model.Currency}").AlignRight().FontSize(14).FontFamily("Poppins");
                col.Item().PaddingTop(-21).PaddingRight(38).Text($"{model.Cost}").AlignRight().FontSize(14).FontFamily("Poppins");

                col.Item().PaddingTop(76).PaddingRight(125).Text($"{model.Currency}").AlignRight().FontSize(14).SemiBold().FontFamily("Poppins");
                col.Item().PaddingTop(-21).PaddingRight(38).Text($"{model.Cost}").AlignRight().FontSize(14).SemiBold().FontFamily("Poppins");

                col.Item().PaddingTop(17).PaddingRight(125).Text($"{model.Currency}").AlignRight().FontSize(14).Bold().FontColor("#FFFFFF").FontFamily("Poppins");
                col.Item().PaddingTop(-21).PaddingRight(38).Text($"{model.Cost}").AlignRight().FontSize(14).Bold().FontColor("#FFFFFF").FontFamily("Poppins");
                col.Item().AlignRight().PaddingTop(84).Image(model.FooterImageFile);
            });
        }
    }
}
