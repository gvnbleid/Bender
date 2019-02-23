using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Bender.ClassLibrary
{
    public class Torus : IGeometry
    {
        public Geometry Geometry { get; }

        private float _bigRadius;
        private float _smallRadius;
        private float _alphaDensity;
        private float _betaDensity;

        public Torus(float bigRadius, float smallRadius, float alphaDensity, float betaDensity)
        {
            if (bigRadius < 0.0f || smallRadius < 0.0f || bigRadius <= smallRadius || alphaDensity <= 0 ||
                alphaDensity > 1 || betaDensity <= 0 || betaDensity > 1)
            {
                throw new Exception("Wrong arguments");
            } 

            _bigRadius = bigRadius;
            _smallRadius = smallRadius;
            _alphaDensity = alphaDensity;
            _betaDensity = betaDensity;

            List<Vector> vertices = new List<Vector>();
            List<Edge> edges = new List<Edge>();
            int i, j;
            i = j = 0;

            for (float alpha = 0.0f; alpha <= (float) MathNet.Numerics.Trig.DegreeToRadian(360); alpha += (float) MathNet.Numerics.Trig.DegreeToRadian(5))
            {
                for (float beta = 0.0f; beta <= (float) MathNet.Numerics.Trig.DegreeToRadian(360); beta += (float) MathNet.Numerics.Trig.DegreeToRadian(5))
                {
                    float x = (float) Math.Cos(alpha) * (_bigRadius + _smallRadius * (float) Math.Cos(beta));
                    float z = (float) Math.Sin(alpha) * (_bigRadius + _smallRadius * (float) Math.Cos(beta));
                    float y = (float) Math.Sin(beta) * _smallRadius;

                    vertices.Add(new DenseVector(new []{x, y, z, 1f}));

                    if(j > 0) edges.Add(new Edge(i*72 + j - 1, i*72 + j));
                    if(i > 0) edges.Add(new Edge(i*72 + j, (i-1)*72 + j));

                    j++;
                }

                j = 0;
                i++;
            }

            Geometry = new Geometry(vertices.ToArray(), edges.ToArray());
        }
    }
}
