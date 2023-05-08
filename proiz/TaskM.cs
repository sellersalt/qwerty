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
    public partial class TaskM : Form
    {
        private int userId;
        private string username;

        DataTable table;
        MySqlDataAdapter IDataAdapter;

        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;


        MySqlConnection conn = new MySqlConnection(Daniel.Twenty());

        public TaskM(int userId, string username)
        {
            InitializeComponent();
            this.userId = userId;
            this.username = username;

        }
        public void Table()
        {
            string sql = $"SELECT id_up as 'Номер', desk_up as 'Описание', data_up as 'Дата выполение', task_up as 'Выполнение', prior_up as 'Приоритет', otvest_up as 'Ответственный', zakaz_up as 'Заказчик'  FROM upravlenie";

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
        private void TaskM_Load(object sender, EventArgs e)
        {
            dataGridView1.RowHeadersVisible = false;
            Table();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {


            if (this.dataGridView1.Columns[e.ColumnIndex].Name == "Приоритет")
            {
                if (e.Value != null && e.Value.ToString() == "активно")
                {
                    e.CellStyle.BackColor = Color.Green;
                }
                else if (e.Value != null && e.Value.ToString() == "неактивно")
                {
                    e.CellStyle.BackColor = Color.Red;
                }
            }

        }


        private void button3_Click(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedCells.Count > 0)
            {
                // Открываем соединение
                conn.Open();

                DataGridViewCell selectedCell = dataGridView1.SelectedCells[0];
                DataGridViewRow selectedRow = selectedCell.OwningRow;
                int rowId = Convert.ToInt32(selectedRow.Cells["Номер"].Value);

                if (selectedCell.Value.ToString() == "неактивно")
                {
                    selectedCell.Value = "активно";

                    // Создаем запрос к базе данных для обновления
                    string updateQuery = "UPDATE upravlenie SET prior_up = 'активно' WHERE id_up = @id";
                    MySqlCommand command = new MySqlCommand(updateQuery, conn);
                    command.Parameters.AddWithValue("@id", rowId);
                    command.ExecuteNonQuery();
                }
                else
                {
                    selectedCell.Value = "неактивно";

                    // Создаем запрос к базе данных для обновления
                    string updateQuery = "UPDATE upravlenie SET prior_up = 'неактивно' WHERE id_up = @id";

                    // Создаем команду и устанавливаем соединение
                    MySqlCommand command = new MySqlCommand(updateQuery, conn);
                    command.Parameters.AddWithValue("@id", rowId);

                    // Выполняем запрос к базе данных
                    command.ExecuteNonQuery();
                }

                // Закрываем соединение
                conn.Close();
            }

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
            // Скрываем текущую форму
            this.Hide();
            // Показываем первую форму
            Menu menu = new Menu(userId, username);
            menu.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }



        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Table();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Получаем новые строки из DataGridView
            DataTable newRows = table.GetChanges(DataRowState.Added);

            if (newRows != null)
            {
                foreach (DataRow row in newRows.Rows)
                {
                    string sql = $"INSERT INTO upravlenie (id_up, desk_up, data_up, task_up, prior_up, otvest_up, zakaz_up) VALUES (@id, @desk, @data, @task, @prior, @otvest, @zakaz)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@id", row["Номер"]);
                    cmd.Parameters.AddWithValue("@desk", row["Описание"]);
                    cmd.Parameters.AddWithValue("@data", row["Дата выполение"]);
                    cmd.Parameters.AddWithValue("@task", row["Выполнение"]);
                    cmd.Parameters.AddWithValue("@prior", row["Приоритет"]);
                    cmd.Parameters.AddWithValue("@otvest", row["Ответственный"]);
                    cmd.Parameters.AddWithValue("@zakaz", row["Заказчик"]);

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

        private void button5_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = dataGridView1.CurrentRow;

            if (selectedRow != null && !selectedRow.IsNewRow)
            {
                string idToDelete = selectedRow.Cells[0].Value.ToString();

                string sql = $"DELETE FROM upravlenie WHERE id_up = @id";
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