using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using System.Linq;
using UnityEngine;
using UnityEngine.Video;

namespace Faust
{
    [CreateAssetMenu()]
    [System.Serializable]
    public class SpectrumData : ScriptableObject
    {
        [System.Serializable]
        public struct Frame
        {
            public int[] index;
            public float[] level;

            public int Count => index.Length;

            public override string ToString()
            {
                var sb = new System.Text.StringBuilder();
                sb.Append("Frame [");
                for (int i = 0; i < index.Length; i++)
                {
                    sb.AppendFormat("{0}:{1:0.000} ", index[i], level[i]);
                }
                sb.Append("]");
                return sb.ToString();
            }
        }

        public List<Frame> frames;

        SortedList<float, int> peaks;

        public const int PEAKS = 32;
        static int BufferLength;
        static int SampleRate;

        void OnEnable()
        {
            if (frames != null)
            {
                frames = new List<Frame>();
            }
            peaks = new SortedList<float, int>();

            int numBuffers;
            AudioSettings.GetDSPBufferSize(out BufferLength, out numBuffers);
            SampleRate = AudioSettings.outputSampleRate;
            Debug.Log($"buffer sr:{SampleRate} length:{BufferLength} num:{numBuffers}");
        }

        public void Add(float[] data)
        {
            var frame = GetPeeks(data, PEAKS);
            // Debug.Log(frame);
            frames.Add(frame);
        }

        public void ResetData()
        {
            frames.Clear();
        }

        public void SaveToFile(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(fs, frames);
                }
                catch (SerializationException e)
                {
                    Debug.LogErrorFormat("Failed to serialze: {0}", e.Message);
                }
            }
        }

        public void LoadFile(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                try
                {
                    frames = (List<Frame>)formatter.Deserialize(fs);
                }
                catch (SerializationException e)
                {
                    Debug.LogErrorFormat("filed to deserialize: {0}", e.Message);
                }
            }
        }

        public static float IndexToFrequency(int index)
        {
            return (float)index * ((float)SampleRate / (float)BufferLength / 2.0f);
        }

        public static int FrequencyToIndex(float frequency)
        {
            return (int)(frequency / ((float)SampleRate / (float)BufferLength / 2.0f));
        }


        public static string DataPath(VideoClip clip)
        {
            return Path.Combine(Application.streamingAssetsPath, $"{clip.name}.dat");
        }

        Frame GetPeeks(float[] data, int count)
        {
            peaks.Clear();
            bool up = true;
            float n = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (n <= data[i])
                {
                    up = true;
                }
                else
                {
                    if (up && !peaks.ContainsKey(n))
                    {
                        peaks.Add(n, i);
                    }
                    up = false;
                }
                n = data[i];
            }

            var last = peaks.Count > count ? peaks.Skip(peaks.Count - count) : peaks;

            // Debug.Log($"last: {last.Count()}");

            return new Frame()
            {
                index = last.Select((o) => o.Value).ToArray(),
                level = last.Select((o) => o.Key).ToArray(),
            };
        }
    }
}

