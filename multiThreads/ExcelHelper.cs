using ClosedXML.Excel;

namespace SeekFilesCompare.multiThreads
{
    public abstract class ExcelHelper
    {
        private XLWorkbook _wb;
        private IXLWorksheet _workSheet;

        private static readonly string[] Columns = { "A", "B", "C", "D" };

        private int _cptLine = 2;
        private int _cptContent = 0;
        private const int MAX_CONTENT_AUTO_SAVE = 100;
        private static string file_name_result = "";
        private readonly object _excelLock = new();

        public void WriteContent(List<string> content)
        {
            if (content == null || content.Count == 0)
            {
                return;
            }

            lock (_excelLock)
            {
                _workSheet.Cell(Columns[0] + _cptLine).Value = content.ElementAt(0);
                _workSheet.Cell(Columns[1] + _cptLine).Value = content.ElementAt(1);
                _workSheet.Cell(Columns[2] + _cptLine).Value = content.ElementAt(2);
                _workSheet.Cell(Columns[3] + _cptLine).Value = content.ElementAt(3);
                _cptLine++;
                _cptContent++;
            }
            AutoSave();
        }

        public void SaveExcel()
        {
            try
            {
                _wb.SaveAs(file_name_result);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        public static void SetNameDirectory(string rootFileSrc)
        {
            string[] directories = rootFileSrc.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

            if (directories.Length >= 2)
            {
                file_name_result = $"{directories[directories.Length - 2]}_{directories[directories.Length - 1]}";
            }
            else if (directories.Length == 1)
            {
                file_name_result = directories[0];
            }
            else
            {
                file_name_result = "Unknown";
            }

            file_name_result += "_result.xlsx";
        }

        public void AutoSave()
        {
            lock (_excelLock)
            {
                if (_cptContent < MAX_CONTENT_AUTO_SAVE)
                {
                    return;
                }

                SaveExcel();

                _wb = new XLWorkbook(file_name_result);
                _workSheet = _wb.Worksheet(1);
                _cptContent = 0;
            }
        }

        public class ExcelCreator : ExcelHelper
        {
            private const string WHITE = "#ffffff";
            private const string BLUE = "#0070c0";
            private const string ORANGE = "#fe9d0e";
            private const string GREEN = "#548235";
            private const string RED = "#c00000";

            public ExcelCreator()
            {
                _wb = new XLWorkbook();
                _workSheet = _wb.AddWorksheet("results");

                var headerRow = _workSheet.Row(1);
                headerRow.Cell(1).Value = "Path";
                headerRow.Cell(1).Style.Fill.BackgroundColor = XLColor.FromHtml(BLUE);
                headerRow.Cell(1).Style.Font.FontColor = XLColor.FromHtml(WHITE);
                headerRow.Cell(1).Style.Font.Bold = true;
                headerRow.Cell(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                headerRow.Cell(2).Value = "HashKey";
                headerRow.Cell(2).Style.Fill.BackgroundColor = XLColor.FromHtml(ORANGE);
                headerRow.Cell(2).Style.Font.FontColor = XLColor.FromHtml(WHITE);
                headerRow.Cell(2).Style.Font.Bold = true;
                headerRow.Cell(2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                headerRow.Cell(3).Value = "FileSize";
                headerRow.Cell(3).Style.Fill.BackgroundColor = XLColor.FromHtml(GREEN);
                headerRow.Cell(3).Style.Font.FontColor = XLColor.FromHtml(WHITE);
                headerRow.Cell(3).Style.Font.Bold = true;
                headerRow.Cell(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                headerRow.Cell(4).Value = "Error";
                headerRow.Cell(4).Style.Fill.BackgroundColor = XLColor.FromHtml(RED);
                headerRow.Cell(4).Style.Font.FontColor = XLColor.FromHtml(WHITE);
                headerRow.Cell(4).Style.Font.Bold = true;
                headerRow.Cell(4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
        }
    }
}
