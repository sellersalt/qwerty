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
    public partial class PMS : Form
    {
        MySqlConnection conn = new MySqlConnection(Daniel.Twenty());

        private int userId;
        private string username;
        private int userRole;

        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        MySqlDataAdapter IDataAdapter;
        DataTable table;

        public PMS(int userId, string username)
        {
            InitializeComponent();
            dataGridView1.RowHeadersVisible = false;
            this.userId = userId;
            this.username = username;

            string sql = $"SELECT role_user FROM auth WHERE id_user = @userId";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@userId", userId);

            try
            {
                conn.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    userRole = Convert.ToInt32(reader["role_user"]);
                }
            }
            catch (Exception ex)
            {
             
            }
            finally
            {
                conn.Close();
            }
            Table();
        }

        public void Table()
        { 
            string sql = $"SELECT id_pms as 'Номер', project_pms as 'Проект', sroki_start_pms as 'Начало выполнение', otvest_pms as 'Отвественные за работу', money_pms as 'Бюджет', comand_pms as 'Рабочие', desk_pms as 'Описание работы', sroki_end_pms as 'Конец выполнение' FROM pms";

            try
            {
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

        private void PMS_Load(object sender, EventArgs e)
        {
            dataGridView1.RowHeadersVisible = false;
            Table();
            if (userRole == 2)
            {
                button6.Visible = false;
                button7.Visible = false;
                button5.Visible = false;
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

        private void button6_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = dataGridView1.CurrentRow;

            if (selectedRow != null && !selectedRow.IsNewRow)
            {
                string idToDelete = selectedRow.Cells[0].Value.ToString();

                string sql = $"DELETE FROM pms WHERE id_pms = @id";
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

        private void button7_Click(object sender, EventArgs e)
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

        private void button4_Click(object sender, EventArgs e)
        {
            Table();
        }

        private void button8_Click(object sender, EventArgs e)
        {

            Question q = new Question(userId, username);
            q.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Получаем новые строки из DataGridView
            DataTable newRows = table.GetChanges(DataRowState.Added);

            if (newRows != null)
            {
                foreach (DataRow row in newRows.Rows)
                {
                    string sql = $"INSERT INTO pms (id_pms, project_pms, sroki_start_pms, otvest_pms, money_pms, comand_pms, desk_pms, sroki_end_pms) VALUES (@id, @project, @start, @responsible, @budget, @workers, @description, @end)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", row["Номер"]);
                    cmd.Parameters.AddWithValue("@project", row["Проект"]);
                    cmd.Parameters.AddWithValue("@start", row["Начало выполнение"]);
                    cmd.Parameters.AddWithValue("@responsible", row["Отвественные за работу"]);
                    cmd.Parameters.AddWithValue("@budget", row["Бюджет"]);
                    cmd.Parameters.AddWithValue("@workers", row["Рабочие"]);
                    cmd.Parameters.AddWithValue("@description", row["Описание работы"]);
                    cmd.Parameters.AddWithValue("@end", row["Конец выполнение"]);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
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

            // Обновляем DataTable чтобы отражать новые строки в базе данных
            table.AcceptChanges();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
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
