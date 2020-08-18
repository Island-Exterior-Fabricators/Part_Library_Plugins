using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Part_Library_App
{
    class ExcelHelper
    {
        public static Sheets GetAllWorksheets(string fileName)
        {
            Sheets theSheets = null;

            using (SpreadsheetDocument document =
                SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart wbPart = document.WorkbookPart;
                theSheets = wbPart.Workbook.Sheets;
            }
            return theSheets;
        }

        public static int ColumnIndex(string reference)
        {
            int ci = 0;
            reference = reference.ToUpper();
            for (int ix = 0; ix < reference.Length && reference[ix] >= 'A'; ix++)
                ci = (ci * 26) + ((int)reference[ix] - 64);
            return ci;
        }

        public static Dictionary<int, string> LoadDictionary(SharedStringTable sst)
        {
            int i = 0;
            Dictionary<int, string> dict = new Dictionary<int, string>();
            foreach (var ss in sst.ChildElements)
                dict.Add(i++, ss.InnerText);
            return dict;
        }

        public class ExcelCellWithType
        {
            public string Value { get; set; }
            public UInt32Value ExcelCellFormat { get; set; }
            public bool IsDateTimeType { get; set; }
        }

        public static ExcelCellWithType ReadExcelCell(Cell cell, WorkbookPart workbookPart)
        {
            var cellValue = cell.CellValue;
            var text = (cellValue == null) ? cell.InnerText : cellValue.Text;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                text = workbookPart.SharedStringTablePart.SharedStringTable
                  .Elements<SharedStringItem>().ElementAt(
                  Convert.ToInt32(cell.CellValue.Text)).InnerText;
            }

            var cellText = (text ?? string.Empty).Trim();

            var cellWithType = new ExcelCellWithType();

            if (cell.StyleIndex != null)
            {
                var cellFormat = workbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ChildElements[
                  int.Parse(cell.StyleIndex.InnerText)] as CellFormat;

                if (cellFormat != null)
                {
                    cellWithType.ExcelCellFormat = cellFormat.NumberFormatId;

                    var dateFormat = GetDateTimeFormat(cellFormat.NumberFormatId);
                    if (!string.IsNullOrEmpty(dateFormat))
                    {
                        cellWithType.IsDateTimeType = true;

                        if (!string.IsNullOrEmpty(cellText))
                        {
                            double cellDouble;
                            if (double.TryParse(cellText, out cellDouble))
                            {
                                var theDate = DateTime.FromOADate(cellDouble);
                                cellText = theDate.ToString(dateFormat);
                            }
                        }
                    }
                }
            }

            cellWithType.Value = cellText;

            return cellWithType;
        }

        public static string GetDateTimeFormat(UInt32Value numberFormatId)
        {
            return DateFormatDictionary.ContainsKey(numberFormatId) ? DateFormatDictionary[numberFormatId] : string.Empty;
        }

        public static Dictionary<uint, string> DateFormatDictionary = new Dictionary<uint, string>();

        public static void SetDictionary()
        {
            DateFormatDictionary[14] = "dd/MM/yyyy";
            DateFormatDictionary[15] = "d-MMM-yy";
            DateFormatDictionary[16] = "d-MMM";
            DateFormatDictionary[17] = "MMM-yy";
            DateFormatDictionary[18] = "h:mm AM/PM";
            DateFormatDictionary[19] = "h:mm:ss AM/PM";
            DateFormatDictionary[20] = "h:mm";
            DateFormatDictionary[21] = "h:mm:ss";
            DateFormatDictionary[22] = "M/d/yy h:mm";
            DateFormatDictionary[30] = "M/d/yy";
            DateFormatDictionary[34] = "yyyy-MM-dd";
            DateFormatDictionary[45] = "mm:ss";
            DateFormatDictionary[46] = "[h]:mm:ss";
            DateFormatDictionary[47] = "mmss.0";
            DateFormatDictionary[51] = "MM-dd";
            DateFormatDictionary[52] = "yyyy-MM-dd";
            DateFormatDictionary[53] = "yyyy-MM-dd";
            DateFormatDictionary[55] = "yyyy-MM-dd";
            DateFormatDictionary[56] = "yyyy-MM-dd";
            DateFormatDictionary[58] = "MM-dd";
            DateFormatDictionary[165] = "M/d/yy";
            DateFormatDictionary[166] = "dd MMMM yyyy";
            DateFormatDictionary[167] = "dd/MM/yyyy";
            DateFormatDictionary[168] = "dd/MM/yy";
            DateFormatDictionary[169] = "d.M.yy";
            DateFormatDictionary[170] = "yyyy-MM-dd";
            DateFormatDictionary[171] = "dd MMMM yyyy";
            DateFormatDictionary[172] = "d MMMM yyyy";
            DateFormatDictionary[173] = "M/d";
            DateFormatDictionary[174] = "M/d/yy";
            DateFormatDictionary[175] = "MM/dd/yy";
            DateFormatDictionary[176] = "d-MMM";
            DateFormatDictionary[177] = "d-MMM-yy";
            DateFormatDictionary[178] = "dd-MMM-yy";
            DateFormatDictionary[179] = "MMM-yy";
            DateFormatDictionary[180] = "MMMM-yy";
            DateFormatDictionary[181] = "MMMM d, yyyy";
            DateFormatDictionary[182] = "M/d/yy hh:mm t";
            DateFormatDictionary[183] = "M/d/y HH:mm";
            DateFormatDictionary[184] = "MMM";
            DateFormatDictionary[185] = "MMM-dd";
            DateFormatDictionary[186] = "M/d/yyyy";
            DateFormatDictionary[187] = "d-MMM-yyyy";
        }

        public static Sheet GetSheetFromWorkSheet(WorkbookPart workbookPart, WorksheetPart worksheetPart)
        {
            string relationshipId = workbookPart.GetIdOfPart(worksheetPart);
            IEnumerable<Sheet> sheets = workbookPart.Workbook.Sheets.Elements<Sheet>();
            return sheets.FirstOrDefault(s => s.Id.HasValue && s.Id.Value == relationshipId);
        }

        public static string GetCell(SpreadsheetDocument document, Cell cell, SharedStringTablePart sstp)
        {
            WorkbookPart wbPart = document.WorkbookPart;

            string cellVal;
            if (cell.DataType != null && cell.DataType == CellValues.SharedString)
            {
                //it's a shared string so use the cell inner text as the index into the 
                //shared strings table
                var stringId = Convert.ToInt32(cell.InnerText);
                cellVal = sstp.SharedStringTable
                    .Elements<SharedStringItem>().ElementAt(stringId).InnerText;
            }
            else
            {
                //it's NOT a shared string, use the value directly
                cellVal = cell.InnerText;
            }

            return cellVal;
        }

        public static string getXLFileName(string xlFileLink, out string xlFileName)
        {
            int xlNameStart = xlFileLink.LastIndexOf("/");
            int xlNameEnd = xlFileLink.LastIndexOf(".xlsm");

            xlFileName = xlFileLink.Substring(xlNameStart + 1, xlNameEnd - xlNameStart + 4);
            xlFileName = xlFileName.Replace("%20", " ");

            return xlFileName;
        }
    }
}
