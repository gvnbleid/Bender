using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MathNet.Numerics.LinearAlgebra;

namespace Bender.ClassLibrary
{
    public static class Serializer
    {
        public static void WriteVerticesToFile(string fileName, Vector<float>[] vertices)
        {
            List<string> lines = new List<string>();
            foreach (Vector<float> vertex in vertices)
            {
                StringBuilder sb = new StringBuilder();

                foreach (float f in vertex.Storage.AsArray())
                {
                    sb.Append($"{f}, ");
                }

                lines.Add(sb.ToString());
            }

            File.AppendAllLines(fileName, lines);
        }

        public static void WriteVerticesToFile(string fileName, Point[] vertices)
        {
            List<string> lines = new List<string>();

            foreach (Point vertex in vertices)
            {
                lines.Add($"{vertex.X}, {vertex.Y}");
            }

            File.AppendAllLines(fileName, lines);
        }
    }
}
