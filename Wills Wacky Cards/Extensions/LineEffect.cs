using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace WWC.Extensions
{
    public static class LineEffectExtension
    {
        public static float GetRadius(this LineEffect lineEffect)
        {
            return (float)typeof(LineEffect).InvokeMember("GetRadius",
                BindingFlags.Instance | BindingFlags.InvokeMethod |
                BindingFlags.NonPublic, null, lineEffect, new object[] {});
        }
    }
}