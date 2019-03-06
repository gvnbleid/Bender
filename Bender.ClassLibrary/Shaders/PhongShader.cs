using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Bender.ClassLibrary.Geometry;
using MathNet.Numerics.LinearAlgebra;

namespace Bender.ClassLibrary.Shaders
{
    public class PhongShader : IPositionable
    {
        public Vector<float> PositionVector { get; set; }
        public Vector<float> RotationVector { get; set; }
        public Vector<float> ScaleVector { get; set; }
        public string Name { get; set; }
        public Color AmbientColor { get; set; }
        public Color SpecularColor { get; set; }

        public PhongShader(string name, Vector<float> position, Color ambientColor, Color specularColor)
        {
            Name = name;
            PositionVector = position;
            AmbientColor = ambientColor;
            SpecularColor = specularColor;
        }

        public Color GetColor(Camera c, Vector<float> vertexPosition, Vector<float> normalVector, Color diffuseColor)
        {
            Vector<float> lightVector = (PositionVector - vertexPosition).Normalize(2);
            Vector<float> viewerVector = (c.PositionVector - vertexPosition).Normalize(2);
            Vector<float> reflectionVector = 2 * (lightVector.DotProduct(normalVector)) * normalVector - lightVector;

            Color color = AmbientColor + diffuseColor * lightVector.DotProduct(normalVector) +
                                SpecularColor * (float) Math.Pow(reflectionVector.DotProduct(viewerVector), 5);
            return color;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
