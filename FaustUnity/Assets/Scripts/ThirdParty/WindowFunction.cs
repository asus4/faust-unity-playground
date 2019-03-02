using UnityEngine;

namespace Fourier
{

    public static class WindowFunction
    {
        // Implementation of Unity FFT Window Functions
        // https://docs.unity3d.com/ScriptReference/FFTWindow.html

        public static float[] Create(int length, FFTWindow type)
        {
            float[] buffer = new float[length];
            switch (type)
            {
                case FFTWindow.Rectangular:
                    Rectangular(buffer);
                    break;
                case FFTWindow.Triangle:
                    Triangle(buffer);
                    break;
                case FFTWindow.Hamming:
                    Hamming(buffer);
                    break;
                case FFTWindow.Hanning:
                    Hanning(buffer);
                    break;
                case FFTWindow.Blackman:
                    Blackman(buffer);
                    break;
                case FFTWindow.BlackmanHarris:
                    BlackmanHarris(buffer);
                    break;
            }
            return buffer;
        }

        static float PI2 = Mathf.PI * 2;
        static float PI3 = Mathf.PI * 3;
        static float PI4 = Mathf.PI * 4;

        static void Rectangular(float[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 1;
            }
        }

        static void Triangle(float[] buffer)
        {
            int halfLen = buffer.Length / 2;
            for (int i = 0; i < buffer.Length / 2; i++)
            {
                buffer[i] = i / (float)halfLen;
            }
            for (int i = 0; i < buffer.Length / 2; i++)
            {
                buffer[i + halfLen] = (halfLen - i) / (float)halfLen;
            }
        }

        static void Hamming(float[] buffer)
        {
            float len = buffer.Length;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0.54f - (0.46f * Mathf.Cos(PI2 * i / len));
            }
        }

        static void Hanning(float[] buffer)
        {
            float len = buffer.Length;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0.5f * (1.0f - Mathf.Cos(PI2 * i / len));
            }
        }

        static void Blackman(float[] buffer)
        {
            float len = buffer.Length;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0.42f - (0.5f * Mathf.Cos(PI2 * i / len)) + (0.08f * Mathf.Cos(PI4 * i / len));
            }
        }

        static void BlackmanHarris(float[] buffer)
        {
            float len = buffer.Length;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0.35875f
                    - (0.48829f * Mathf.Cos(PI2 * i / len))
                    + (0.14128f * Mathf.Cos(PI2 * i / len))
                    - (0.01168f * Mathf.Cos(PI3 * i / len));
            }
        }

    }
}