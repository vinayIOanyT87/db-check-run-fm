using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordResetToolGUI
{
    public partial class adminPasscode : Form
    {
        public adminPasscode()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
            this.AcceptButton = button1;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "5834Varec")
            { DialogResult = DialogResult.OK;
                this.Hide();
                passwordReset form = new passwordReset();
                form.ShowDialog();
            }
            else
            {
                label2.Text = "Passcode is incorrect";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
