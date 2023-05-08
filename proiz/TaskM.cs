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

        MySqlConnection conn = new MySqlConnection(Daniel.Twenty());
        public TaskM()
        {
            InitializeComponent();
            dataGridView1.RowHeadersVisible = false;
            Table();
           
        }
        public void Table()
        {
            string sql = $"SELECT id_up as 'Номер', desk_up as 'Описание', data_up as 'Дата выполение', task_up as 'Выполнение', prior_up as 'Приоритет', otvest_up as 'Ответственный', zakaz_up as 'Заказчик'  FROM upravlenie";

            try
            {
                conn.Open();
                MySqlDataAdapter IDataAdapter = new MySqlDataAdapter(sql, conn);
                DataSet dataset = new DataSet();
                IDataAdapter.Fill(dataset);
                dataGridView1.DataSource = dataset.Tables[0];

                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


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

        private void button2_Click(object sender, EventArgs e)
        {
            string searchText = textBox1.Text;
            if (!string.IsNullOrEmpty(searchText))
            {
                bool found = false;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["Заказчик"].Value.ToString().Equals(searchText))
                    {
                        row.Selected = true;
                        row.DefaultCellStyle.SelectionBackColor = Color.Blue;
                        found = true;
                        break;
                    }
                    if (!found)
                    {
                        dataGridView1.ClearSelection();
                        MessageBox.Show("Заказчик " + searchText + " не найден");
                        break;

                    }
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
    }
}
