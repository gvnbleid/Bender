using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Bender.ClassLibrary;
using Bender.ClassLibrary.Geometry;

namespace Bender.GUI.ViewModels
{
    public class TorusViewModel : GeometryViewModel
    {
        private readonly Torus _torus;

        public int AlphaDensity
        {
            get => _torus.AlphaDensity;
            set
            {
                //SetProperty(ref _alphaDensity, value);
                _torus.AlphaDensity = value;
                OnPropertyChanged(nameof(AlphaDensity));
            }
        }

        public int BetaDensity
        {
            get => _torus.BetaDensity;
            set
            {
                //SetProperty(ref _betaDensity, value);
                _torus.BetaDensity = value;
                OnPropertyChanged(nameof(BetaDensity));
            }
        }

        public TorusViewModel(Torus t, SceneViewModel svm) : base(t, svm)
        {
            _torus = t;
        }

        public override UserControl CreateView()
        {
            return new Views.Torus() {DataContext = this};
        }
    }
}
