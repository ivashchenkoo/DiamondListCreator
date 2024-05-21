using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using DiamondListCreator.Models;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Action = System.Action;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

namespace DiamondListCreator.Services
{
    public class ExcelDiamondsListService : IDisposable
    {
        private readonly Application xlApp;
        private readonly Workbook xlWorkBook;
        private readonly Worksheet xlWorkSheet;

        private readonly string[] colors;
        private int rowsCount = 0, diamondsIndex = 0;

        public ExcelDiamondsListService(PathSettings paths)
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

        public void Dispose()
        {
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
            FormatWorksheet(lastRow + 1, rowsCount, diamondName.Length);

            diamondsIndex++;
        }

        /// <summary>
        /// Saves the created Excel workbook.
        /// Closes the Excel workbook after saving.
        /// </summary>
        /// <param name="savePath">The directory to save the created workbook</param>
        /// <param name="fileName">The name of the workbook</param>
        public void SaveWorkbook(string savePath, string fileName, bool isWord = false, string diamondsListStr = "")
        {
            dynamic allDataRange = xlWorkSheet.Range[$"A1:D{rowsCount}"];
            allDataRange.Sort(allDataRange.Columns[3], XlSortOrder.xlAscending);
            allDataRange.Sort(allDataRange.Columns[1], XlSortOrder.xlAscending);

            xlWorkBook.CheckCompatibility = false;
            xlWorkBook.DoNotPromptForConvert = true;

            if (!isWord)
            {
                try
                {
                    xlWorkBook.SaveAs(Path.Combine(savePath, fileName + ".xls"), XlFileFormat.xlWorkbookNormal);
                }
                catch (Exception)
                {
                    xlWorkBook.SaveAs(Path.Combine(savePath, fileName + DateTime.Now.ToString("HH-mm-ss") + ".xls"), XlFileFormat.xlWorkbookNormal);
                }
            }
            else
            {
                SaveWordFile(savePath, fileName, diamondsListStr);
            }
            xlWorkBook.Close(true);
        }

        /// <summary>
        /// Saves the created Excel file to the accounting Excel file.
        /// Must be called before SaveWorkbook method.
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
        private void FormatWorksheet(int startRow, int endRow, int nameLength)
        {
            if (startRow <= 0)
            {
                startRow = 1;
            }
            if (startRow > endRow)
            {
                (endRow, startRow) = (startRow, endRow);
            }
            Range range = xlWorkSheet.Range[$"A1:D{endRow}"];
            range.Borders.LineStyle = XlLineStyle.xlContinuous;
            range.Borders.Weight = XlBorderWeight.xlMedium;
            xlWorkSheet.Range[$"A{startRow}:A{endRow}"].Font.Bold = true;
            xlWorkSheet.Range[$"C{startRow}:D{endRow}"].Font.Bold = true;
            xlWorkSheet.Range[$"A{startRow}:A{endRow}"].Font.Size = 16;
            xlWorkSheet.Range[$"C{startRow}:C{endRow}"].Font.Size = 16;
            xlWorkSheet.Range[$"B{startRow}:C{endRow}"].HorizontalAlignment = XlHAlign.xlHAlignCenter;
            xlWorkSheet.Range[$"A{startRow}:A{endRow}"].HorizontalAlignment = XlHAlign.xlHAlignRight;
            range.Columns.EntireColumn.AutoFit();
            if (nameLength > 4)
            {
                xlWorkSheet.Range[$"D{startRow}:D{endRow}"].HorizontalAlignment = XlHAlign.xlHAlignLeft;
                xlWorkSheet.Range[$"D{startRow}:D{endRow}"].Font.Size = 12;
            }
            else
            {
                xlWorkSheet.Range[$"D{startRow}:D{endRow}"].HorizontalAlignment = XlHAlign.xlHAlignRight;
                xlWorkSheet.Range[$"D{startRow}:D{endRow}"].Font.Size = 14;
                xlWorkSheet.Range["D:D"].ColumnWidth = 6;
                xlWorkSheet.Range["D:D"].Font.Name = "Tahoma";
            }
        }

