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
    public partial class TaskClient : Form
    {

        private int userId;
        private string username;

        DataTable table;
        MySqlDataAdapter IDataAdapter;

        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;


        MySqlConnection conn = new MySqlConnection(Daniel.Twenty());

        public TaskClient(int userId, string username)
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

        private void TaskClient_Load(object sender, EventArgs e)
        {
            Table();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Table();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Question q = new Question(userId, username);
            q.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
