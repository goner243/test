using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class Neuron
        {
            public decimal weight = 0.5m;
            public decimal activation = 0.5m;
            public decimal near = 0.05m;

            public decimal GetSum(decimal input)
            {
                if (input > activation)
                {
                    return input * weight; 
                }

                return 0;
            }

            public void Learn(decimal input, decimal result)
            {
                if(input != result )
                {
                    weight = result / input;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var n = new Neuron();

            while (textBox3.Text != textBox2.Text)
            {
                textBox2.Text = n.GetSum(decimal.Parse(textBox1.Text)).ToString();
                n.Learn(decimal.Parse(textBox1.Text), decimal.Parse(textBox3.Text));
            }
        }
    }
}
