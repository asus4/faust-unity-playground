using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace Faust
{
    public class FaustParameter : MonoBehaviour
    {
        [SerializeField] GameObject target = null;
        [SerializeField] int parameterIndex = 0;
        [SerializeField] protected string parameterName = "";

        MonoBehaviour targetComp = null;
        MethodInfo setParameter = null;
        MethodInfo getParameter = null;
        MethodInfo getParameterMin = null;
        MethodInfo getParameterMax = null;

        protected void OnEnable()
        {
            FindComponent();
        }

        void FindComponent()
        {
            var behaviours = target.GetComponents<MonoBehaviour>();
            targetComp = behaviours.FirstOrDefault((o) => o.GetType().ToString().StartsWith("FaustPlugin"));
            if (targetComp == null)
            {
                Debug.LogError($"Could not find FaustPlugin Code");
                return;
            }
            Type type = targetComp.GetType();
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

            setParameter = type.GetMethod("setParameter", flags);
            getParameter = type.GetMethod("getParameter", flags);
            getParameterMin = type.GetMethod("getParameterMin", flags);
            getParameterMax = type.GetMethod("getParameterMax", flags);
        }

        public float Parameter
        {
            get
            {
                return (float)getParameter.Invoke(targetComp, new object[] { parameterIndex });
            }
            set
            {
                setParameter.Invoke(targetComp, new object[] { parameterIndex, value });
            }
        }

        public float ParameterMin
        {
            get
            {
                return (float)getParameterMin.Invoke(targetComp, new object[] { parameterIndex });
            }
        }

        public float ParameterMax
        {
            get
            {
                return (float)getParameterMax.Invoke(targetComp, new object[] { parameterIndex });
            }
        }

    }
}
