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
    public partial class infoclient : Form
    {
        MySqlConnection conn = new MySqlConnection(Daniel.Twenty());

        private int userId;
        private string username;
        public infoclient(int userId, string username)
        {
            InitializeComponent();
            this.userId = userId;
            this.username = username;
        }

        private void infoclient_Load(object sender, EventArgs e)
        {
            try
            {
                // откройте соединение с базой данных
                conn.Open();

                // создайте команду запроса
                string sql = $"SELECT fio_c, telephon_c, service_с, app_date_c FROM client WHERE id_c = @userId";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                // выполните запрос и получите результат
                MySqlDataReader reader = cmd.ExecuteReader();

                // если результат существует, установите его в соответствующие метки
                if (reader.Read())
                {
                    label5.Text = reader["fio_c"].ToString();
                    label6.Text = reader["telephon_c"].ToString();
                    label7.Text = reader["service_с"].ToString();
                    label8.Text = reader["app_date_c"].ToString();
                }
            }
            catch (Exception ex)
            {
                // обработайте любые ошибки, которые могут возникнуть
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // всегда закрывайте соединение после завершения работы с базой данных
                conn.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Question q = new Question(userId, username);
            q.Show();
        }
    }
}
