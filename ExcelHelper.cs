using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OfficeFlow
{
    class ExcelHelper
    {
        public static void ExportMonthToExcel(int userId, string username, int month, int year)
        {
            // Starten der Excel Anwendung und Erstellen eines neuen Arbeitsblatts
            Application excelApp = new Application();
            Workbook workbook = excelApp.Workbooks.Add();
            Worksheet worksheet = (Worksheet)excelApp.ActiveSheet;
            int row = 1;

            // Vorbereiten der Datenanzeige
            DateTime date = new DateTime(year, month, 1);
            string formatedDate = date.ToString("MMMM yyyy");
            worksheet.Cells[row, 1] = "Zeitraum: " + formatedDate;
            worksheet.Cells[row, 1].Font.Bold = true;
            Microsoft.Office.Interop.Excel.Range range4 = worksheet.Range[worksheet.Cells[row, 1], worksheet.Cells[row, 6]];
            range4.Merge();
            range4.HorizontalAlignment = XlHAlign.xlHAlignLeft;

            row++;

            // Vorbereiten der Nutzeranzeige
            worksheet.Cells[row, 1] = "Nutzer: " + username;
            worksheet.Cells[row, 1].Font.Bold = true;
            Microsoft.Office.Interop.Excel.Range range5 = worksheet.Range[worksheet.Cells[row, 1], worksheet.Cells[row, 6]];
            range5.Merge();
            range5.HorizontalAlignment = XlHAlign.xlHAlignLeft;

            row++;
            row++;

            // Vorbereiten der ersten Kopfzeile
            worksheet.Cells[row, 1] = "Startzeit";
            Microsoft.Office.Interop.Excel.Range range1 = worksheet.Range[worksheet.Cells[row, 1], worksheet.Cells[row, 2]];
            range1.Merge();
            range1.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            worksheet.Cells[row, 3] = "Endzeit";
            Microsoft.Office.Interop.Excel.Range range2 = worksheet.Range[worksheet.Cells[row, 3], worksheet.Cells[row, 4]];
            range2.Merge();
            range2.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            worksheet.Cells[row, 5] = "Zeiten";
            Microsoft.Office.Interop.Excel.Range range3 = worksheet.Range[worksheet.Cells[row, 5], worksheet.Cells[row, 6]];
            range3.Merge();
            range3.HorizontalAlignment = XlHAlign.xlHAlignCenter;

            row++;

            // Vorbereiten der zweiten Kopfzeile
            worksheet.Cells[row, 1] = "Tag";
            worksheet.Cells[row, 1].HorizontalAlignment = XlHAlign.xlHAlignCenter;
            worksheet.Cells[row, 2] = "Uhrzeit";
            worksheet.Cells[row, 2].HorizontalAlignment = XlHAlign.xlHAlignCenter;
            worksheet.Cells[row, 3] = "Tag";
            worksheet.Cells[row, 3].HorizontalAlignment = XlHAlign.xlHAlignCenter;
            worksheet.Cells[row, 4] = "Uhrzeit";
            worksheet.Cells[row, 4].HorizontalAlignment = XlHAlign.xlHAlignCenter;
            worksheet.Cells[row, 5] = "Arbeit";
            worksheet.Cells[row, 5].HorizontalAlignment = XlHAlign.xlHAlignCenter;
            worksheet.Cells[row, 6] = "Pause";
            worksheet.Cells[row, 6].HorizontalAlignment = XlHAlign.xlHAlignCenter;

            row++;

            // Einfügen der Zeiten in die Excel Tabelle
            List<Time> times = TimeDatabaseHelper.GetAllTimes(userId);
            TimeSpan sumTotalDuration = TimeSpan.Zero;
            TimeSpan sumPauseDuration = TimeSpan.Zero;
            foreach (Time time in times)
            {
                // Nur Zeiten des gewünschten Monats exportieren
                if (time.Start.Month == month && time.Start.Year == year)
                {
                    worksheet.Cells[row, 1] = time.Start.ToString("dd.MM.yyyy");
                    worksheet.Cells[row, 2] = time.Start.ToString("HH:mm:ss");
                    worksheet.Cells[row, 3] = time.End.ToString("dd.MM.yyyy");
                    worksheet.Cells[row, 4] = time.End.ToString("HH:mm:ss");
                    worksheet.Cells[row, 5] = time.TotalDuration.ToString();
                    worksheet.Cells[row, 6] = time.PauseDuration.ToString();
                    row++;
                    sumTotalDuration += time.TotalDuration;
                    sumPauseDuration += time.PauseDuration;
                }
            }

            row++;

            // Vorbereiten der Zeitenanzeige
            TimeSpan workDuration = sumTotalDuration - sumPauseDuration;
            worksheet.Cells[row, 1] = "Summe Arbeitszeit:";
            Microsoft.Office.Interop.Excel.Range range7 = worksheet.Range[worksheet.Cells[row, 1], worksheet.Cells[row, 4]];
            range7.Merge();
            range7.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            worksheet.Cells[row, 5] = sumTotalDuration.ToString();
            Microsoft.Office.Interop.Excel.Range range10 = worksheet.Range[worksheet.Cells[row, 5], worksheet.Cells[row, 6]];
            range10.Merge();
            range10.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            row++;
            worksheet.Cells[row, 1] = "Summe Pausenzeit:";
            Microsoft.Office.Interop.Excel.Range range8 = worksheet.Range[worksheet.Cells[row, 1], worksheet.Cells[row, 4]];
            range8.Merge();
            range8.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            worksheet.Cells[row, 5] = sumPauseDuration.ToString();
            Microsoft.Office.Interop.Excel.Range range11 = worksheet.Range[worksheet.Cells[row, 5], worksheet.Cells[row, 6]];
            range11.Merge();
            range11.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            row++;
            worksheet.Cells[row, 1] = "Tatsächliche Arbeitszeit:";
            worksheet.Cells[row, 1].Font.Bold = true;
            Microsoft.Office.Interop.Excel.Range range9 = worksheet.Range[worksheet.Cells[row, 1], worksheet.Cells[row, 4]];
            range9.Merge();
            range9.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            worksheet.Cells[row, 5] = workDuration.ToString();
            worksheet.Cells[row, 5].Font.Bold = true;
            Microsoft.Office.Interop.Excel.Range range12 = worksheet.Range[worksheet.Cells[row, 5], worksheet.Cells[row, 6]];
            range12.Merge();
            range12.HorizontalAlignment = XlHAlign.xlHAlignLeft;

            // Automatische Breite der Spalten
            worksheet.Columns.AutoFit();
            // Sichtbar machen der Excel Anwendung
            excelApp.Visible = true;
            excelApp.WindowState = XlWindowState.xlMinimized;
            excelApp.WindowState = XlWindowState.xlMaximized;
            // Freigen der Excel Andwendung
            Marshal.ReleaseComObject(excelApp);
        }

        public static void ExportCurrentMonthToExcel(int userId, string username)
        {
            ExportMonthToExcel(userId, username, DateTime.Now.Month, DateTime.Now.Year);
        }

        public static void ExportLastMonthToExcel(int userId, string username)
        {
            ExportMonthToExcel(userId, username, DateTime.Now.AddMonths(-1).Month, DateTime.Now.Year);
        }
    }
}
