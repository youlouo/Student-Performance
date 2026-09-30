using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Student_Performance
{
    internal class ReportExporter
    {
        public static void ExportToExcel(DataTable dt, string reportTitle, Dictionary<string, string> summaryMetrics, string defaultFileName = "Отчет.xlsx")
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("Сначала сформируйте отчет!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "Excel Workbook|*.xlsx", FileName = defaultFileName })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var ws = workbook.Worksheets.Add("Отчет");

                        // 1. Заголовок отчета
                        ws.Cell(1, 1).Value = reportTitle.ToUpper();
                        ws.Cell(1, 1).Style.Font.Bold = true;
                        ws.Cell(1, 1).Style.Font.FontSize = 14;

                        // 2. Вставка таблицы с данными
                        ws.Cell(3, 1).InsertTable(dt);

                        // 3. Динамический вывод показателей подвала
                        int currentRow = dt.Rows.Count + 5;
                        int col = 1;

                        foreach (var metric in summaryMetrics)
                        {
                            ws.Cell(currentRow, col).Value = $"{metric.Key}: {metric.Value}";
                            ws.Cell(currentRow, col).Style.Font.Bold = true;

                            // Размещаем показатели в 2 колонки
                            if (col == 1)
                            {
                                col = 3;
                            }
                            else
                            {
                                col = 1;
                                currentRow++;
                            }
                        }

                        ws.Columns().AdjustToContents();
                        workbook.SaveAs(sfd.FileName);
                        MessageBox.Show("Отчет успешно сохранен в Excel!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        public static void ExportToPdf(DataTable dt, string reportTitle, Dictionary<string, string> summaryMetrics, string defaultFileName = "Отчет.pdf")
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("Сначала сформируйте отчет!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "PDF Document|*.pdf", FileName = defaultFileName })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    QuestPDF.Fluent.Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4.Landscape());
                            page.Margin(1.5f, Unit.Centimetre);
                            page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Lato"));

                            // Header
                            page.Header().Text(reportTitle)
                                .Bold().FontSize(14).FontColor(Colors.Purple.Medium);

                            // Content
                            page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                            {
                                col.Spacing(10);

                                // Построение таблицы
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        for (int i = 0; i < dt.Columns.Count; i++)
                                            columns.RelativeColumn();
                                    });

                                    // Заголовки столбцов
                                    foreach (DataColumn column in dt.Columns)
                                    {
                                        table.Cell().Background(Colors.Grey.Lighten2)
                                             .Padding(5).Text(column.ColumnName).Bold();
                                    }

                                    // Заполнение данных
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        foreach (var item in row.ItemArray)
                                        {
                                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1)
                                                 .Padding(4).Text(item?.ToString() ?? "—");
                                        }
                                    }
                                });

                                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);

                                // Подвал (метрики)
                                col.Item().Table(summaryTable =>
                                {
                                    summaryTable.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                                    foreach (var metric in summaryMetrics)
                                    {
                                        summaryTable.Cell().Text($"{metric.Key}: {metric.Value}").Bold();
                                    }
                                });
                            });

                            // Footer
                            page.Footer().AlignCenter().Text(x => { x.CurrentPageNumber(); x.Span(" / "); x.TotalPages(); });
                        });
                    }).GeneratePdf(sfd.FileName);

                    MessageBox.Show("Отчет успешно сохранен в PDF!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
