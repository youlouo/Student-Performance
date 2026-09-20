using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClosedXML.Excel;
using System.IO;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Student_Performance
{
    public partial class StudentForm : Form
    {
        private SSQLRepository repository = new SSQLRepository();
        private readonly StudentService studentServ = new StudentService();
        private SubjectDetailStats statsSubjects = new SubjectDetailStats();
        private StudentStats stats = new StudentStats();
        private string group = "";
        public StudentForm()
        {
            InitializeComponent();
        }

        private void StudentForm_Load(object sender, EventArgs e)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            LoadStudentProfile();
            LoadCourseAndSemestr();
            GetSubjects();
        }
        //Загружаем профиль студента
        private void LoadStudentProfile()
        {
            int currentId = UserSession.CurrentUser.Id;
            try
            {
                StudentProfile profile = studentServ.GetStudentData(currentId);
                if (profile != null)
                {
                    label17.Text = profile.fullName;
                    label10.Text = profile.birthDate;
                    label11.Text = profile.male;
                    label12.Text = profile.contact;
                    label13.Text = profile.group;
                    label14.Text = profile.type;
                    label15.Text = profile.startDate;
                    label16.Text = profile.status;
                    label19.Text = profile.id;
                    group = profile.group;
                }
                else
                {
                    MessageBox.Show("Профиль не найден", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке профиля:\n{ex.Message}", "Ошибка СУБД", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCourseAndSemestr(){
            comboBox3.DataSource = repository.GetStudentCourse(group);
            comboBox3.SelectedIndex = -1;
            comboBox4.DataSource = repository.GetStudentSemestr(group);
            comboBox4.SelectedIndex = -1;
        }

        private void CourseAndSemesterSelected(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex != -1 && comboBox4.SelectedIndex != -1)
            {
                UpdateDashboard();
            }
        }

        private void UpdateDashboard()
        {
            if (comboBox3.SelectedIndex == -1 || comboBox4.SelectedIndex == -1)
                return;
            button4.Enabled = true;
            button5.Enabled = true;
            int course = Convert.ToInt32(comboBox3.SelectedItem);
            int semester = Convert.ToInt32(comboBox4.SelectedItem);
            stats = repository.GetStudentStats(label17.Text, course, semester);

            label48.Text = stats.AvgGrade.ToString("0.00");
            label49.Text = stats.RankInGroup.ToString();
            label50.Text = stats.MissedLessons.ToString();

            int ratingPercent = (int)Math.Min((stats.AvgGrade / 5.0m) * 100, 100);
            label27.Text = $"{ratingPercent}%";
            label22.Text = $"{Math.Round(stats.AttendanceRate)}%";
        }

        private void GetSubjectStatistics(object sender, EventArgs e)
        {
            if(comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите дисциплину!");
                return;
            }
            if (comboBox5.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите форму работы!");
                return;
            }

            button4.Enabled = true;
            button5.Enabled = true;
            string studentFio = label17.Text;
            string subjectName = comboBox2.SelectedItem.ToString();
            string workType = comboBox5.SelectedIndex != -1 ? comboBox5.SelectedItem.ToString() : null;
            DateTime? dateFrom = DateTime.TryParse(maskedTextBox3.Text, out DateTime dFrom) ? dFrom : (DateTime?)null;
            DateTime? dateTo = DateTime.TryParse(maskedTextBox2.Text, out DateTime dTo) ? dTo : (DateTime?)null;

            statsSubjects = repository.GetSubjectDetails(studentFio, subjectName, workType, dateFrom, dateTo);


            label43.Text = statsSubjects.TeacherFio;                          
            label44.Text = statsSubjects.AvgGrade.ToString("0.00");        
            label45.Text = $"{Math.Round(statsSubjects.AttendanceRate)}%";  
            label46.Text = statsSubjects.ControlForm;   
            label47.Text = statsSubjects.TeacherEmail;
            listBox1.Items.Clear();
            string[] lines = statsSubjects.Description.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                listBox1.Items.Add(line.Trim());
            }

        }

        private void GetSubjects()
        {
            comboBox2.DataSource = repository.GetStudentSubjects(label17.Text);
            comboBox2.SelectedIndex = -1;
            comboBox1.DataSource = repository.GetStudentSubjects(label17.Text);
            comboBox1.SelectedIndex = -1;

        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null) return;
            string selectedSubject = comboBox2.SelectedItem.ToString();
            comboBox5.DataSource = repository.GetStudentWorkTypes(label17.Text, selectedSubject); ;
            comboBox5.SelectedIndex = -1;
        }

        private void ShowGradesClick(object sender, EventArgs e)
        {
            ShowGrades();
        }

        private void ShowGrades()
        {
            if (string.IsNullOrWhiteSpace(comboBox1.Text) || comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Необходимо ввести дисциплину", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string fio = label17.Text;
            DateTime? date = DateTime.TryParse(maskedTextBox1.Text, out DateTime parsedDate) ? parsedDate : (DateTime?)null;
            string subject = comboBox1.SelectedIndex != -1 ? comboBox1.Text : null;
            string type = SelectedType();

            DataTable data = repository.GetStudentFilterData(fio, subject, date, type);
            dataGridView1.DataSource = data;
        }

        private string SelectedType()
        {
            if (radioButton1.Checked) return "Лабораторная работа";
            if (radioButton2.Checked) return "Лекция";
            if (radioButton3.Checked) return "Практика";
            if (radioButton6.Checked) return "Курсовая работа";
            if (radioButton5.Checked) return "Зачет";
            if (radioButton4.Checked) return "Проектная работа";
            return null;
        }

        private void LogOutClick(object sender, EventArgs e)
        {
            this.Hide();
            UserSession.Logout();
            Form1 form = new Form1();
            form.FormClosed += (s, args) => this.Close();
            form.Show();
        }

        private void ExportToExcel(StudentStats generalStats, SubjectDetailStats subjectStats, string studentFio)
        {
            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "Excel Workbook|*.xlsx", FileName = $"Статистика_{studentFio}.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var ws = workbook.Worksheets.Add("Статистика");


                        ws.Cell("A1").Value = $"Отчет по успеваемости: {studentFio}";
                        ws.Cell("A1").Style.Font.Bold = true;
                        ws.Cell("A1").Style.Font.FontSize = 14;

                        ws.Cell("A3").Value = "ОБЩАЯ СТАТИСТИКА";
                        ws.Cell("A3").Style.Font.Bold = true;

                        ws.Cell("A4").Value = "Общий средний балл:";
                        ws.Cell("B4").Value = generalStats.AvgGrade;

                        ws.Cell("A5").Value = "Место в рейтинге группы:";
                        ws.Cell("B5").Value = generalStats.RankInGroup;

                        ws.Cell("A6").Value = "Пропущенные занятия:";
                        ws.Cell("B6").Value = generalStats.MissedLessons;

                        ws.Cell("A7").Value = "Посещаемость (%):";
                        ws.Cell("B7").Value = generalStats.AttendanceRate;

                        if (subjectStats != null)
                        {
                            ws.Cell("A9").Value = "СТАТИСТИКА ПО ДИСЦИПЛИНЕ";
                            ws.Cell("A9").Style.Font.Bold = true;

                            ws.Cell("A10").Value = "Дисциплина:";
                            ws.Cell("B10").Value = comboBox2.SelectedItem.ToString();

                            ws.Cell("A11").Value = "Преподаватель:";
                            ws.Cell("B11").Value = subjectStats.TeacherFio;

                            ws.Cell("A12").Value = "Средний балл по предмету:";
                            ws.Cell("B12").Value = subjectStats.AvgGrade;

                            ws.Cell("A13").Value = "Посещаемость по предмету (%):";
                            ws.Cell("B13").Value = subjectStats.AttendanceRate;

                            ws.Cell("A14").Value = "Форма контроля:";
                            ws.Cell("B14").Value = subjectStats.ControlForm;

                            ws.Cell("A15").Value = "Почта преподавателя:";
                            ws.Cell("B15").Value = subjectStats.TeacherEmail;
                        }

                        ws.Columns().AdjustToContents();
                        workbook.SaveAs(sfd.FileName);
                        MessageBox.Show("Отчет успешно сохранен в Excel!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void ExportToPdf(StudentStats generalStats, SubjectDetailStats subjectStats, string studentFio)
        {
            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "PDF Document|*.pdf", FileName = $"Статистика_{studentFio}.pdf" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4);
                            page.Margin(2, Unit.Centimetre);
                            page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Lato"));

                            page.Header().Text($"Отчет по успеваемости студента: {studentFio}")
                                .SemiBold().FontSize(16).FontColor(Colors.Purple.Medium);

                            page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                            {
                                col.Spacing(10);

                                col.Item().Text("Общая статистика").Bold().FontSize(13);
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Cell().Text("Общий средний балл:");
                                    table.Cell().Text(generalStats.AvgGrade.ToString("0.00"));

                                    table.Cell().Text("Место в рейтинге:");
                                    table.Cell().Text(generalStats.RankInGroup.ToString());

                                    table.Cell().Text("Пропущенные занятия:");
                                    table.Cell().Text(generalStats.MissedLessons.ToString());

                                    table.Cell().Text("Общая посещаемость:");
                                    table.Cell().Text($"{generalStats.AttendanceRate}%");
                                });

                                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                                if (subjectStats != null)
                                {
                                    col.Item().Text("Статистика по дисциплине").Bold().FontSize(13);
                                    col.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                        });

                                        table.Cell().Text("Предмет:");
                                        table.Cell().Text(comboBox2.SelectedItem);

                                        table.Cell().Text("Преподаватель:");
                                        table.Cell().Text(subjectStats.TeacherFio);

                                        table.Cell().Text("Средний балл:");
                                        table.Cell().Text(subjectStats.AvgGrade.ToString("0.00"));

                                        table.Cell().Text("Посещаемость:");
                                        table.Cell().Text($"{subjectStats.AttendanceRate}%");

                                        table.Cell().Text("Форма контроля:");
                                        table.Cell().Text(subjectStats.ControlForm);

                                        table.Cell().Text("Контакты:");
                                        table.Cell().Text(subjectStats.TeacherEmail);
                                    });
                                }
                            });

                            page.Footer().AlignCenter().Text(x =>
                            {
                                x.CurrentPageNumber();
                                x.Span(" / ");
                                x.TotalPages();
                            });
                        });
                    }).GeneratePdf(sfd.FileName);

                    MessageBox.Show("Отчет успешно сохранен в PDF!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ExportToExcelClick(object sender, EventArgs e)
        {
            ExportToExcel(stats, statsSubjects, label17.Text);
        }

        private void ExportToPdfClick(object sender, EventArgs e)
        {
            ExportToPdf(stats, statsSubjects, label17.Text);
        }
    }
}
