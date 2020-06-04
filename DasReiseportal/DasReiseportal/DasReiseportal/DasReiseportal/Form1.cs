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

namespace DasReiseportal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void password_txt_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            string user, pass;
            user = name_txt.Text;
            pass = password_txt.Text;
            if (user == "Admin" && pass == "Admin")
            {
                MessageBox.Show("Erfolgreich eingeloggt!");
                this.Hide();
                Form2 f2 = new Form2();
                f2.ShowDialog();
            }
        }
        }
    }

