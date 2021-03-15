using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio;
using NAudio.CoreAudioApi;
using NAudio.Dsp;
using NAudio.Utils;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace test
{
    public partial class Form1 : Form
    {
        int volume = 5000;
        int n = 1024; // number of x-axis pints
        //Stopwatch time = new Stopwatch();
        WaveIn wi;
        WaveOut wo;
        List<double> myQ;
        List<double> spec;
        List<double> noise;
        NeuralNet nn = new NeuralNet(32, 3);

        public Form1()
        {
            InitializeComponent();


            for (int n = 0; n < WaveIn.DeviceCount; n++)
            {
                var caps = WaveIn.GetCapabilities(n);
                comboBox1.Items.Add($"{n}: {caps.ProductName} / {caps.Channels}");
            }

            comboBox1.SelectedIndex = 0;

            myQ = new List<double>(Enumerable.Repeat(0.0, n)); // fill myQ w/ zeros
            spec = new List<double>(Enumerable.Repeat(0.0, n / 8));
            noise = new List<double>(Enumerable.Repeat(0.0, n ));
            chart1.ChartAreas[0].AxisY.Minimum = -volume;
            chart1.ChartAreas[0].AxisY.Maximum = volume;

            chart2.ChartAreas[0].AxisY.Minimum = -10;
            chart2.ChartAreas[0].AxisY.Maximum = volume * 100;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            wi = new WaveIn();
            wo = new WaveOut();

            wi.StartRecording();
            wi.WaveFormat = new WaveFormat(4, 16, 1); // (44100, 16, 1);
            wi.DataAvailable += new EventHandler<WaveInEventArgs>(wi_DataAvailable);
            timer1.Enabled = true;
            //time.Start();
        }


        void wi_DataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] buffer = e.Buffer;

            System.Numerics.Complex[] c = new System.Numerics.Complex[n];

            for (int i = 0; i < n; i += 2)
            {
                c[i] = new System.Numerics.Complex(BitConverter.ToInt16(buffer, i), 0);
                c[i + 1] = new System.Numerics.Complex(BitConverter.ToInt16(buffer, i), 0);
            }

            var fft = FFT.fft(c);
            var nfft = FFT.nfft(fft);

            for (int i = 0; i < n; i += 1)
            {
                spec.Add(fft[i].Magnitude * int.Parse(label3.Text));
                spec.RemoveAt(0);
            }

            spec.Reverse();

            label4.Text = spec.IndexOf(spec.Max(a => a)).ToString();

            for (int i = 0; i < e.BytesRecorded; i += 2)
            {
                myQ.Add(BitConverter.ToInt16(buffer, i) * int.Parse(label3.Text));
                myQ.RemoveAt(0);
            }

            //for (int i = 0; i < e.BytesRecorded; i += 2)
            //{
            //    noise.Add(myQ.FindAll(a => a > 0).Average(b => b));
            //    noise.RemoveAt(0);
            //}

            label5.Text = myQ.FindAll(a => a > 0).Average(b => b).ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                chart1.Series["Series1"].Points.DataBindY(myQ);

                var normal = nn.Normalize(myQ.ToList());

                double sum = 0;

                foreach (var a in normal)
                {
                    sum += nn.CalculateOut(a);
                }

                label1.Text = sum.ToString();

                //chart1.ResetAutoValues();

                chart2.Series["Series1"].Points.DataBindY(spec);
            }
            catch
            {
                Console.WriteLine("No bytes recorded");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label2.Text = trackBar1.Value.ToString();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (wi != null)
            {
                timer1.Enabled = false;
                wi.StopRecording();
                wi = new WaveIn();
                wi.DeviceNumber = comboBox1.SelectedIndex;
                wi.StartRecording();
                wi.WaveFormat = new WaveFormat(4, 16, 1); // (44100, 16, 1);
                wi.DataAvailable += new EventHandler<WaveInEventArgs>(wi_DataAvailable);
                timer1.Enabled = true;
            }
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            label3.Text = trackBar2.Value.ToString();
        }

        public void FindPatterns(int[] input)
        {
            int patternSize = 16;


        }
    }
}
