using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace proiz
{
    public partial class TimeT : Form
    {

        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public static class UserInfo
        {
            public static string CurrentUsername { get; set; }
            public static int CurrentEmployeeId { get; set; }
        }

        MySqlConnection conn = new MySqlConnection(Daniel.Twenty());
        private int userId;
        private string username;
        public TimeT(int userId, string username)
        {
            InitializeComponent();
            dataGridView1.RowHeadersVisible = false;
            this.userId = userId;
            this.username = username;
            UserInfo.CurrentEmployeeId = userId;
            UserInfo.CurrentUsername = username;
            Table();
        }
        // Метод для получения текущего ID сотрудника
        public int GetCurrentEmployeeId()
        {
            return UserInfo.CurrentEmployeeId;
        }

        // Метод для заполнения таблицы данными
        public void Table()
        {
            // Получение текущего ID сотрудника
            // Получение текущего ID сотрудника
            int currentEmployeeId = GetCurrentEmployeeId();

            // SQL запрос для получения информации о рабочем времени
            string sql = $@"
                SELECT 
                    timet.id_t, 
                    timet.employee_id_t as 'Сотрудник', 
                    timet.date_t as 'Дата рабочего времени', 
                    timet.clock_in_time_t as 'Начало работы', 
                    timet.clock_out_time_t as 'Окончание работы', 
                    TIMEDIFF(timet.clock_out_time_t, timet.clock_in_time_t) as 'Время потрачено',
                    timet.desk_t as 'Описание услуги'
                FROM timet
                INNER JOIN auth ON timet.employee_id_t = auth.id_user
                WHERE timet.employee_id_t = @userId";

            try
            {
                // Открытие соединения с базой данных
                conn.Open();

                // Создание команды SQL с подстановкой параметра
                MySqlCommand command = new MySqlCommand(sql, conn);
                command.Parameters.AddWithValue("@userId", currentEmployeeId);

                // Выполнение команды и получение данных
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataSet dataset = new DataSet();
                dataAdapter.Fill(dataset);
                dataGridView1.DataSource = dataset.Tables[0];


                // Настройка автоматического изменения размера столбцов
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                // Расчет разницы во времени и обновление столбца "Время потрачено"
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    DataGridViewCell cell = row.Cells["Время потрачено"];
                    if (cell.Value != null)
                    {
                        TimeSpan timeSpent;
                        if (cell.Value is TimeSpan)
                        {
                            timeSpent = (TimeSpan)cell.Value;
                        }
                        else
                        {
                            if (TimeSpan.TryParse(cell.Value.ToString(), out timeSpent))
                            {
                                cell.Value = timeSpent;
                            }
                            else
                            {
                                cell.Value = TimeSpan.Zero;
                            }
                        }

                        // Обновление колонки TOTAL_hours_t в базе данных
                        string updateSql = "UPDATE timet SET `total_hours_t` = ADDTIME(`total_hours_t`, @timeSpent) WHERE id_t = @id";
                        MySqlCommand updateCommand = new MySqlCommand(updateSql, conn);
                        updateCommand.Parameters.AddWithValue("@timeSpent", timeSpent.ToString(@"hh\:mm\:ss"));
                        updateCommand.Parameters.AddWithValue("@id", row.Cells["id_t"].Value);
                        updateCommand.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                // Закрытие соединения с базой данных
                conn.Close();
            }
        }


        private void TimeT_Load(object sender, EventArgs e)
        {
          
            username = UserInfo.CurrentUsername; 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Скрываем текущую форму
            this.Hide();
            // Показываем первую форму
            Menu menu = new Menu(userId, username);
            menu.Show();


        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

      

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            try
            {
                string filter = textBox1.Text.Trim().ToLower();
                // Отключаем CurrencyManager
                ((CurrencyManager)this.BindingContext[dataGridView1.DataSource]).SuspendBinding();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    // Проверяем, является ли строка новой
                    if (row.IsNewRow)
                    {
                        continue; // Пропускаем новую строку
                    }

                    bool found = false;

                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null && cell.Value.ToString().ToLower().Contains(filter))
                        {
                            found = true;
                            break;
                        }
                    }

                    row.Visible = found;
                }
    // Включаем CurrencyManager
    ((CurrencyManager)this.BindingContext[dataGridView1.DataSource]).ResumeBinding();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Table();
        }

        private void button5_Click(object sender, EventArgs e)
        {

            try
            {
                if (dataGridView1.SelectedCells.Count > 0)
                {
                    int selectedId = Convert.ToInt32(dataGridView1.SelectedCells[0].OwningRow.Cells["id_t"].Value);
                    conn.Open();
                    string updateQuery = $"UPDATE timet SET clock_in_time_t = @CurrentTime WHERE id_t = @ID";
                    MySqlCommand command = new MySqlCommand(updateQuery, conn);
                    command.Parameters.AddWithValue("@ID", selectedId);
                    command.Parameters.AddWithValue("@CurrentTime", DateTime.Now.TimeOfDay);
                    command.ExecuteNonQuery();
                    conn.Close();
                    Table();
                }
                else
                {
                    MessageBox.Show("Выберите ячейку для обновления");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedCells.Count > 0) // Убедитесь, что есть выбранные ячейки
                {
                    int selectedId = Convert.ToInt32(dataGridView1.SelectedCells[0].OwningRow.Cells["id_t"].Value); // Получите строку, которой принадлежит выбранная ячейка

                    conn.Open();

                    string updateQuery = $"UPDATE timet SET clock_out_time_t = @CurrentTime WHERE id_t = @ID";
                    MySqlCommand command = new MySqlCommand(updateQuery, conn);
                    command.Parameters.AddWithValue("@ID", selectedId);

                    // Добавьте текущее время как параметр запроса
                    command.Parameters.AddWithValue("@CurrentTime", DateTime.Now.TimeOfDay);
                    command.ExecuteNonQuery();
                    conn.Close();

                    Table(); // Обновление таблицы после обновления значения
                }
                else
                {
                    MessageBox.Show("Выберите ячейку для обновления");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            AddTime add = new AddTime();
            add.Show();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
    }
}
