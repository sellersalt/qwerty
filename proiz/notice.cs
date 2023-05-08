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
    public partial class notice : Form
    {
        MySqlConnection conn = new MySqlConnection(Daniel.Twenty());
        private int userId;
        private string username;

        public notice(int userId, string username)
        {
            InitializeComponent();
            this.userId = userId;
            this.username = username;

            this.dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

        }
        public void Table()
        {
            string sql = $"SELECT id_r as 'Номер', question_r as 'Заданный вопрос', id_Qauthor_r as 'Заявитель' FROM report";

            try
            {
                conn.Open();
                MySqlDataAdapter IDataAdapter = new MySqlDataAdapter(sql, conn);
                DataSet dataset = new DataSet();
                IDataAdapter.Fill(dataset);
                dataGridView1.DataSource = dataset.Tables[0];

                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            }
            catch
            {

            }
            finally
            {
                conn.Close();

            }

        }

        private void notice_Load(object sender, EventArgs e)
        {
            Table();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];

                // Обновить высоту строки
                row.Height = row.GetPreferredHeight(e.RowIndex, DataGridViewAutoSizeRowMode.AllCells, true);
            }
        }
    }
}