using iText.IO.Font.Constants;
using iText.IO.Image;
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
using Image = iText.Layout.Element.Image;
using TextAlignment = iText.Layout.Properties.TextAlignment;
using VerticalAlignment = iText.Layout.Properties.VerticalAlignment;

namespace TimeSheet.Documents.Pdf;

public class TextPdfInvoice(InvoiceData data) {

    private decimal _grandTotal;
    private readonly PdfFont _regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
    private readonly PdfFont _boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

    private const int HeaderFontSize = 20;
    private const int CommentsFontSize = 14;
    private const int DefaultPadding = 5;
    private const int DefaultMargin = 10;
    private const int PageMargin = 50;
    private const int FooterYPosition = 30;
    private const int ProfileImageHeight = 50;

    private static readonly DeviceRgb LightGrey = new(230, 230, 230);
    private static readonly DeviceRgb VeryLightGrey = new(240, 240, 240);

    public async Task GeneratePdfAsync(string outputPath) {
        await using var writer = new PdfWriter(outputPath);
        using var pdf = new PdfDocument(writer);
        using var document = new Document(pdf, PageSize.A4);

        document.SetMargins(PageMargin, PageMargin, PageMargin, PageMargin);

        ComposeHeader(document);
        ComposeContent(document);
        ComposeFooter(document, pdf);
    }

    private void ComposeHeader(Document document) {
        var headerTable = new Table(UnitValue.CreatePercentArray([70f, 30f]))
            .UseAllAvailableWidth();
        var leftCell = CreateHeaderLeftCell();

        headerTable.AddCell(leftCell);

        if (data.Profile.Image is not null) {
            var rightCell = CreateHeaderRightCell();
            headerTable.AddCell(rightCell);
        }

        document.Add(headerTable);
    }

    private Cell CreateHeaderLeftCell() {
        var leftCell = new Cell()
            .SetBorder(Border.NO_BORDER)
            .SetPaddingBottom(DefaultPadding * 2);

        leftCell.Add(CreateInvoiceTitle());
        leftCell.Add(CreateDateParagraph("Issue date: ", data.Invoice.IssueDate, 2));
        leftCell.Add(CreateDateParagraph("Due date: ", data.Invoice.DueDate, 0));

        return leftCell;
    }

    private Cell CreateHeaderRightCell() {
        var imageData = ImageDataFactory.Create(data.Profile.Image);
        var image = new Image(imageData);

        return new Cell()
            .SetBorder(Border.NO_BORDER)
            .SetHeight(ProfileImageHeight)
            .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
            .Add(image);
    }

    private Paragraph CreateInvoiceTitle() {
        return new Paragraph($"Invoice #{data.Invoice.Number}")
            .SetFont(_boldFont)
            .SetFontSize(HeaderFontSize)
            .SetFontColor(ColorConstants.BLUE)
            .SetMarginBottom(DefaultPadding);
    }

    private Paragraph CreateDateParagraph(string label, DateTime date, int marginBottom) {
        var paragraph = new Paragraph()
            .Add(new Text(label).SetFont(_boldFont))
            .Add(new Text($"{date:d}").SetFont(_regularFont));

        if (marginBottom > 0) {
            paragraph.SetMarginBottom(marginBottom);
        }

        return paragraph;
    }

    private void ComposeContent(Document document) {
        document.Add(new Paragraph().SetMarginBottom(20));

        var addressTable = CreateAddressTable();
        document.Add(addressTable);

        ComposeTable(document);
        AddGrandTotal(document);

        if (!string.IsNullOrWhiteSpace(data.Invoice.Comments)) {
            ComposeComments(document);
        }
    }

    private Table CreateAddressTable() {
        var addressTable = new Table(UnitValue.CreatePercentArray([45f, 10f, 45f]))
            .UseAllAvailableWidth()
            .SetMarginBottom(20);

        var fromCell = CreateFromCell();
        var spacerCell = new Cell().SetBorder(Border.NO_BORDER);
        var forCell = CreateForCell();

        addressTable.AddCell(fromCell);
        addressTable.AddCell(spacerCell);
        addressTable.AddCell(forCell);

        return addressTable;
    }

