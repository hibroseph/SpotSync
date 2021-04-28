using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpotSync.Domain
{
    public static class GenericExtensions
    {
        public static List<T> GetRandomNItems<T>(this List<T> source, int n)
        {
            Random random = new Random();

            List<T> list = new List<T>();

            int loopIterations = DetermineLoopIterations<T>(source, n);

            for (int i = 0; i < loopIterations; i++)
            {
                list.Add(source.ElementAt(random.Next(0, source.Count - 1)));
            }
            return list;
        }

        private static int DetermineLoopIterations<T>(List<T> source, int n)
        {
            return source.Count > n ? n : source.Count;
        }

        public static bool IsNullOrEmpty<T>(this List<T> source)
        {
            return source == null || source?.Count == 0;
        }
    }
}
