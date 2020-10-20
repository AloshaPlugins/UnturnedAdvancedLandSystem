using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdvancedHouseSystem.Helper
{
    public static class CustomMath
    {
        public static float SqrtDistanceFromCube(float x1, float x2, float z1, float z2)
        {
            var x = Mathf.Abs(x1 - x2);
            var y = Mathf.Abs(z1 - z2);
            return Mathf.Sqrt((x * x) + (y * y));
        }
    }
}