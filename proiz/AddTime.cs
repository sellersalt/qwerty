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
    public partial class AddTime : Form
    {
        MySqlConnection conn = new MySqlConnection(Daniel.Twenty());
        public AddTime()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int employeeId = Int32.Parse(textBox1.Text);
                DateTime date = DateTime.Parse(maskedTextBox1.Text);

                string query = "INSERT INTO timet (employee_id_t, date_t, clock_in_time_t, clock_out_time_t, total_hours_t) " +
                               "VALUES (@employeeId, @date, @clockInTime, @clockOutTime, @totalHours)";

                conn.Open();

                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@employeeId", employeeId);
                    command.Parameters.AddWithValue("@date", date);
                    command.Parameters.AddWithValue("@clockInTime", TimeSpan.Zero);
                    command.Parameters.AddWithValue("@clockOutTime", TimeSpan.Zero);
                    command.Parameters.AddWithValue("@totalHours", 0.0);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Данные успешно добавлены. Пожалуйста, обновите таблицу.");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось добавить данные.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

        }

      
    }
    
}
