using iText.Kernel.Pdf;
using iText.Layout;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;



namespace AppliedAccounts.Pages.Testing
{
    public partial class CreatePDFinMemory_and_Download
    {
        public byte[] fileData { get; set; } = [];  

        public void DownloadPdf()
        {
            try
            {
                // Generate your file (for example, a PDF) in a MemoryStream
                var memoryStream = new MemoryStream();

                // You would write your file data to the memory stream here.
                // For example, let's assume you're generating a PDF with some data.
                //var pdfContent = "This is a sample PDF file content."; // Replace with actual PDF generation logic
                //var writer = new StreamWriter(memoryStream);
                //writer.Write(pdfContent);
                //writer.Flush();
                //memoryStream.Position = 0; // Reset the position of the memory stream

                // Convert the MemoryStream to a byte array
                //fileData = memoryStream.ToArray();

                fileData = GetPDFBytes();

                // Use JS interop to trigger the file download in the browser
                //await js.InvokeVoidAsync("downloadPDF", "invoice.pdf", fileData);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public byte[] GetPDFBytes()
        {
            using (var memoryStream = new MemoryStream())
            {
                // Create PdfWriter and PdfDocument
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Add content to the PDF
                document.Add(new iText.Layout.Element.Paragraph("Hello, this is a test PDF file using iText 7!"));

                document.Close();  // Close the document

                byte[] pdfBytes = memoryStream.ToArray();  // Get the PDF byte array
                return pdfBytes;
                //return File(pdfBytes, "application/pdf", "GeneratedFile.pdf");  // Return the file to the client
            }
        }

    }
}

[Route("api/pdf")]
[ApiController]
public class PdfController : ControllerBase
{
    [HttpGet("generate")]
    public IActionResult GeneratePdf()
    {
        using (var memoryStream = new MemoryStream())
        {
            // Create PdfWriter and PdfDocument
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Add content to the PDF
            document.Add(new iText.Layout.Element.Paragraph("Hello, this is a test PDF file using iText 7!"));

            document.Close();  // Close the document

            byte[] pdfBytes = memoryStream.ToArray();  // Get the PDF byte array
            return File(pdfBytes, "application/pdf", "GeneratedFile.pdf");  // Return the file to the client
        }
    }
}
