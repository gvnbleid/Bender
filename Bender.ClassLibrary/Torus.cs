using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Bender.ClassLibrary
{
    public class Torus : Geometry
    {
        public Geometry Geometry { get; }

        private float _bigRadius;
        private float _smallRadius;
        private int _alphaDensity;
        private int _betaDensity;
        public int AlphaDensity
        {
            get => _alphaDensity;
            set
            {
                _alphaDensity = value;
                Generate();
            }
        }
        public int BetaDensity
        {
            get => _betaDensity;
            set
            {
                _betaDensity = value;
                Generate();
            }
        }

        public Torus(string name, float bigRadius, float smallRadius, int alphaDensity, int betaDensity) : base(
            name, new DenseVector(new[] {0f, 0f, 0f, 0f}),
            new DenseVector(new[] {0f, 0f, 0f, 0f}), new DenseVector(new[] {1f, 1f, 1f, 0f}))
        {
            if (bigRadius < 0.0f || smallRadius < 0.0f || bigRadius <= smallRadius || alphaDensity <= 0 ||
                alphaDensity > 360 || betaDensity < 4 || betaDensity > 360)
            {
                throw new Exception("Wrong arguments");
            }

            _bigRadius = bigRadius;
            _smallRadius = smallRadius;
            _alphaDensity = alphaDensity;
            _betaDensity = betaDensity;

            Generate();
        }

        private void Generate()
        {
            var alphaIncrement = 360f / AlphaDensity;
            var betaIncrement = 360f / BetaDensity;

            List<Vector> vertices = new List<Vector>();
            List<Edge> edges = new List<Edge>();
            int i, j;
            i = j = 0;

            int k = (int)BetaDensity;
            int l = (int)AlphaDensity;

            for (float alpha = 0.0f;
                alpha < 359.5f;
                alpha += alphaIncrement)
            {
                for (float beta = 0.0f;
                    beta < 359.5f;
                    beta += betaIncrement)
                {
                    double trigAlpha = Trig.DegreeToRadian(alpha);
                    double trigBeta = Trig.DegreeToRadian(beta);

                    float x = (float)Math.Cos(trigAlpha) * (_bigRadius + _smallRadius * (float)Math.Cos(trigBeta));
                    float z = (float)Math.Sin(trigAlpha) * (_bigRadius + _smallRadius * (float)Math.Cos(trigBeta));
                    float y = (float)Math.Sin(trigBeta) * _smallRadius;

                    vertices.Add(new DenseVector(new[] { x, y, z, 1f }));

                    edges.Add(new Edge(i * k + j, i * k + (j + 1) % k));
                    edges.Add(new Edge(i * k + j, ((i + 1) % l) * k + j));
                    j++;
                }

                j = 0;
                i++;
            }

            Vertices = vertices.ToArray();
            Edges = edges.ToArray();
        }
    }
}
