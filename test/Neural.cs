using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Neural
    {
        public double weight = 0.5;
        public double activation;
        public double acuracy = 0.1;
        public double current;

        public double CalcActivation(double input)
        {
            activation = 1 / (1 + Math.Exp(input));
            return activation;
        }

        public double CalculateN(double input)
        {
            CalcActivation(input);
            if (input >= activation)
            {
                return input * weight;
            }
            return 0;
        }

        public void ReCalcWheight(bool rightAnswer, double input)
        {
            if (rightAnswer && input >= activation)
            {
                weight = weight + activation;
            }
            else
            {
                weight = weight - activation;
            }
        }

    }

    class NeuralNet
    {
        int NeuronCount { get; set; }
        int Layers { get; set; }

        Neural[,] Neurals;

        //Инициализация сетки
        public NeuralNet(int neuronCount, int layers)
        {
            NeuronCount = neuronCount;
            Layers = layers;
            Neurals = new Neural[layers, neuronCount];

            for (int i = 0; i < Layers; i++)
            {
                for (int j = 0; j < NeuronCount; j++)
                {
                    Neurals[i, j] = new Neural();
                }
            }
        }

        //Расчет выхода, сначала считаем первыцй слой относительно входных данных, затем считаем все последующие относительно предидущего слоя
        public double CalculateOut(double input)
        {
            double sum = 0;

            for (int i = 0; i < Layers; i++)
            {
                if (i == 0)
                {
                    for (int j = 0; j < NeuronCount; j++)
                    {
                        Neurals[i, j].current = Neurals[i, j].CalculateN(input);

                        sum = sum + Neurals[i, j].current;
                    }
                }
                else
                {
                    double layerSum = 0;
                    for (int j = 0; j < NeuronCount; j++)
                    {
                        for (int k = 0; k < NeuronCount; k++)
                        {
                            Neurals[i, j].current = Neurals[i, j].current + Neurals[i, j].CalculateN(Neurals[i -1, k].current);
                        }

                        layerSum = layerSum + Neurals[i, j].current;
                    }

                    sum = layerSum;
                }
            }

            return sum;
        }


        //Приведение входных значений к диапозону  0-1
        public List<double> Normalize(List<double> inputs)
        {
            List<double> outputs = new List<double>();

            foreach(var a in inputs)
            {
                outputs.Add((a - double.Parse(inputs.Min(b => b).ToString())) / (double.Parse(inputs.Max(b => b).ToString()) - double.Parse(inputs.Min(b => b).ToString())));
            }

            return outputs;
        }
    }
}

