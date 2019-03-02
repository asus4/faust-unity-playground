using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Faust
{
    [RequireComponent(typeof(AudioListener), typeof(GLDrawer))]
    public class FFTDataSave : MonoBehaviour
    {
        [SerializeField] SpectrumData data;

        [SerializeField] FFTWindow window = FFTWindow.Hanning;

        float[] levels;
        float[] spectrum;
        B83.MathHelpers.Complex[] bufferA;
        float[] spectrumA;
        System.Numerics.Complex[] bufferB;
        float[] spectrumB;

        float[] windowFunction;

        void Awake()
        {
            Application.targetFrameRate = 60;

            int bufferLength, numBuffers;
            AudioSettings.GetDSPBufferSize(out bufferLength, out numBuffers);
            Debug.Log($"buffer length:{bufferLength} num:{numBuffers}");
        }

        void Start()
        {
            GetComponent<GLDrawer>().OnDraw += OnGLDraw;
        }

        void OnDestroy()
        {
            GetComponent<GLDrawer>().OnDraw -= OnGLDraw;
        }

        void Update()
        {
            AudioListener.GetSpectrumData(spectrum, 0, window);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (levels != null)
            {
                windowFunction = Fourier.WindowFunction.Create(levels.Length, window);
            }
        }
#endif // UNITY_EDITOR

        void OnGLDraw()
        {
            if (levels == null) { return; }

            DrawWaves(levels, Color.cyan);
            DrawWaves(spectrum, Color.green, 0.5f);
            lock (spectrumA)
            {
                DrawWaves(spectrumA, Color.yellow, 0.5f);
            }
            lock (spectrumB)
            {
                DrawWaves(spectrumB, Color.red, 0.5f);
            }
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            if (spectrum == null)
            {
                Debug.Log(data.Length);
                levels = new float[data.Length / channels];
                spectrum = new float[levels.Length];
                spectrumA = new float[levels.Length / 2];
                bufferA = new B83.MathHelpers.Complex[levels.Length];
                spectrumB = new float[levels.Length / 2];
                bufferB = new System.Numerics.Complex[levels.Length];
                windowFunction = Fourier.WindowFunction.Create(levels.Length, window);
            }

            MixChannels(data, levels, channels);

            // Apply windows function
            for (int i = 0; i < levels.Length; i++)
            {
                levels[i] *= windowFunction[i];
            }

            lock (spectrumA)
            {
                ProcessFFTA(levels, bufferA, spectrumA);
            }
            lock (spectrumB)
            {
                ProcessFFTB(levels, bufferB, spectrumB);
            }
        }

        void DrawWaves(float[] buffer, Color color, float showUntil = 1.0f)
        {
            int length = (int)(buffer.Length * showUntil); // nyquist rate
            float width = Screen.width;
            float height = Screen.height;
            float offset = height / 2;

            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);

            for (int i = 0; i < length; i++)
            {
                float f = i / (float)length;
                GL.Vertex3(f * width, buffer[i] * height + offset, 0);
            }

            GL.End();
        }

        static void MixChannels(float[] original, float[] mixed, int channels)
        {
            Debug.AssertFormat(original.Length == mixed.Length * 2,
                               "Buffer length is not correct - original: {0} mixed: {1}", original, mixed);
            float n;
            for (int i = 0; i < mixed.Length; i++)
            {
                n = 0;
                for (int c = 0; c < channels; c++)
                {
                    n += original[i * channels + c];
                }
                mixed[i] = n / channels;
            }
        }

        static void ProcessFFTA(float[] levels, B83.MathHelpers.Complex[] buffer, float[] spectrum)
        {
            Debug.Assert(levels.Length == buffer.Length);
            Debug.Assert(levels.Length == spectrum.Length * 2);

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = new B83.MathHelpers.Complex(levels[i], 0);
            }

            B83.MathHelpers.FFT.CalculateFFT(buffer, false);

            for (int i = 0; i < spectrum.Length; i++)
            {
                spectrum[i] = (float)buffer[i].magnitude;
            }
        }

        void ProcessFFTB(float[] levels, System.Numerics.Complex[] buffer, float[] spectrum)
        {
            Debug.Assert(levels.Length == buffer.Length);
            Debug.Assert(levels.Length == spectrum.Length * 2);

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = new System.Numerics.Complex(levels[i], 0);
            }

            Fourier.Fourier.Transform(buffer, false);

            for (int i = 0; i < spectrum.Length; i++)
            {
                spectrum[i] = (float)buffer[i].Magnitude * bScale;
            }
        }

        public float bScale = 0.1f;
    }
}