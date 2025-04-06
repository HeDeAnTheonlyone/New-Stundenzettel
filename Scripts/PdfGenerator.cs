using System.IO;
using Godot;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using Orientation = MigraDoc.DocumentObjectModel.Orientation;
using VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment;

public class PdfGenerator
{
    public static void GeneratePdf(Timesheet timesheet)
    {
        WrapFormat wrapFmt = new();
        wrapFmt.Style = WrapStyle.Through;

        XFont fnt = new("arial", 14);

        // Set up document
        var doc = new Document();
        var normalStyle = doc.Styles[StyleNames.Normal];
        normalStyle.Font.Size = Unit.FromPoint(14);

        // Set up content section
        var sec = doc.AddSection();
        sec.PageSetup.PageFormat = PageFormat.A4;
        sec.PageSetup.Orientation = Orientation.Landscape;
        sec.PageSetup.TopMargin = sec.PageSetup.LeftMargin = 0;

        // Create PDF components
        CreateBackgroundImage(sec, wrapFmt);
        CreatTimespanRow(sec, wrapFmt, timesheet);
        CreateName(sec, wrapFmt);
        CreateTimesheetTable(sec, wrapFmt, timesheet);

        // Render PDF
        var renderer = new PdfDocumentRenderer();
        renderer.Document = doc;
        renderer.RenderDocument();
        renderer.Save(Path.Join(Manager.SaveDir, "stundenzettel", $"Arbeisbericht {Settings.UserSettings.User} [{timesheet.FomDate:d}-{timesheet.FomDate.AddDays(5):d}].pdf"));
    }

    private static void CreateBackgroundImage(Section sec, WrapFormat wrapFmt)
    {
        var img = sec.AddImage(Path.Join(OS.GetUserDataDir(), "Template.jpg"));
        img.WrapFormat = wrapFmt.Clone();
        img.Width = sec.PageSetup.PageWidth;
        img.Height = sec.PageSetup.PageHeight;
    }

    private static void CreatTimespanRow(Section sec, WrapFormat wrawFmt, Timesheet timesheet)
    {
        var frame = sec.AddTextFrame();
        frame.WrapFormat = wrawFmt.Clone();
        frame.Left = ShapePosition.Left;
        frame.Top = ShapePosition.Top;
        frame.MarginTop = 77;
        frame.MarginLeft = 200;

        var table = frame.AddTable();
        table.AddColumn(70);
        table.AddColumn(30);
        table.AddColumn(70);

        var row = table.AddRow();
        row.Cells[0].AddParagraph($"{timesheet.FomDate:d}");
        row.Cells[2].AddParagraph($"{timesheet.FomDate.AddDays(5):d}");
    }

    private static void CreateName(Section sec, WrapFormat wrapFmt)
    {
        var frame = sec.AddTextFrame();
        frame.WrapFormat = wrapFmt.Clone();
        frame.Left = ShapePosition.Left;
        frame.Top = ShapePosition.Top;
        frame.MarginTop = 95;
        frame.MarginLeft = 140;
        frame.Width = 500;

        frame.AddParagraph(Settings.UserSettings.User);
    }

    private static void CreateTimesheetTable(Section sec, WrapFormat wrapFmt, Timesheet timesheet)
    {
        var frame = sec.AddTextFrame();
        frame.WrapFormat = wrapFmt.Clone();
        frame.Left = ShapePosition.Left;
        frame.Top = ShapePosition.Top;
        frame.MarginTop = 161;
        frame.MarginLeft = 131;

        var table = frame.AddTable();
        foreach (short width in new short[] {50, 46, 260, 141, 41}) table.AddColumn(width);
        for (byte i = 0; i < 6; i++)
        {
            var row = table.AddRow();
            row.VerticalAlignment = VerticalAlignment.Center;
            row.Height = 58;
            
            var hoursPrgph = row.Cells[0].AddParagraph(timesheet.Entries[i].Hours);
            hoursPrgph.Format.Alignment = ParagraphAlignment.Center;

            var locationCell = row.Cells[2].AddParagraph(timesheet.Entries[i].Location);
            locationCell.Format.LeftIndent = 5;

            var managerCell = row.Cells[3].AddParagraph(timesheet.Entries[i].Manager);
            managerCell.Format.Alignment = ParagraphAlignment.Center;
            
            var kmCell = row.Cells[4].AddParagraph(timesheet.Entries[i].Km);
            kmCell.Format.Alignment = ParagraphAlignment.Center;
        }
    }
}