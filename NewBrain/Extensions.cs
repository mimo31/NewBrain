using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBrain
{
    public static class Extensions
    {
        public static bool EqualsArray(this byte[] array2, byte[] array)
        {
            for (int i = 0; i < array2.Length; i++)
            {
                if (array2[i] != array[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
