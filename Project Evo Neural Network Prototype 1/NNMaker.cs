using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Project_Evo_Neural_Network_Prototype_1
{
    class NNMaker
    {
        /* This is the class where most, if not all, of the NN functions will be held
         * In this class, I'll be able to make the NN
         * Let's hope for the best
         * 
         * Additionally, the main resource I'm using is making this for Unity, which is, well, not Visual Studio.
         */

        private int[] layers;//layers    
        private float[][] neurons;//neurons    
        private float[][] biases;//biasses    
        private float[][][] weights;//weights    
        private int[] activations;//layers

        public float fitness = 0;//fitness

        private Random randomNumber = new Random();

        public NNMaker()
        {

        }

        public NNMaker(int[] layers)
        {
            this.layers = new int[layers.Length];

            for(int i = 0; i < layers.Length; i++)
            {
                this.layers[i] = layers[i];
            }

            InitNeurons();
            InitBiases();
            InitWeights();
        }

        private void InitNeurons()
        {
            List<float[]> neuronList = new List<float[]>();

            for (int i = 0; i < layers.Length; i++)
            {
                neuronList.Add(new float[layers[i]]);
            }

            neurons = neuronList.ToArray();
        }

        private void InitBiases()
        {
            List<float[]> biasList = new List<float[]>();

            for(int i = 0; i < layers.Length; i++)
            {
                float[] bias = new float[layers[i]];

                for(int j = 0; j < layers[i]; j++)
                {
                    bias[j] = randomNumber.Next(-10, 10);
                }

                biasList.Add(bias);
            }

            biases = biasList.ToArray();
        }

        private void InitWeights()
        {
            List<float[][]> weightsList = new List<float[][]>();

            for(int i = 1; i < layers.Length; i++)
            {
                List<float[]> layerWeightsList = new List<float[]>();

                int neuronsInPreviousLayer = layers[i - 1];

                for(int j = 0; j < neurons[i].Length; j++)
                {
                    float[] neuronWeight = new float[neuronsInPreviousLayer];

                    for(int k = 0; k < neuronsInPreviousLayer; k++)
                    {
                        neuronWeight[k] = randomNumber.Next(-10, 10);
                    }

                    layerWeightsList.Add(neuronWeight);
                }

                weightsList.Add(layerWeightsList.ToArray());
            }

            weights = weightsList.ToArray();
        }

        public float Activate(float value)
        {
            return (float)Math.Tanh(value);
        }
        // This returns the tanh of the value given. Glad it exists because I didn't want to write out the formula

        // This function allows to calculate what the output is from a number of inputs
        public float[] FeedForward(float[] inputs)
        {
            for(int i = 0; i < inputs.Length; i++)
            {
                neurons[0][i] = inputs[i];
            }

            for(int i = 1; i < neurons.Length; i++)
            {
                int layer = i - 1;

                for(int j = 0; j < neurons[i - 1].Length; j++)
                {
                    float value = 0f;

                    for(int k = 0; k < neurons[i - 1].Length; k++)
                    {
                        value += weights[i - 1][j][k] * neurons[i - 1][k];
                    }

                    neurons[i][j] = Activate(value + biases[i][j]);
                }
            }

            return neurons[neurons.Length - 1];
        }

        public int CompareNetworks(NNMaker NueralNet)
        {
            if(NueralNet == null)
            {
                return 1;
            }


            if(fitness > NueralNet.fitness)
            {
                return 1;
            }

            else if (fitness < NueralNet.fitness)
            {
                return -1;
            }

            else
            {
                return 0;
            }
        }

        // This is supposed to load the weightsw, biases etc. from a file onto the network at hand
        public void LoadNetwork(string path)
        {
            TextReader tr = new StreamReader(path);
            int numberOfLines = (int)new FileInfo(path).Length;
            string[] listLines = new string[numberOfLines];
            int index = 1;

            for(int i = 1; i < numberOfLines; i++)
            {
                listLines[i] = tr.ReadLine();
            }
            tr.Close();

            if(new FileInfo(path).Length > 0)
            {
                for(int i = 0; i < biases.Length; i++)
                {
                    for(int j = 0; j < biases[i].Length; j++)
                    {
                        biases[i][j] = float.Parse(listLines[index]);
                        index++;
                    }
                }

                for(int i = 0; i < weights.Length; i++)
                {
                    for (int j = 0; j < weights[i].Length; j++)
                    {
                        for(int k = 0; k < weights[i][j].Length; k++)
                        {
                            weights[i][j][k] = float.Parse(listLines[index]);
                            index++;
                        }
                    }
                }
            }
        }

        public void Mutate(int chance, float val)
        {
            for (int i = 0; i < biases.Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    biases[i][j] = (randomNumber.Next(0, chance) <= 5) ? biases[i][j] += randomNumber.Next((int)-val, (int)val) : biases[i][j];
                }
            }

            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        weights[i][j][k] = (randomNumber.Next(0, chance) <= 5) ? weights[i][j][k] += randomNumber.Next((int)-val, (int)val) : weights[i][j][k];
                    }
                }
            }
        }

    }
}
