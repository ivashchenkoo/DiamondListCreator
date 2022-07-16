using DiamondListCreator.Models;
using DiamondListCreator.Services.ConsumablesCreators;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DiamondListCreator.Services
{
    public class DiamondListService
    {
        private readonly Application xlApp;
        private readonly string[] colors = { "#ebf1dd", "#99ff66", "#ffccff", "#dbeef3", "#8db3e2", "#ff99ff", "#fdeada", "#92cddc", "#ffff99", "#6699ff",
                                   "#dbe5f1", "#938953", "#99ff99", "#7f7f7f", "#f2dcdb", "#00b0f0", "#cc66ff", "#ddd9c3", "#f2f2f2", "#ccc1d9",
                                   "#e5e0ec", "#ff0000", "#fac08f", "#d99694", "#95b3d7", "#b7dde8", "#b8cce4", "#c4bd97", "#ff7c80", "#e5b9b7",
                                   "#cccc00", "#66ffff", "#548dd4", "#d7e3bc", "#c6d9f0", "#c3d69b", "#b2a1c7", "#cc66ff", "#fbd5b5", "#ffc000"};

        public DiamondListService()
        {
            xlApp = new Application();
            if (xlApp == null)
            {
                throw new Exception("Excel is not properly installed!");
            }
            xlApp.DisplayAlerts = false;
        }

        ~DiamondListService()
        {
            xlApp.Quit();
        }

        /// <summary>
        /// Creates diamonds list in MS Excel
        /// </summary>
        /// <param name="diamonds"></param>
        /// <param name="paths"></param>
        /// <param name="isSaveAccounting">Saves created Excel file to accounting table</param>
        /// <param name="isSaveListStickersPdf">Creates pdf with diamonds colors list stickers</param>
        public void CreateDiamondsList(List<DiamondSettings> diamonds, PathSettings paths, bool isSaveAccounting, bool isSaveListStickersPdf)
        {
            Workbook xlWorkBook = xlApp.Workbooks.Add(System.Reflection.Missing.Value);
            Worksheet xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

            ColorsListCreator colorsListCreator = new ColorsListCreator();
            List<DiamondColor> diamondsColors = new List<DiamondColor>();

            int row = 0;
            for (int i = 0; i < diamonds.Count; i++)
            {
                List<DiamondColor> diamondColors = colorsListCreator.Create(diamonds[i]);

                for (int j = 0; j < diamondColors.Count; j++)
                {
                    row++;
                    xlWorkSheet.Cells[row, 1] = diamondColors[j].Name;
                    xlWorkSheet.Cells[row, 2] = diamondColors[j].Quantity;
                    xlWorkSheet.Cells[row, 3] = diamondColors[j].Weight;
                    xlWorkSheet.Cells[row, 4] = diamonds[i].Name;
                }

                xlWorkSheet.Range[$"A{diamondColors.Count + 1}:D{row}"].Interior.Color = ColorTranslator.ToOle(ColorTranslator.FromHtml(colors[i]));

                if (isSaveListStickersPdf)
                {
                    diamondsColors.AddRange(diamondColors);
                }
            }

            xlWorkBook.CheckCompatibility = false;
            xlWorkBook.DoNotPromptForConvert = true;
            xlWorkBook.SaveAs($"{paths.FilesSavePath}/DiamondsList {DateTime.Now: dd.MM.yyyy}.xls", XlFileFormat.xlWorkbookNormal);

            if (isSaveAccounting)
            {
                SaveAccounting(xlWorkSheet, paths.AccountingExcelFilePath, row);
            }

            xlWorkBook.Close(true);

            if (isSaveListStickersPdf)
            {
                ListStickersService listStickersService = new ListStickersService();
                listStickersService.CreateListStickersPdf(diamondsColors, paths.FilesSavePath);
            }
        }

        /// <summary>
        /// Saves created Excel file to accounting table
        /// </summary>
        /// <param name="xlWorkSheet"></param>
        /// <param name="accountingPath"></param>
        /// <param name="rowsCount"></param>
        private void SaveAccounting(Worksheet xlWorkSheet, string accountingPath, int rowsCount)
        {
            Workbook xlWorkBookAccounting = xlApp.Workbooks.Open(accountingPath);
            Worksheet xlWorkSheetAccounting = (Worksheet)xlWorkBookAccounting.Worksheets.get_Item(1);

            int lastUsedRowAccounting = xlWorkSheetAccounting.Cells.Find("*", System.Reflection.Missing.Value,
                                           System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                                           XlSearchOrder.xlByRows, XlSearchDirection.xlPrevious,
                                           false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;

            Range sourceRange = xlWorkSheet.get_Range("A1", "D" + rowsCount);
            Range destinationRange = xlWorkSheetAccounting.get_Range("A" + (lastUsedRowAccounting + 1).ToString() + ":D" + (lastUsedRowAccounting + rowsCount).ToString());
            sourceRange.Copy(destinationRange);
            xlWorkBookAccounting.Close(true);
        }

        /// <summary>
        /// Formats Excel worksheet
        /// </summary>
        /// <param name="lastRow"></param>
        /// <param name="xlWorkSheet"></param>
        private void FormatWorksheet(int lastRow, ref Worksheet xlWorkSheet)
        {
            Range range = xlWorkSheet.Range[$"A1:D{lastRow}"];
            range.Borders.LineStyle = XlLineStyle.xlContinuous;
            range.Borders.Weight = XlBorderWeight.xlMedium;
            xlWorkSheet.Range[$"A1:A{lastRow}"].Font.Bold = true;
            xlWorkSheet.Range[$"C1:D{lastRow}"].Font.Bold = true;
            xlWorkSheet.Range[$"A1:A{lastRow}"].Font.Size = 16;
            xlWorkSheet.Range[$"C1:C{lastRow}"].Font.Size = 16;
            xlWorkSheet.Range[$"B1:C{lastRow}"].HorizontalAlignment = XlHAlign.xlHAlignCenter;
            xlWorkSheet.Range[$"A1:A{lastRow}"].HorizontalAlignment = XlHAlign.xlHAlignRight;
            xlWorkSheet.Range[$"D1:D{lastRow}"].HorizontalAlignment = XlHAlign.xlHAlignLeft;
            xlWorkSheet.Range[$"D1:D{lastRow}"].Font.Size = 12;
            range.Columns.EntireColumn.AutoFit();

            dynamic allDataRange = xlWorkSheet.Range[$"A1:D{lastRow}"];
            allDataRange.Sort(allDataRange.Columns[3], XlSortOrder.xlAscending);
            allDataRange.Sort(allDataRange.Columns[1], XlSortOrder.xlAscending);
        }
    }
}