        private void SaveWordFile(string savePath, string fileName, string diamondsListStr)
        {
            // Create a new Word application instance
            Word.Application wordApp = new Word.Application
            {
                Visible = false,
                DisplayAlerts = Word.WdAlertLevel.wdAlertsNone
            };
            Word.Document document = null;

            try
            {
                // Check if Excel is installed on the system
                if (xlApp == null)
                {
                    Console.WriteLine("Excel is not properly installed!!");
                    return;
                }

                // Check if Word is installed on the system
                if (wordApp == null)
                {
                    Console.WriteLine("Word is not properly installed!!");
                    return;
                }

                // Select the range you want to copy
                Excel.Range range = xlWorkSheet.Range[$"A1:D{rowsCount}"];
                RetryAction(() => range.Copy());

                document = RetryAction(() => wordApp.Documents.Add());

                float margin = 0.3f;
                document.PageSetup.TopMargin = wordApp.CentimetersToPoints(margin);
                document.PageSetup.BottomMargin = wordApp.CentimetersToPoints(margin);
                document.PageSetup.LeftMargin = wordApp.CentimetersToPoints(margin);
                document.PageSetup.RightMargin = wordApp.CentimetersToPoints(margin);

                Word.Section section = document.Sections[1];
                section.PageSetup.TextColumns.SetCount(3);

                Word.Range wordRange = document.Content;
                RetryAction(() => wordRange.Paste());

                wordRange.ParagraphFormat.FirstLineIndent = wordApp.CentimetersToPoints(0);
                wordRange.ParagraphFormat.LineSpacingRule = Word.WdLineSpacing.wdLineSpaceMultiple;
                wordRange.ParagraphFormat.LineSpacing = wordApp.LinesToPoints(1);

                Word.Table table = document.Tables[1];
                foreach (Word.Row row in table.Rows)
                {
                    row.Height = wordApp.CentimetersToPoints(0.77f);
                    table.Cell(row.Index, 1).LeftPadding = wordApp.CentimetersToPoints(0.19f);
                    table.Cell(row.Index, 1).RightPadding = wordApp.CentimetersToPoints(0.19f);
                    table.Cell(row.Index, 2).LeftPadding = wordApp.CentimetersToPoints(0.19f);
                    table.Cell(row.Index, 2).RightPadding = wordApp.CentimetersToPoints(0.19f);
                    table.Cell(row.Index, 3).LeftPadding = wordApp.CentimetersToPoints(0.19f);
                    table.Cell(row.Index, 3).RightPadding = wordApp.CentimetersToPoints(0.19f);
                    table.Cell(row.Index, 4).LeftPadding = wordApp.CentimetersToPoints(0.19f);
                    table.Cell(row.Index, 4).RightPadding = wordApp.CentimetersToPoints(0.19f);
                }

                table.Columns[1].Width = wordApp.CentimetersToPoints(1.53f);
                table.Columns[2].Width = wordApp.CentimetersToPoints(1.17f);
                table.Columns[3].Width = wordApp.CentimetersToPoints(1.41f);
                table.Columns[4].Width = wordApp.CentimetersToPoints(1.24f);

                Word.Row emptyRow = table.Rows.Add();
                emptyRow.Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorWhite;

                Word.Row newRow = table.Rows.Add();
                newRow.Cells[1].Merge(newRow.Cells[newRow.Cells.Count]);
                Word.Range cellRange = newRow.Cells[1].Range;
                cellRange.Text = "\n" + diamondsListStr;
                cellRange.Font.Name = "Calibri";
                cellRange.Font.Size = 14;
                cellRange.Font.Bold = 1;
                cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                cellRange.Shading.BackgroundPatternColor = Word.WdColor.wdColorYellow;

                RetryAction(() => document.SaveAs2((Path.Combine(savePath, fileName + ".docx"), XlFileFormat.xlWorkbookNormal)));
                document.Close();

                Console.WriteLine("Table copied from Excel and pasted into Word successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                if (wordApp != null)
                {
                    wordApp.Quit();
                    Marshal.ReleaseComObject(wordApp);
                }
            }
        }

        private void RetryAction(Action action, int maxRetries = 3, int delayMilliseconds = 500)
        {
            int attempts = 0;
            while (true)
            {
                try
                {
                    action();
                    return;
                }
                catch (COMException ex) when (ex.ErrorCode == unchecked((int)0x8001010A))
                {
                    attempts++;
                    if (attempts >= maxRetries)
                    {
                        throw;
                    }
                    System.Threading.Thread.Sleep(delayMilliseconds);
                }
            }
        }

        private T RetryAction<T>(Func<T> func, int maxRetries = 3, int delayMilliseconds = 500)
        {
            int attempts = 0;
            while (true)
            {
                try
                {
                    return func();
                }
                catch (COMException ex) when (ex.ErrorCode == unchecked((int)0x8001010A))
                {
                    attempts++;
                    if (attempts >= maxRetries)
                    {
                        throw;
                    }
                    System.Threading.Thread.Sleep(delayMilliseconds);
                }
            }
        }
    }
}
