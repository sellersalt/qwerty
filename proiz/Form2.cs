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
    public partial class Form2 : Form
    {
        MySqlConnection conn = new MySqlConnection(Daniel.Twenty());

        public Form2()
        {
            InitializeComponent();
            dataGridView1.RowHeadersVisible = false;
            Table();
        }
        public void Table()
        {
            string sql = $"SELECT info_user FROM Auth";

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

        private void Form2_Load(object sender, EventArgs e)
        {
            dataGridView1.RowHeadersVisible = false;
            Table();
        }
    }
}
