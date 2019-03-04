using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace Faust
{
    [RequireComponent(typeof(FaustPlugin_SimpleOsc), typeof(VideoPlayer))]
    public class SpectrumDataPlayer : MonoBehaviour
    {
        [SerializeField] SpectrumData data = null;
        [SerializeField, Range(0f, 100f)] float masterGain = 1;

        FaustPlugin_SimpleOsc osc;

        int currentIndex = 0;

        void Start()
        {
            osc = GetComponent<FaustPlugin_SimpleOsc>();

            var player = GetComponent<VideoPlayer>();
            data.LoadFile(SpectrumData.DataPath(player.clip));
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
                for (int i = frame.Count; i < 16; i++)
                {
                    SetParameter(i, 50, 0);
                }
            }
            currentIndex++;
        }

        void SetParameter(int index, float frequency, float gain)
        {
            osc.setParameter(index * 2, frequency);
            // osc.setParameter(index * 2 + 1, Mathf.Sqrt(gain * masterGain));
            // osc.setParameter(index * 2 + 1, gain * gain * masterGain);
            osc.setParameter(index * 2 + 1, gain * masterGain);
        }
    }
}