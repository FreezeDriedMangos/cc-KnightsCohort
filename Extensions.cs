using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort
{
    public static class Extensions
    {
        public static T KnightRandom<T>(this List<T> list, Rand rng)
        {
            return list[rng.NextInt() % list.Count];
        }
    }
}
