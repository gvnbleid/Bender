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
        private PhongShader _light;

        public decimal PositionX
        {
            get => (decimal) _light.PositionVector[0];
            set
            {
                _light.PositionVector[0] = (float) value;
                OnPropertyChanged(nameof(PositionX));
            }
        }

        public decimal PositionY
        {
            get => (decimal) _light.PositionVector[1];
            set
            {
                _light.PositionVector[1] = (float) value;
                OnPropertyChanged(nameof(PositionY));
            }
        }

        public decimal PositionZ
        {
            get => (decimal) _light.PositionVector[2];
            set
            {
                _light.PositionVector[2] = (float) value;
                OnPropertyChanged(nameof(PositionZ));
            }
        }

        public LightViewModel(PhongShader l, SceneViewModel svm) : base(svm)
        {
            _light = l;
        }

        public override UserControl CreateView()
        {
            return new Light() {DataContext = this};
        }
    }
}
