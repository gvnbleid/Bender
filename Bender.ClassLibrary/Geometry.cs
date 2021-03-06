﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Bender.ClassLibrary.Annotations;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using Vector = System.Windows.Vector;

namespace Bender.ClassLibrary
{
    public abstract class Geometry
    {
        private Vector<float> _positionVector;
        private Vector<float> _rotationVector;
        private Vector<float> _scaleVector;

        public Vector<float> PositionVector
        {
            get => _positionVector;
            protected set
            { 
                _positionVector = value;
            }
        }

        public Vector<float> RotationVector
        {
            get => _rotationVector;
            protected set
            {
                _rotationVector = value;
            }
        }

        public Vector<float> ScaleVector
        {
            get => _scaleVector;
            protected set
            {
                _scaleVector = value;
            }
        }

        public Matrix<float> WorldMatrix { get; protected set; }
        public Vector<float>[] Vertices { get; protected set; }
        public Edge[] Edges { get; protected set; }

        public string Name { get; protected set; }

        protected Geometry(string name, Vector<float> positionVector, Vector<float> rotationVector, Vector<float> scaleVector, Vector<float>[] vertices = null, Edge[] edges = null)
        {
            Name = name;
            Vertices = vertices;
            Edges = edges;

            PositionVector = positionVector.Clone();
            RotationVector = rotationVector.Clone();
            ScaleVector = scaleVector.Clone();

            WorldMatrix = MathHelpers.CalculateTranslationMatrix(PositionVector) * MathHelpers.CalculateRotationMatrix(RotationVector);
        }

        public override string ToString()
        {
            return Name;
        }

        public void Draw(Matrix<float> viewMatrix, Matrix<float> projectionMatrix, out Vector<float>[] verticesInScreenSpace, out IEnumerable<Edge> topology)
        {
            foreach (var vertex in Vertices)
            {
                var xprim = viewMatrix * vertex;
                var xprimprim = projectionMatrix * xprim;


            }

            verticesInScreenSpace = Vertices.Select(x => (projectionMatrix * viewMatrix * x).Divide(x[2])).ToArray();
            topology = Edges;
        }

        public virtual void Update(Vector<float> positionVector, Vector<float> rotationVector, Vector<float> scaleVector)
        {
            PositionVector = positionVector.Clone();
            RotationVector = rotationVector.Clone();
            ScaleVector = scaleVector.Clone();

            WorldMatrix = MathHelpers.CalculateTranslationMatrix(PositionVector) * MathHelpers.CalculateRotationMatrix(RotationVector);
        }

        public void Transform(Vector<float> transformVector)
        {
            var m = MathHelpers.CalculateTranslationMatrix(transformVector);
            PositionVector += transformVector;
            WorldMatrix *= m;
        }

        public void Rotate(Vector<float> rotationVector)
        {
            var m = MathHelpers.CalculateRotationMatrix(rotationVector);
            RotationVector += rotationVector;
            WorldMatrix *= m;
        }
    }
}
