using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using TimeSheet.Models.Dtos;
using Border = iText.Layout.Borders.Border;
using Cell = iText.Layout.Element.Cell;
using TextAlignment = iText.Layout.Properties.TextAlignment;
using VerticalAlignment = iText.Layout.Properties.VerticalAlignment;

namespace TimeSheet.Documents.Pdf;

public class TextPdfInvoice(
    InvoiceDto invoiceDto,
    ClientDto clientDto,
    IEnumerable<ProjectDto> projectDtos,
    IEnumerable<TimesheetDto> timesheetDtos) {

    private decimal _grandTotal;
    private readonly PdfFont _regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
    private readonly PdfFont _boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

    public async Task GeneratePdfAsync(string outputPath) {
        await using var writer = new PdfWriter(outputPath);
        using var pdf = new PdfDocument(writer);
        using var document = new Document(pdf, PageSize.A4);

        document.SetMargins(50, 50, 50, 50);

        ComposeHeader(document);
        ComposeContent(document);
        ComposeFooter(document, pdf);
    }

    private void ComposeHeader(Document document) {
        var headerTable = new Table(UnitValue.CreatePercentArray([70f, 30f]))
            .UseAllAvailableWidth();

        var leftCell = new Cell().SetBorder(Border.NO_BORDER).SetPaddingBottom(10);
        var invoiceTitle = new Paragraph($"Invoice #{invoiceDto.Number}")
            .SetFont(_boldFont)
            .SetFontSize(20)
            .SetFontColor(ColorConstants.BLUE)
            .SetMarginBottom(5);
        leftCell.Add(invoiceTitle);

        var issueDate = new Paragraph()
            .Add(new Text("Issue date: ").SetFont(_boldFont))
            .Add(new Text($"{invoiceDto.IssueDate:d}").SetFont(_regularFont))
            .SetMarginBottom(2);
        leftCell.Add(issueDate);

        var dueDate = new Paragraph()
            .Add(new Text("Due date: ").SetFont(_boldFont))
            .Add(new Text($"{invoiceDto.DueDate:d}").SetFont(_regularFont));
        leftCell.Add(dueDate);

        // Logo placeholder
        var rightCell = new Cell()
            .SetBorder(Border.NO_BORDER)
            .SetHeight(50)
            .SetBackgroundColor(ColorConstants.LIGHT_GRAY);

        headerTable.AddCell(leftCell);
        headerTable.AddCell(rightCell);

        document.Add(headerTable);
    }

    private void ComposeContent(Document document) {
        document.Add(new Paragraph().SetMarginBottom(20));

        var addressTable = new Table(UnitValue.CreatePercentArray([45f, 10f, 45f]))
            .UseAllAvailableWidth()
            .SetMarginBottom(20);
        var fromCell = new Cell().SetBorder(Border.NO_BORDER);

        fromCell.Add(new Paragraph("From")
            .SetFont(_boldFont)
            .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1))
            .SetPaddingBottom(5)
            .SetMarginBottom(5));
        fromCell.Add(new Paragraph("MyName").SetFont(_regularFont).SetMarginBottom(2));
        fromCell.Add(new Paragraph("MyPhone").SetFont(_regularFont).SetMarginBottom(2));
        fromCell.Add(new Paragraph("MyEmail").SetFont(_regularFont));

        var spacerCell = new Cell().SetBorder(Border.NO_BORDER);
        var forCell = new Cell().SetBorder(Border.NO_BORDER);

        forCell.Add(new Paragraph("For")
            .SetFont(_boldFont)
            .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1))
            .SetPaddingBottom(5)
            .SetMarginBottom(5));
        forCell.Add(new Paragraph(clientDto.Name).SetFont(_regularFont).SetMarginBottom(2));
        forCell.Add(new Paragraph(clientDto.ContactName).SetFont(_regularFont).SetMarginBottom(2));
        forCell.Add(new Paragraph(clientDto.ContactPhone).SetFont(_regularFont).SetMarginBottom(2));
        forCell.Add(new Paragraph(clientDto.ContactEmail).SetFont(_regularFont));


        addressTable.AddCell(fromCell);
        addressTable.AddCell(spacerCell);
        addressTable.AddCell(forCell);

        document.Add(addressTable);

        ComposeTable(document);

        var grandTotalParagraph = new Paragraph($"Grand total: {_grandTotal:C}")
            .SetFont(_boldFont)
            .SetTextAlignment(TextAlignment.RIGHT)
            .SetMarginTop(10)
            .SetMarginRight(5);

        document.Add(grandTotalParagraph);

        if (!string.IsNullOrWhiteSpace(invoiceDto.Comments)) {
            ComposeComments(document);
        }
    }

    private void ComposeTable(Document document) {
        foreach (var projectDto in projectDtos) {
            var table = new Table(UnitValue.CreatePercentArray([5f, 30f, 15f, 15f, 15f, 20f]))
                .UseAllAvailableWidth()
                .SetMarginBottom(15);

            var headerCell = new Cell(1, 6)
                .Add(new Paragraph(projectDto.Name).SetFont(_boldFont))
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetBorder(Border.NO_BORDER)
                .SetPaddingTop(5)
                .SetPaddingBottom(5);
            table.AddHeaderCell(headerCell);

            table.AddHeaderCell(CreateHeaderCell("#"));
            table.AddHeaderCell(CreateHeaderCell("Description"));
            table.AddHeaderCell(CreateHeaderCell("Date", TextAlignment.RIGHT));
            table.AddHeaderCell(CreateHeaderCell("Wage", TextAlignment.RIGHT));
            table.AddHeaderCell(CreateHeaderCell("Hours", TextAlignment.RIGHT));
            table.AddHeaderCell(CreateHeaderCell("Total", TextAlignment.RIGHT));

            var separatorCell = new Cell(1, 6)
                .SetBorder(Border.NO_BORDER)
                .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1))
                .SetHeight(5);
            table.AddCell(separatorCell);

            var projectTimesheetDtos = timesheetDtos.Where(t => t.ProjectId == projectDto.Id).ToList();
            if (projectTimesheetDtos.Count == 0) {
                continue;
            }

            for (var i = 0; i < projectTimesheetDtos.Count; i++) {
                var workTime = projectTimesheetDtos[i].EndTime - projectTimesheetDtos[i].StartTime;
                var total = (decimal)workTime.TotalHours * projectDto.HourlyWage;

                _grandTotal += total;

                table.AddCell(CreateDataCell(i.ToString()));
                table.AddCell(CreateDataCell(""));
                table.AddCell(CreateDataCell($"{projectTimesheetDtos[i].Date:d}", TextAlignment.RIGHT));
                table.AddCell(CreateDataCell($"{projectDto.HourlyWage:C}", TextAlignment.RIGHT));
                table.AddCell(CreateDataCell($"{workTime.TotalHours:F2}", TextAlignment.RIGHT));
                table.AddCell(CreateDataCell($"{total:C}", TextAlignment.RIGHT));
            }

            document.Add(table);
        }
    }

    private Cell CreateHeaderCell(string text, TextAlignment alignment = TextAlignment.LEFT) {
        return new Cell()
            .Add(new Paragraph(text).SetFont(_boldFont).SetTextAlignment(alignment))
            .SetBackgroundColor(ColorConstants.WHITE)
            .SetBorder(Border.NO_BORDER)
            .SetPaddingTop(5)
            .SetPaddingBottom(5);
    }

    private Cell CreateDataCell(string text, TextAlignment alignment = TextAlignment.LEFT) {
        var lightGrey = new DeviceRgb(230, 230, 230);
        return new Cell()
            .Add(new Paragraph(text).SetFont(_regularFont).SetTextAlignment(alignment))
            .SetBorder(Border.NO_BORDER)
            .SetBorderBottom(new SolidBorder(lightGrey, 1))
            .SetPaddingTop(5)
            .SetPaddingBottom(5);
    }

    private void ComposeComments(Document document) {
        var lightGrey = new DeviceRgb(240, 240, 240);

        var commentsDiv = new Div()
            .SetBackgroundColor(lightGrey)
            .SetPadding(10)
            .SetMarginTop(25);

        commentsDiv.Add(new Paragraph("Comments")
            .SetFont(_boldFont)
            .SetFontSize(14)
            .SetMarginBottom(5));

        commentsDiv.Add(new Paragraph(invoiceDto.Comments)
            .SetFont(_regularFont));

        document.Add(commentsDiv);
    }

    private void ComposeFooter(Document document, PdfDocument pdf) {
        var numberOfPages = pdf.GetNumberOfPages();
        for (var i = 1; i <= numberOfPages; i++) {
            var pageSize = pdf.GetPage(i).GetPageSize();
            var footerText = $"{i} / {numberOfPages}";

            document.ShowTextAligned(
                new Paragraph(footerText).SetFont(_regularFont),
                pageSize.GetWidth() / 2,
                30,
                i,
                TextAlignment.CENTER,
                VerticalAlignment.BOTTOM,
                0
            );
        }
    }
}