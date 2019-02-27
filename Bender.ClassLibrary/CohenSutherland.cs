using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace Bender.ClassLibrary
{
    public static class CohenSutherland
    {
        private static bool[] GetCode(Vector<float> vertex)
        {
            bool[] code = new bool[6];

            for (int i = 0; i < 3; i++)
            {
                if (vertex[i] < -1)
                {
                    code[i * 2] = true;
                    continue;
                }

                if (vertex[i] > 1) code[i * 2 + 1] = true;
            }

            //if (vertex[3] > 1) code[4] = true;
            //if (vertex[3] < -1) code[5] = true;

                return code;
        }

        public static bool TryClipLine(Vector<float> v1, Vector<float> v2, out float[] line)
        {
            line = null;
            int counter = 0;

            if (v1[3] < 0 && v2[3] < 0) return false;

            while (true)
            {
                bool[] code1 = GetCode(v1);
                bool[] code2 = GetCode(v2);

                if (!code1.Any(x => x) && !code2.Any(x => x))
                {
                    line = new[] { v1[0], v1[1], v2[0], v2[1] };
                    return true;
                }

                for (int i = 0; i < 6; i++)
                {
                    if (code1[i] && code2[i]) return false;
                }

                for (int i = 0; i < 3; i++)
                {
                    bool br = false;
                    for (int j = 0; j < 2; j++)
                    {
                        if (code1[2 * i + j] != code2[2 * i + j])
                        {
                            if (!code1[2 * i + j])
                            {
                                var tmp = v1.Clone();
                                v1 = v2.Clone();
                                v2 = tmp;

                                var btmp = code1.Select(x => x).ToArray();
                                code1 = code2.Select(x => x).ToArray();
                                code2 = btmp;
                            }

                            float t = (((2 * i + j) % 2 == 0 ? -1 : 1) - v1[i]) / (v2[i] - v1[i]);
                            v1 += (v2 - v1) * t;

                            br = true;

                            break;
                        }
                    }

                    if (br) break;

                }

                counter++;
                if (counter == 3)
                {
                    int a = 5;
                }
            }
        }
    }
}
