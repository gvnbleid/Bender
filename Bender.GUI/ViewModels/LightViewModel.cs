using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Bender.ClassLibrary.Shaders;
using Bender.GUI.Views;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Bender.GUI.ViewModels
{
    public class LightViewModel : ViewModelBase
    {
        private float _positionX;
        private float _positionY;
        private float _positionZ;

        private PhongShader _light;

        public decimal PositionX
        {
            get => (decimal) _light.PositionVector[0];
            set
            {
                _positionX = (float) value;
                SetProperty(ref _positionX, (float) value);
                _light.PositionVector = new DenseVector(new []{_positionX, _positionY, _positionZ, 1f});
                SceneViewModel.Refresh();
            }
        }

        public decimal PositionY
        {
            get => (decimal) _light.PositionVector[1];
            set
            {
                _positionY = (float) value;
                SetProperty(ref _positionY, (float) value);
                _light.PositionVector = new DenseVector(new[] { _positionX, _positionY, _positionZ, 1f });
                SceneViewModel.Refresh();
            }
        }

        public decimal PositionZ
        {
            get => (decimal) _light.PositionVector[2];
            set
            {
                _positionZ = (float) value;
                SetProperty(ref _positionZ, (float) value);
                _light.PositionVector = new DenseVector(new[] { _positionX, _positionY, _positionZ, 1f });
                SceneViewModel.Refresh();
            }
        }

        public LightViewModel(PhongShader l, SceneViewModel svm) : base(svm)
        {
            _light = l;

            _positionX = l.PositionVector[0];
            _positionY = l.PositionVector[1];
            _positionZ = l.PositionVector[2];
        }

        public override UserControl CreateView()
        {
            return new Light() {DataContext = this};
        }
    }
}
