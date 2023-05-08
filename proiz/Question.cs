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
    public partial class Question : Form
    {

        MySqlConnection conn = new MySqlConnection(Daniel.Twenty());
        private int userId;
        private string username;

        public Question(int userId, string username)
        {
            InitializeComponent();
            this.userId = userId;
            this.username = username;
        }

        private void Question_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string userQuestion = textBox1.Text;

            string sql = "INSERT INTO report (question_r, id_qauthor_r) VALUES (@question, @id)";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@question", userQuestion);
                cmd.Parameters.AddWithValue("@id", userId);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Все успешно");
                    // Очищаем textBox1 после успешной отправки данных
                    textBox1.Text = "";
                }
                catch (Exception ex)
                {
                    // Обрабатываем исключение здесь
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
