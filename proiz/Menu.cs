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
    public partial class Menu : Form
    {
        public static class UserInfo
        {
            public static string CurrentUsername { get; set; }
            public static int CurrentEmployeeId { get; set; }
        }

        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        MySqlConnection conn = new MySqlConnection(Daniel.Twenty());

        private int userId;
       
        private string username;

        private string currentUserRole;
        public Menu(int userId, string username)
        {
            InitializeComponent();
            this.userId = userId;
            this.username = username;
            CheckUserRole();
            RestrictGuestAccess();
            UpdateLabels();

        }

        private void UpdateLabels()
        {
            label3.Text = userId.ToString(); 
            label4.Text = username; 
        }

        private void CheckUserRole()
        {
           
        }

        private void RestrictGuestAccess()
        {
            if (currentUserRole == "2")
            {
                button9.Visible = false; // Отключаем кнопку для гостя
        
            }
        }
        private void Form4_Load(object sender, EventArgs e)
        {
             
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            PMS pms = new PMS(userId, username);
            pms.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            TaskM TM = new TaskM(userId, username);
            TM.ShowDialog();
           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            TimeT TiM = new TimeT(userId, username);
            TiM.ShowDialog();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            Empl emp = new Empl(userId, username);
            emp.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Close();
            report rep = new report(userId, username);
            rep.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            notice notice = new notice(userId, username);
            notice.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
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
