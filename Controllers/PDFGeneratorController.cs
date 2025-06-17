using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

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
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Content().Text("Hello, QuestPDF!");
                });
            });
        }
    }
}
