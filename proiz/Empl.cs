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
    public partial class Empl : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        MySqlDataAdapter IDataAdapter;
        DataTable table;

        MySqlConnection conn = new MySqlConnection(Daniel.Twenty());
        private int userId;
        private string username;

        public static class UserInfo
        {
            public static string CurrentUsername { get; set; }
            public static int CurrentEmployeeId { get; set; }
        }

        public Empl(int userId, string username)
        {
            InitializeComponent();
            dataGridView1.RowHeadersVisible = false;
            this.userId = userId;
            this.username = username;
            UserInfo.CurrentEmployeeId = userId;
            UserInfo.CurrentUsername = username;
            Table();
        }

        private void Empl_Load(object sender, EventArgs e)
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

        public int GetCurrentEmployeeId()
        {
            return UserInfo.CurrentEmployeeId;
        }

        public void Table()
        {

            // Получение текущего ID сотрудника
            int currentEmployeeId = GetCurrentEmployeeId();

            // SQL запрос для получения информации о рабочем времени
            string sql = $"SELECT id_s as 'Номер', name_s as 'Имя', surname_s as 'Фамилия', lastname_s as 'Отчетство', dataR_s as 'Дата рождение', dataAccept_s as 'Принятие на работу', doljnost_s as 'Должность'  FROM sotrudniki";

                try
                {
                    // Создание команды SQL с подстановкой параметра
                    MySqlCommand command = new MySqlCommand(sql, conn);
                    command.Parameters.AddWithValue("@userId", currentEmployeeId);
                // Выполнение команды и получение данных
                conn.Open();
                IDataAdapter = new MySqlDataAdapter(sql, conn);
                table = new DataTable();
                IDataAdapter.Fill(table);
                dataGridView1.DataSource = table;

                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridView1.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


                }
                catch
                {

                }
                finally
                {
                    conn.Close();

                }
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Скрываем текущую форму
            this.Hide();
            // Показываем первую форму
            Menu menu = new Menu(userId, username);
            menu.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = dataGridView1.CurrentRow;

            if (selectedRow != null && !selectedRow.IsNewRow)
            {
                string idToDelete = selectedRow.Cells[0].Value.ToString();

                string sql = $"DELETE FROM sotrudniki WHERE id_s = @id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", idToDelete);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    
                }
                finally
                {
                    conn.Close();
                }

                dataGridView1.Rows.Remove(selectedRow);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Table();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();

                // Создаем объект MySqlCommandBuilder, который будет автоматически генерировать SQL-команды для обновления базы данных
                MySqlCommandBuilder mySqlCommandBuilder = new MySqlCommandBuilder(IDataAdapter);

                // Обновляем базу данных
                IDataAdapter.Update(table);
            }
            catch (Exception ex)
            {
                // Обрабатываем ошибки здесь
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
