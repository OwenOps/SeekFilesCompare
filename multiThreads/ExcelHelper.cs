using IronXL;
using IronXL.Styles;
using Sha256.environment;

namespace Sha256.multiThreads
{
    public abstract class ExcelHelper
    {
        private WorkBook _wb;
        private WorkSheet _workSheet;

        private static readonly string[] Columns = ["A", "B", "C", "D"];

        private int _cptLine = 2;
        private int _cptContent = 0;
        private const int MAX_CONTENT_AUTO_SAVE = 100;
        private readonly object _excelLock = new();

        public void WriteContent(List<string> content)
        {
            if (content == null || content.Count == 0)
            {
                return;
            }

            lock (_excelLock)
            {
                _workSheet[Columns[0] + _cptLine].Value = content.ElementAt(0);
                _workSheet[Columns[1] + _cptLine].Value = content.ElementAt(1);
                _workSheet[Columns[2] + _cptLine].Value = content.ElementAt(2);
                _workSheet[Columns[3] + _cptLine].Value = content.ElementAt(3);
                _cptLine++;
                _cptContent++;
            }
            AutoSave();
        }

        public void SaveExcel()
        {
            try
            {
                _wb.SaveAs(Settings.PathResult);

            }
            catch (Exception ex) { Console.Error.WriteLine(ex.ToString()); }
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

                _wb = WorkBook.Load(Settings.PathResult);
                _workSheet = _wb.WorkSheets[0];
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
                _wb = WorkBook.Create(ExcelFileFormat.XLSX);
                _workSheet = _wb.CreateWorkSheet("results");

                _workSheet["A1"].Value = "Path";
                _workSheet["A1"].Style.BackgroundColor = BLUE;
                _workSheet["A1"].Style.Font.Color = WHITE;
                _workSheet["A1"].Style.Font.Bold = true;
                _workSheet["A1"].Style.HorizontalAlignment = HorizontalAlignment.Center;

                _workSheet["B1"].Value = "HashKey";
                _workSheet["B1"].Style.BackgroundColor = ORANGE;
                _workSheet["B1"].Style.Font.Color = WHITE;
                _workSheet["B1"].Style.Font.Bold = true;
                _workSheet["B1"].Style.HorizontalAlignment = HorizontalAlignment.Center;

                _workSheet["C1"].Value = "FileSize";
                _workSheet["C1"].Style.BackgroundColor = GREEN;
                _workSheet["C1"].Style.Font.Color = WHITE;
                _workSheet["C1"].Style.Font.Bold = true;
                _workSheet["C1"].Style.HorizontalAlignment = HorizontalAlignment.Center;

                _workSheet["D1"].Value = "Error";
                _workSheet["D1"].Style.BackgroundColor = RED;
                _workSheet["D1"].Style.Font.Color = RED;
                _workSheet["D1"].Style.Font.Bold = true;
                _workSheet["D1"].Style.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }
    }
}
