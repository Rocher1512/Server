using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace location
{
    public partial class Form1 : Form
    {
        public int whichrequest = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Whois_Click(object sender, EventArgs e)
        {
            whichrequest = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            whichrequest = 1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            whichrequest = 2;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            whichrequest = 3;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<string> argument = new List<string>();
            string user = textBox1.Text;
            string location = textBox2.Text;
            string requesttype = "";
            if (whichrequest == 0)
            {
                requesttype = "";
            }
            else if (whichrequest == 1)
            {
                requesttype = "-h9";
            }
            else if (whichrequest == 2)
            {
                requesttype = "-h0";
            }
            else if(whichrequest == 3)
            {
                requesttype = "-h1";
            }
            argument.Add(user);
            if (location != "")
            {
                argument.Add(location);
            }
            argument.Add(requesttype);
            string[] args = argument.ToArray();
            Console.WriteLine(args.Length);
            Request r = new Request(args);
            Response.Text = r.windowsresponse;
        }
    }
}
