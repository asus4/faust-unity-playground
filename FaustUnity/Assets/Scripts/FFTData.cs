using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Faust
{
    [CreateAssetMenu()]
    public class SpectrumData : ScriptableObject
    {
        [System.Serializable]
        public struct Frame
        {
            public int[] index;
            public float[] level;
        }

        public List<Frame> frames;

        void OnEnable()
        {
            frames = new List<Frame>();
        }

        public void Add(float[] data)
        {
            
        }
    }
}