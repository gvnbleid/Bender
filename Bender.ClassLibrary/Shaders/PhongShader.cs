using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Bender.ClassLibrary.Geometry;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

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

        public Color GetColor(Camera c, Vector<float> vertexPosition, Vector<float> normalVector, Color diffuseColor, float diffuse, float specular, float ambient, float shininess)
        {
            Vector<float> lightVector = (PositionVector - vertexPosition).Normalize(2);
            Vector<float> viewerVector = (c.PositionVector - vertexPosition).Normalize(2);
            float lightByNormal = lightVector.DotProduct(normalVector);
            Vector<float> reflectionVector = (2 * lightByNormal * normalVector - lightVector).Normalize(2);

            Vector<float> ambientVector = new DenseVector(new float[]{AmbientColor.R + 30, AmbientColor.G + 30, AmbientColor.B + 30});
            Vector<float> diffuseVector = new DenseVector(new float[] { diffuseColor.R, diffuseColor.G, diffuseColor.B });
            Vector<float> specularVector = new DenseVector(new float[] { SpecularColor.R, SpecularColor.G, SpecularColor.B });

            Vector<float> diffusePart = diffuseVector * lightVector.DotProduct(normalVector);
            diffusePart = new DenseVector(diffusePart.Select(MathHelpers.Between0And255).ToArray());

            var refdot = reflectionVector.DotProduct(viewerVector);
            //Vector<float> specularPart =  specularVector * (float)Math.Pow(reflectionVector.DotProduct(viewerVector), shininess);
            //specularPart = new DenseVector(specularPart.Select(MathHelpers.Between0And255).ToArray());

            Vector<float> color = ambient * ambientVector + diffuse * diffusePart;

            if (refdot > 0)
            {
                Vector<float> specularPart = specularVector * (float)Math.Pow(reflectionVector.DotProduct(viewerVector), shininess);
                specularPart = new DenseVector(specularPart.Select(MathHelpers.Between0And255).ToArray());

                color += specularPart * specular;
            }
            color = new DenseVector(color.Select(MathHelpers.Between0And255).ToArray());

            return Color.FromRgb((byte) color[0], (byte) color[1], (byte) color[2]);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
