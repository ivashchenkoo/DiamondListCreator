using DiamondListCreator.Models;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace DiamondListCreator.Services
{
    public class DiamondListService
    {
        private readonly Application xlApp;
        private readonly Workbook xlWorkBook;
        private readonly Worksheet xlWorkSheet;

        private readonly string[] colors;
        private int rowsCount = 0, diamondsIndex = 0;

        public DiamondListService(PathSettings paths)
        {
            xlApp = new Application();
            if (xlApp == null)
            {
                throw new Exception("Excel is not properly installed!");
            }
            xlApp.DisplayAlerts = false;

            colors = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(Environment.CurrentDirectory + "\\Config\\colors.json"));

            xlWorkBook = xlApp.Workbooks.Add(System.Reflection.Missing.Value);
            xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);
        }

        ~DiamondListService()
        {
            xlWorkBook.Close(true);
            xlApp.Quit();
        }

        /// <summary>
        /// Appends diamonds colors to the Excel workbook
        /// </summary>
        /// <param name="diamondColors">The list with diamonds colors</param>
        /// <param name="diamondName">The short diamond name</param>
        public void AddDiamondColorsToWorkBook(List<DiamondColor> diamondColors, string diamondName)
        {
            int lastRow = rowsCount;

            for (int j = 0; j < diamondColors.Count; j++)
            {
                rowsCount++;
                xlWorkSheet.Cells[rowsCount, 1] = diamondColors[j].Name;
                xlWorkSheet.Cells[rowsCount, 2] = diamondColors[j].Quantity;
                xlWorkSheet.Cells[rowsCount, 3] = diamondColors[j].Weight;
                xlWorkSheet.Cells[rowsCount, 4] = diamondName;
            }

            xlWorkSheet.Range[$"A{lastRow + 1}:D{rowsCount}"].Interior.Color = ColorTranslator.ToOle(ColorTranslator.FromHtml(colors[diamondsIndex]));

            diamondsIndex++;
        }

        /// <summary>
        /// Saves the created Excel workbook
        /// </summary>
        /// <param name="savePath">The directory to save the created workbook</param>
        /// <param name="fileName">The name of the workbook</param>
        public void SaveWorkbook(string savePath, string fileName)
        {
            FormatWorksheet();

            xlWorkBook.CheckCompatibility = false;
            xlWorkBook.DoNotPromptForConvert = true;
            xlWorkBook.SaveAs($"{savePath}/{fileName}.xls", XlFileFormat.xlWorkbookNormal);
        }

        /// <summary>
        /// Saves the created Excel file to the accounting Excel file
        /// </summary>
        /// <param name="accountingPath">The path to the Excel file with diamonds colors accounting</param>
        public void SaveAccounting(string accountingPath)
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
        private void FormatWorksheet()
        {
            Range range = xlWorkSheet.Range[$"A1:D{rowsCount}"];
            range.Borders.LineStyle = XlLineStyle.xlContinuous;
            range.Borders.Weight = XlBorderWeight.xlMedium;
            xlWorkSheet.Range[$"A1:A{rowsCount}"].Font.Bold = true;
            xlWorkSheet.Range[$"C1:D{rowsCount}"].Font.Bold = true;
            xlWorkSheet.Range[$"A1:A{rowsCount}"].Font.Size = 16;
            xlWorkSheet.Range[$"C1:C{rowsCount}"].Font.Size = 16;
            xlWorkSheet.Range[$"B1:C{rowsCount}"].HorizontalAlignment = XlHAlign.xlHAlignCenter;
            xlWorkSheet.Range[$"A1:A{rowsCount}"].HorizontalAlignment = XlHAlign.xlHAlignRight;
            xlWorkSheet.Range[$"D1:D{rowsCount}"].HorizontalAlignment = XlHAlign.xlHAlignLeft;
            xlWorkSheet.Range[$"D1:D{rowsCount}"].Font.Size = 12;
            range.Columns.EntireColumn.AutoFit();

            dynamic allDataRange = xlWorkSheet.Range[$"A1:D{rowsCount}"];
            allDataRange.Sort(allDataRange.Columns[3], XlSortOrder.xlAscending);
            allDataRange.Sort(allDataRange.Columns[1], XlSortOrder.xlAscending);
        }
    }
}
