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
    public partial class MenuGuest : Form
    {
        MySqlConnection conn = new MySqlConnection(Daniel.Twenty());


        private int userId;
        private string username; 

        public MenuGuest(int userId, string username)
        {
            InitializeComponent();
            this.userId = userId;
            this.username = username;
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            label2.Text = userId.ToString();
            label4.Text = username;
        }

        private void MenuGuest_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TaskClient taskClient = new TaskClient(userId, username);
            taskClient.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            infoclient infoclient = new infoclient(userId,username);
            infoclient.Show();
        }
    }
}
