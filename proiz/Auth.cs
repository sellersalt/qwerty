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

namespace proiz
{
    public partial class Form1 : Form
    {

        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        bool vis = true;

        MySqlConnection conn = new MySqlConnection(Daniel.Twenty());

        static string sha256(string randomString)
        {
            //Тут происходит криптографическая магия. Смысл данного метода заключается в том, что строка залетает в метод
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }



        private void label2_Click(object sender, EventArgs e)
        {
            Reg Regis = new Reg();
            Regis.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
         public void GetUserInfo(string login_user)
        {
            //Объявлем переменную для запроса в БД
            string id_user = textBox1.Text;
            // устанавливаем соединение с БД
            conn.Open();
            // запрос
            string sql = $"SELECT * FROM Auth WHERE info_user='{login_user}'";
            // объект для выполнения SQL-запроса
            MySqlCommand command = new MySqlCommand(sql, conn);
            // объект для чтения ответа сервера
            MySqlDataReader reader = command.ExecuteReader();
            // читаем результат
            while (reader.Read())
            {
                // элементы массива [] - это значения столбцов из запроса SELECT
                Auth.auth_id = reader[0].ToString();
                Auth.auth_fio = reader[1].ToString();
                Auth.auth_role = Convert.ToInt32(reader[4].ToString());
            }
            reader.Close(); // закрываем reader
            // закрываем соединение с БД
            conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = "SELECT * FROM Auth WHERE login_user = @un and password= @up";
            conn.Open();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand(sql, conn);
            command.Parameters.Add("@un", MySqlDbType.VarChar, 25);
            command.Parameters.Add("@up", MySqlDbType.VarChar, 25);
            command.Parameters["@un"].Value = textBox1.Text;
            command.Parameters["@up"].Value = sha256(textBox2.Text);
            adapter.SelectCommand = command;
            adapter.Fill(table);
            conn.Close();

            if (table.Rows.Count > 0)
            {
                DataRow userRow = table.Rows[0];
                string role = userRow["role_user"].ToString();
                int employeeId = Convert.ToInt32(userRow["id_user"]);
                string username = userRow["info_user"].ToString(); // Извлекаем значение info_user

                // Передаем userId и username при создании нового экземпляра Menu
                switch (role)
                {
                    case "3":
                        MenuGuest guestForm = new MenuGuest(employeeId, username);
                        guestForm.Show();
                        break;
                    case "2":
                        Menu menu = new Menu(employeeId, username);
                        menu.Show();
                        break;
                    case "1":
                        Menu Main = new Menu(employeeId, username);
                        Main.Show();
                        break;
                }
                Auth.auth = true;
                GetUserInfo(textBox1.Text);
                this.Hide();
            }
            else
            {
                MessageBox.Show("Неверные данные авторизации!");
            }

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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

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
