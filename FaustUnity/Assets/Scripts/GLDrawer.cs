using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Faust
{
    [RequireComponent(typeof(Camera))]
    public class GLDrawer : MonoBehaviour
    {
        public event System.Action OnDraw;

        Material material = null;

        void Start()
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            material = new Material(shader);
            material.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            material.SetInt("_ZWrite", 0);
        }

        void OnRenderObject()
        {
            material.SetPass(0);
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            OnDraw();
            GL.PopMatrix();
        }


    }
}