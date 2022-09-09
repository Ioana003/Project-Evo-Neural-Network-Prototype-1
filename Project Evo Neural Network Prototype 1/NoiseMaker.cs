using System;
using System.Collections.Generic;
using System.Text;

namespace Project_Evo_Neural_Network_Prototype_1
{
    class NoiseMaker
    {

        private float[][] noise;
        private float[][] smoothNoise;

        public NoiseMaker()
        {

        }

        public float[][] GenerateWhiteNoise(int width, int height, int seed)
        {
            Random random = new Random(seed); //The seed is the number inputted by the user
            noise = GetEmptyArray<float>(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    noise[i][j] = (float)random.NextDouble() % 1; //Any number given by random will be between 0 and 1
                }
            }

            return noise;
        }

        public float[][] GenerateSmoothNoise(float[][] baseNoise, int octave)
        {
            int width = baseNoise.Length;
            int height = baseNoise[0].Length;

            smoothNoise = GetEmptyArray<float>(width, height);

            int samplePeriod = 1 << octave; //Bitwise operation to shift left, thus calculates 2^k using octave as k, which is the wavelength of the noise
            float sampleFrequency = 1.0f / samplePeriod; // This calculates the frequency of the noise, which is 1/2^k

            for (int i = 0; i < width; i++)
            {
                //Calculates the horizontal sampling indices
                int sample_i0 = (i / samplePeriod) * samplePeriod;
                int sample_i1 = (sample_i0 + samplePeriod) % width; // To wrap around
                float horizontal_blend = (i - sample_i0) * sampleFrequency;

                for (int j = 0; j < height; j++)
                {
                    // Camculate the vertical sampling indices
                    int sample_j0 = (j / samplePeriod) * samplePeriod;
                    int sample_j1 = (sample_j0 + samplePeriod) % height;
                    float vertical_blend = (j - sample_j0) * sampleFrequency;

                    //Blend the TOP corners
                    float top = Interpolate(baseNoise[sample_i0][sample_j0], baseNoise[sample_i1][sample_j1], horizontal_blend);

                    //Blend the BOTTOM corners
                    float bottom = Interpolate(baseNoise[sample_i0][sample_j0], baseNoise[sample_i1][sample_j1], vertical_blend);

                    //Final Blend
                    smoothNoise[i][j] = Interpolate(top, bottom, vertical_blend);
                }
            }

            return smoothNoise;

        }


        public float[][] GeneratePerlinhNoise(float[][] baseNoise, int octaveCount)
        {
            int width = baseNoise.Length;
            int height = baseNoise[0].Length;

            float[][][] smoothNoise = new float[octaveCount][][];

            float persistance = 0.7f;

            //Generate the Smooth Noise
            for (int i = 0; i < octaveCount; i++)
            {
                smoothNoise[i] = GenerateSmoothNoise(baseNoise, i);
            }

            float[][] perlinNoise = GetEmptyArray<float>(width, height);
            float amplitude = 1f;
            float totalAmplitude = 0.0f;

            //Blend the noise
            for (int octave = octaveCount - 1; octave >= 0; octave--)
            {
                amplitude *= persistance;
                totalAmplitude += amplitude;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        perlinNoise[i][j] += smoothNoise[octave][i][j] * amplitude;
                    }
                }
            }

            //Normalisation
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    perlinNoise[i][j] /= totalAmplitude / (float)1.45;
                }
            }

            return perlinNoise;
        }

        public static T[][] GetEmptyArray<T>(int width, int height)
        {
            T[][] image = new T[width][];

            for (int i = 0; i < width; i++)
            {
                image[i] = new T[height];
            }

            return image;
        }

        public static float Interpolate(float x0, float x1, float alpha)
        {
            return x0 * (1 - alpha) + alpha * x1;
        }
    }
}