    private Cell CreateFromCell() {
        var fromCell = new Cell().SetBorder(Border.NO_BORDER);
        fromCell.Add(CreateSectionHeader("From"));
        ComposeProfile(fromCell);
        return fromCell;
    }

    private Cell CreateForCell() {
        var forCell = new Cell().SetBorder(Border.NO_BORDER);
        forCell.Add(CreateSectionHeader("For"));

        AddParagraphToCell(forCell, data.Client.Name, 2);
        AddParagraphToCell(forCell, data.Client.ContactName, 2);
        AddParagraphToCell(forCell, data.Client.ContactPhone, 2);
        AddParagraphToCell(forCell, data.Client.ContactEmail, 0);

        return forCell;
    }

    private Paragraph CreateSectionHeader(string title) {
        return new Paragraph(title)
            .SetFont(_boldFont)
            .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1))
            .SetPaddingBottom(DefaultPadding)
            .SetMarginBottom(DefaultPadding);
    }

    private void AddParagraphToCell(Cell cell, string? text, int marginBottom) {
        if (string.IsNullOrWhiteSpace(text)) {
            return;
        }

        var paragraph = new Paragraph(text).SetFont(_regularFont);
        if (marginBottom > 0) {
            paragraph.SetMarginBottom(marginBottom);
        }

        cell.Add(paragraph);
    }

    private void ComposeProfile(Cell fromCell) {
        AddParagraphToCell(fromCell, $"{data.Profile.FirstName} {data.Profile.LastName}", 2);
        AddParagraphToCell(fromCell, data.Profile.Phone, 2);
        AddParagraphToCell(fromCell, data.Profile.Email, 0);
        AddParagraphToCell(fromCell, data.Profile.Address, 0);
        AddParagraphToCell(fromCell, data.Profile.Address2, 0);

        if (!string.IsNullOrWhiteSpace(data.Profile.City)) {
            var cityProvince = data.Profile.Province is not null
                ? $"{data.Profile.City}, {data.Profile.Province}"
                : data.Profile.City;
            AddParagraphToCell(fromCell, cityProvince, 0);
        }

        AddParagraphToCell(fromCell, data.Profile.Country, 0);
        AddParagraphToCell(fromCell, data.Profile.PostalCode, 0);
    }

    private void AddGrandTotal(Document document) {
        var grandTotalParagraph = new Paragraph($"Grand total: {_grandTotal:C}")
            .SetFont(_boldFont)
            .SetTextAlignment(TextAlignment.RIGHT)
            .SetMarginTop(DefaultMargin)
            .SetMarginRight(DefaultPadding);

        document.Add(grandTotalParagraph);
    }

    private void ComposeTable(Document document) {
        foreach (var projectDto in data.Projects) {
            var projectTimesheetDtos = data.Timesheets
                .Where(t => t.ProjectId == projectDto.Id)
                .ToList();

            if (projectTimesheetDtos.Count == 0) {
                continue;
            }

            var table = CreateProjectTable(projectDto, projectTimesheetDtos);
            document.Add(table);
        }
    }

    private Table CreateProjectTable(ProjectDto projectDto, List<TimesheetDto> projectTimesheetDtos) {
        var table = new Table(UnitValue.CreatePercentArray([5f, 30f, 15f, 15f, 15f, 20f]))
            .UseAllAvailableWidth()
            .SetMarginBottom(15);

        AddProjectTableHeader(table, projectDto.Name);
        AddProjectTableColumnHeaders(table);
        AddProjectTableSeparator(table);
        AddProjectTableRows(table, projectDto, projectTimesheetDtos);

        return table;
    }

    private void AddProjectTableHeader(Table table, string projectName) {
        var headerCell = new Cell(1, 6)
            .Add(new Paragraph(projectName).SetFont(_boldFont))
            .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
            .SetBorder(Border.NO_BORDER)
            .SetPaddingTop(DefaultPadding)
            .SetPaddingBottom(DefaultPadding);

        table.AddHeaderCell(headerCell);
    }

    private void AddProjectTableColumnHeaders(Table table) {
        table.AddHeaderCell(CreateHeaderCell("#"));
        table.AddHeaderCell(CreateHeaderCell("Description"));
        table.AddHeaderCell(CreateHeaderCell("Date", TextAlignment.RIGHT));
        table.AddHeaderCell(CreateHeaderCell("Wage", TextAlignment.RIGHT));
        table.AddHeaderCell(CreateHeaderCell("Hours", TextAlignment.RIGHT));
        table.AddHeaderCell(CreateHeaderCell("Total", TextAlignment.RIGHT));
    }

    private void AddProjectTableSeparator(Table table) {
        var separatorCell = new Cell(1, 6)
            .SetBorder(Border.NO_BORDER)
            .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1))
            .SetHeight(DefaultPadding);

        table.AddCell(separatorCell);
    }

    private void AddProjectTableRows(Table table, ProjectDto projectDto, List<TimesheetDto> projectTimesheetDtos) {
        for (var i = 0; i < projectTimesheetDtos.Count; i++) {
            var timesheet = projectTimesheetDtos[i];
            var workTime = timesheet.EndTime - timesheet.StartTime;
            var total = (decimal)workTime.TotalHours * projectDto.HourlyWage;

            _grandTotal += total;

            table.AddCell(CreateDataCell((i + 1).ToString()));
            table.AddCell(CreateDataCell(string.Empty));
            table.AddCell(CreateDataCell($"{timesheet.Date:d}", TextAlignment.RIGHT));
            table.AddCell(CreateDataCell($"{projectDto.HourlyWage:C}", TextAlignment.RIGHT));
            table.AddCell(CreateDataCell($"{workTime.TotalHours:F2}", TextAlignment.RIGHT));
            table.AddCell(CreateDataCell($"{total:C}", TextAlignment.RIGHT));
        }
    }

    private Cell CreateHeaderCell(string text, TextAlignment alignment = TextAlignment.LEFT) {
        return new Cell()
            .Add(new Paragraph(text).SetFont(_boldFont).SetTextAlignment(alignment))
            .SetBackgroundColor(ColorConstants.WHITE)
            .SetBorder(Border.NO_BORDER)
            .SetPaddingTop(DefaultPadding)
            .SetPaddingBottom(DefaultPadding);
    }

    private Cell CreateDataCell(string text, TextAlignment alignment = TextAlignment.LEFT) {
        return new Cell()
            .Add(new Paragraph(text).SetFont(_regularFont).SetTextAlignment(alignment))
            .SetBorder(Border.NO_BORDER)
            .SetBorderBottom(new SolidBorder(LightGrey, 1))
            .SetPaddingTop(DefaultPadding)
            .SetPaddingBottom(DefaultPadding);
    }

    private void ComposeComments(Document document) {
        var commentsDiv = new Div()
            .SetBackgroundColor(VeryLightGrey)
            .SetPadding(DefaultMargin)
            .SetMarginTop(25);

        commentsDiv.Add(new Paragraph("Comments")
            .SetFont(_boldFont)
            .SetFontSize(CommentsFontSize)
            .SetMarginBottom(DefaultPadding));

        commentsDiv.Add(new Paragraph(data.Invoice.Comments)
            .SetFont(_regularFont));

        document.Add(commentsDiv);
    }

    private void ComposeFooter(Document document, PdfDocument pdf) {
        var numberOfPages = pdf.GetNumberOfPages();
        for (var i = 1; i <= numberOfPages; i++) {
            AddPageNumber(document, pdf.GetPage(i).GetPageSize(), i, numberOfPages);
        }
    }

    private void AddPageNumber(Document document, Rectangle pageSize, int currentPage, int totalPages) {
        var footerText = $"{currentPage} / {totalPages}";

        document.ShowTextAligned(
            new Paragraph(footerText).SetFont(_regularFont),
            pageSize.GetWidth() / 2,
            FooterYPosition,
            currentPage,
            TextAlignment.CENTER,
            VerticalAlignment.BOTTOM,
            0
        );
    }

}