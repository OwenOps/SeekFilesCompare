using IronXL;
using IronXL.Styles;

namespace Sha256.oneThread
{
    public abstract class ExcelHelper
    {
        private WorkBook _wb;
        private WorkSheet _workSheet;

        private const string FileName = "results";
        private static readonly string[] Columns = ["A", "B", "C", "D"];

        private int _cptLine = 2;
        private int _cptContent = 0;
        private const int LimitContentSave = 50;

        public void WriteContent(List<string> content)
        {
            if (content == null || content.Count == 0)
            {
                return;
            }

            _workSheet[Columns[0] + _cptLine].Value = content.ElementAt(0);
            _workSheet[Columns[1] + _cptLine].Value = content.ElementAt(1);
            _workSheet[Columns[2] + _cptLine].Value = content.ElementAt(2);
            _workSheet[Columns[3] + _cptLine].Value = content.ElementAt(3);
            _cptLine++;
            _cptContent++;
            AutoSave();
        }

        public void SaveExcel()
        {
            try
            {
                _wb.SaveAs($"{FileName}.xlsx");
            }
            catch (Exception ex) { Console.Error.WriteLine(ex.ToString()); }
        }

        public void AutoSave()
        {

            if (_cptContent < LimitContentSave)
            {
                return;
            }

            SaveExcel();

            _wb = WorkBook.Load($"{FileName}.xlsx");
            _workSheet = _wb.WorkSheets[0];
            _cptContent = 0;
        }

        public class ExcelCreator : ExcelHelper
        {
            public ExcelCreator()
            {
                _wb = WorkBook.Create(ExcelFileFormat.XLSX);
                _workSheet = _wb.CreateWorkSheet(FileName);

                _workSheet["A1"].Value = "Path";
                _workSheet["A1"].Style.BackgroundColor = "#0070c0";
                _workSheet["A1"].Style.Font.Color = "#ffffff";
                _workSheet["A1"].Style.Font.Bold = true;
                _workSheet["A1"].Style.HorizontalAlignment = HorizontalAlignment.Center;

                _workSheet["B1"].Value = "HashKey";
                _workSheet["B1"].Style.BackgroundColor = "#fe9d0e";
                _workSheet["B1"].Style.Font.Color = "#ffffff";
                _workSheet["B1"].Style.Font.Bold = true;
                _workSheet["B1"].Style.HorizontalAlignment = HorizontalAlignment.Center;

                _workSheet["C1"].Value = "FileSize";
                _workSheet["C1"].Style.BackgroundColor = "#548235";
                _workSheet["C1"].Style.Font.Color = "#ffffff";
                _workSheet["C1"].Style.Font.Bold = true;
                _workSheet["C1"].Style.HorizontalAlignment = HorizontalAlignment.Center;

                _workSheet["D1"].Value = "Error";
                _workSheet["D1"].Style.BackgroundColor = "#c00000";
                _workSheet["D1"].Style.Font.Color = "#ffffff";
                _workSheet["D1"].Style.Font.Bold = true;
                _workSheet["D1"].Style.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }
    }
}
