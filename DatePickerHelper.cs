using System;
using System.Collections.Generic;
using System.Text;

namespace Student_Performance
{
    internal class DatePickerHelper
    {
        public static void ShowCalendarPopup(Button button, MaskedTextBox targetMaskedTextBox)
        {
            // Создаем всплывающую форму без рамок
            Form popup = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                TopMost = true,
                Size = new Size(165, 162)
            };

            // Создаем контрол календаря
            MonthCalendar calendar = new MonthCalendar
            {
                MaxSelectionCount = 1,
                Dock = DockStyle.Fill
            };

            // Если в MaskedTextBox уже введена корректная дата, открываем календарь на ней
            if (DateTime.TryParse(targetMaskedTextBox.Text, out DateTime parsedDate))
            {
                calendar.SelectionStart = parsedDate;
            }

            // Обработка выбора даты
            calendar.DateSelected += (s, e) =>
            {
                targetMaskedTextBox.Text = calendar.SelectionStart.ToString("dd.MM.yyyy");
                popup.Close();
            };

            // Закрываем календарь, если пользователь кликнул мимо
            popup.Deactivate += (s, e) => popup.Close();

            popup.Controls.Add(calendar);

            // Вычисляем экранные координаты прямо под кнопкой
            Point btnLocationOnScreen = button.PointToScreen(Point.Empty);
            popup.Location = new Point(btnLocationOnScreen.X, btnLocationOnScreen.Y + button.Height);

            popup.Show();
        }
    }
}
