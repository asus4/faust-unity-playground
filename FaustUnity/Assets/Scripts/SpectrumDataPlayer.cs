using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace Faust
{
    [RequireComponent(typeof(FaustPlugin_SimpleOsc), typeof(VideoPlayer), typeof(GLDrawer))]
    public class SpectrumDataPlayer : MonoBehaviour
    {
        public enum GainMode
        {
            Normal,
            Sqrt,
            Sq,
            Log,
        }
        [SerializeField] GainMode gainMode;

        [SerializeField] SpectrumData data = null;
        [SerializeField, Range(0f, 100f)] float masterGain = 1;

        FaustPlugin_SimpleOsc osc;

        int currentIndex = 0;

        void Start()
        {
            osc = GetComponent<FaustPlugin_SimpleOsc>();

            var player = GetComponent<VideoPlayer>();
            data.LoadFile(SpectrumData.DataPath(player.clip));

            GetComponent<GLDrawer>().OnDraw += OnGLDraw;
        }

        void OnDestroy()
        {
            GetComponent<GLDrawer>().OnDraw -= OnGLDraw;
        }

        void Update()
        {
            if (currentIndex < data.frames.Count)
            {
                var frame = data.frames[currentIndex];
                for (int i = 0; i < frame.Count; i++)
                {
                    float freq = SpectrumData.IndexToFrequency(frame.index[i]);
                    float gain = frame.level[i];
                    SetParameter(i, freq, gain);
                }
                for (int i = frame.Count; i < SpectrumData.PEAKS; i++)
                {
                    SetParameter(i, 50, 0);
                }
            }
            currentIndex++;
        }

        void OnGLDraw()
        {
            GL.Begin(GL.LINES);
            GL.Color(Color.red);

            if (currentIndex < data.frames.Count)
            {
                float width = Screen.width;
                float height = Screen.height;
                float offset = 0;

                var frame = data.frames[currentIndex];
                for (int i = frame.Count-1; i >= 0; i--)
                {
                    float freq = SpectrumData.IndexToFrequency(frame.index[i]);
                    float x = Mathf.InverseLerp(0, 22050, freq) * width;
                    float y = frame.level[i] * height * 10;
                    GL.Vertex3(x, offset, 0);
                    GL.Vertex3(x, offset + y, 0);
                }
            }

            GL.End();
        }

        void SetParameter(int index, float frequency, float gain)
        {
            osc.setParameter(index * 2 + 1, frequency);

            gain *= masterGain;

            float volume = 0;
            switch (gainMode)
            {
                case GainMode.Normal:
                    volume = gain; break;
                case GainMode.Sqrt:
                    volume = Mathf.Sqrt(gain); break;
                case GainMode.Sq:
                    volume = gain * gain; break;
                case GainMode.Log:
                    volume = Mathf.Log(gain); break;
            }
            osc.setParameter(index * 2 + 2, volume);
        }
    }
}