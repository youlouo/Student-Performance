using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace Student_Performance
{
    public partial class HelpViewerForm : Form
    {
        private readonly string _pdfFileName;

        public HelpViewerForm(string pdfFileName, string windowTitle = "Справочное руководство")
        {
            InitializeComponent();
            _pdfFileName = pdfFileName;
            this.Text = windowTitle;

            // Настройка окна
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 900;
            this.Height = 700;

            // Инициализация загрузки PDF
            this.Load += HelpViewerForm_Load;
        }

        private async void HelpViewerForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Путь к папке Docs рядом с исполняемым .exe файлом
                string pdfPath = Path.Combine(Application.StartupPath, "Docs", _pdfFileName);

                if (!File.Exists(pdfPath))
                {
                    MessageBox.Show($"Файл руководства не найден по пути:\n{pdfPath}\n\nУбедитесь, что файлы PDF находятся в папке 'Docs'.",
                                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Инициализируем движок WebView2
                await webView21.EnsureCoreWebView2Async(null);

                // Загружаем PDF документ
                webView21.Source = new Uri(pdfPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии руководства:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}