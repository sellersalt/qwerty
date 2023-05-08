using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace proiz
{
    public partial class report : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        MySqlConnection conn = new MySqlConnection(Daniel.Twenty());
        private int userId;
        private string username;

        public static class UserInfo
        {
            public static string CurrentUsername { get; set; }
            public static int CurrentEmployeeId { get; set; }
        }

        private void FetchUserHours()
        {
            string queryString =
                "SELECT SUM(TIME_TO_SEC(timet.total_hours_t)) " +
                "FROM auth JOIN timet ON auth.id_user = timet.employee_id_t " +
                "WHERE auth.id_user = @id";

            using (MySqlCommand command = new MySqlCommand(queryString, conn))
            {
                command.Parameters.AddWithValue("@id", userId);

                try
                {
                    conn.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                int totalSeconds = reader.GetInt32(0);

                                // Читаем текущее значение из label6 и пытаемся преобразовать его обратно в TimeSpan
                                TimeSpan currentTotalTime;
                                try
                                {
                                    currentTotalTime = TimeSpan.ParseExact(label6.Text, @"hh\:mm\:ss", CultureInfo.InvariantCulture);
                                }
                                catch (FormatException)
                                {
                                    // Если label6.Text не может быть преобразовано в TimeSpan, то устанавливаем его значение в "00:00:00"
                                    currentTotalTime = TimeSpan.Zero;
                                    label6.Text = "00:00:00";
                                }

                                // Добавляем новые секунды к текущему общему времени
                                TimeSpan newTotalTime = currentTotalTime.Add(TimeSpan.FromSeconds(totalSeconds));

                                label6.Text = newTotalTime.ToString(@"hh\:mm\:ss");
                            }
                            else
                            {
                                // Если label6 пуст, устанавливаем его значение в "00:00:00"
                                if (String.IsNullOrWhiteSpace(label6.Text))
                                {
                                    label6.Text = "00:00:00";
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void FetchTotalMoney()
        {
            string queryString =
                "SELECT SUM(money_pms) " +
                "FROM `pms`";

            using (MySqlCommand command = new MySqlCommand(queryString, conn))
            {
                try
                {
                    conn.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                decimal totalMoney = reader.GetDecimal(0);
                                label1.Text = totalMoney.ToString("0.00");  // Форматируем как десятичное число с двумя знаками после запятой
                            }
                            else
                            {
                                label1.Text = "0.00"; // Выбираем значение по умолчанию, когда данных нет
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void FetchTotalIdPms()
        {
            string queryString = "SELECT COUNT(id_pms) FROM `pms`";

            using (MySqlCommand command = new MySqlCommand(queryString, conn))
            {
                try
                {
                    conn.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                int count = reader.GetInt32(0);
                                label7.Text = count.ToString();
                            }
                            else
                            {
                                label7.Text = "0"; // Выбираем значение по умолчанию, когда данных нет
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void FetchTotalHours()
        {
            string queryString =
                "SELECT SEC_TO_TIME(SUM(TIME_TO_SEC(total_hours_t))) " +
                "FROM `timet`";

            using (MySqlCommand command = new MySqlCommand(queryString, conn))
            {
                try
                {
                    conn.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                TimeSpan totalTime = TimeSpan.Parse(reader.GetString(0));
                                label4.Text = totalTime.ToString(@"hh\:mm\:ss");  // Форматируем как время в формате hh:mm:ss
                            }
                            else
                            {
                                label4.Text = "00:00:00"; // Выбираем значение по умолчанию, когда данных нет
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public report(int userId, string username)
        {
            InitializeComponent();
            this.userId = userId;
            this.username = username;
            UserInfo.CurrentEmployeeId = userId;
            UserInfo.CurrentUsername = username;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void report_Load(object sender, EventArgs e)
        {
            FetchUserHours();
            FetchTotalMoney();
            FetchTotalIdPms();
            FetchTotalHours();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Скрываем текущую форму
            this.Hide();
            // Показываем первую форму
            Menu menu = new Menu(userId, username);
            menu.Show();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
