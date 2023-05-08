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
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace proiz
{
    public partial class Reg : Form
    {
        readonly MySqlConnection conn = new MySqlConnection(Daniel.Twenty());

        static string sha256(string pass)
        {
            //ХЭШ пароля
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(pass));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        bool vis = true;
        public Reg()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            textBox3.Text = "Введите имя";
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (vis)
            {
                textBox2.UseSystemPasswordChar = false;
                vis = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
                vis = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "Имя")
            {
                MessageBox.Show("Введите имя");
                return;
            }

            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(Daniel.Twenty());
                conn.Open();

                var login = textBox1.Text;
                var Pass = Sha256.HasPass(textBox2.Text);
                var Pass1 = textBox2.Text;
                var Name = textBox3.Text;

                string querystring = "insert into Auth(login_user, password, info_user, role_user, pass_user) values(@login, @pass, @name, 3, @pass1)";

                MySqlCommand command = new MySqlCommand(querystring, conn);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@pass", Pass);
                command.Parameters.AddWithValue("@name", Name);
                command.Parameters.AddWithValue("@pass1", Pass1);

                if (!IsLoginUnique(conn, login))
                {
                    MessageBox.Show("Логин уже занят. Пожалуйста, выберите другой логин.");
                    return;
                }

                bool result = command.ExecuteNonQuery() == 1;
                if (result)
                {
                    MessageBox.Show("Аккаунт успешно создан");
                    Form1 Auth1 = new Form1();
                    Auth1.Show();
                    this.Hide();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }

        private bool IsLoginUnique(MySqlConnection conn, string login)
        {

            try
            {
                string query = "SELECT COUNT(*) FROM auth WHERE login_user = @login";

                MySqlCommand command = new MySqlCommand(query, conn);

                command.Parameters.AddWithValue("@login", login);

                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count > 0)
                {
                    return false;
                }

                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
                return false;
            }

        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            if  (textBox3.Text == "Введите имя")
            {
                textBox3.Text = "";
            }
        }

      


        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                textBox3.Text = "Введите имя";
            }
        }

    }
}
