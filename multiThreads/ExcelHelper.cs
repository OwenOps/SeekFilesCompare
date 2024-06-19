using ClosedXML.Excel;
using System.Reflection;

namespace SeekFilesCompare.multiThreads
{
    public abstract class ExcelHelper
    {
        private XLWorkbook? _wb;
        private IXLWorksheet? _workSheet;

        private static readonly string[] Columns = ["A", "B", "C", "D"];

        private int _cptLine = 2;
        private int _cptContent = 0;
        private const int MaxContentAutoSave = 100;
        private static string _fileNameResult = "";
        private readonly object _excelLock = new();

        public void WriteContent(List<string>? content)
        {
            if (content == null || content.Count == 0)
            {
                return;
            }

            lock (_excelLock)
            {
                if (_workSheet != null)
                {
                    _workSheet.Cell(Columns[0] + _cptLine).Value = content.ElementAt(0);
                    _workSheet.Cell(Columns[1] + _cptLine).Value = content.ElementAt(1);
                    _workSheet.Cell(Columns[2] + _cptLine).Value = content.ElementAt(2);
                    _workSheet.Cell(Columns[3] + _cptLine).Value = content.ElementAt(3);
                }

                _cptLine++;
                _cptContent++;
            }
            AutoSave();
        }

        public void SaveExcel()
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(baseDirectory, _fileNameResult);
                _wb?.SaveAs(filePath);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        private string GetExecutionPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        }

        public static void SetNameDirectory(string rootFileSrc)
        {
            string[] directories = rootFileSrc.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

            if (directories.Length >= 2)
            {
                _fileNameResult = $"{directories[^2]}_{directories[^1]}";
            }
            else if (directories.Length == 1)
            {
                _fileNameResult = directories[0];
            }
            else
            {
                _fileNameResult = "Unknown";
            }

            _fileNameResult += "_result.xlsx";
        }

        public void AutoSave()
        {
            lock (_excelLock)
            {
                if (_cptContent < MaxContentAutoSave)
                {
                    return;
                }

                SaveExcel();

                _wb = new XLWorkbook(Path.Combine(GetExecutionPath(), _fileNameResult));
                _workSheet = _wb.Worksheet(1);
                _cptContent = 0;
            }
        }

        public class ExcelCreator : ExcelHelper
        {
            private const string White = "#ffffff";
            private const string Blue = "#0070c0";
            private const string Orange = "#fe9d0e";
            private const string Green = "#548235";
            private const string Red = "#c00000";

            public ExcelCreator()
            {
                _wb = new XLWorkbook();
                _workSheet = _wb.AddWorksheet("results");

                var headerRow = _workSheet.Row(1);
                headerRow.Cell(1).Value = "Path";
                headerRow.Cell(1).Style.Fill.BackgroundColor = XLColor.FromHtml(Blue);
                headerRow.Cell(1).Style.Font.FontColor = XLColor.FromHtml(White);
                headerRow.Cell(1).Style.Font.Bold = true;
                headerRow.Cell(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                headerRow.Cell(2).Value = "HashKey";
                headerRow.Cell(2).Style.Fill.BackgroundColor = XLColor.FromHtml(Orange);
                headerRow.Cell(2).Style.Font.FontColor = XLColor.FromHtml(White);
                headerRow.Cell(2).Style.Font.Bold = true;
                headerRow.Cell(2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                headerRow.Cell(3).Value = "FileSize";
                headerRow.Cell(3).Style.Fill.BackgroundColor = XLColor.FromHtml(Green);
                headerRow.Cell(3).Style.Font.FontColor = XLColor.FromHtml(White);
                headerRow.Cell(3).Style.Font.Bold = true;
                headerRow.Cell(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                headerRow.Cell(4).Value = "Error";
                headerRow.Cell(4).Style.Fill.BackgroundColor = XLColor.FromHtml(Red);
                headerRow.Cell(4).Style.Font.FontColor = XLColor.FromHtml(White);
                headerRow.Cell(4).Style.Font.Bold = true;
                headerRow.Cell(4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
        }
    }
}
