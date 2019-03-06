using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Bender.ClassLibrary;

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
                SceneViewModel.Refresh();
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
                SceneViewModel.Refresh();
            }
        }

        public decimal SmallRadius
        {
            get => (decimal) _torus.SmallRadius;
            set
            {
                if ((float) value < _torus.BigRadius)
                {
                    _torus.SmallRadius = (float) value;
                    OnPropertyChanged(nameof(SmallRadius));
                    SceneViewModel.Refresh();
                }
            }
        }

        public decimal BigRadius
        {
            get => (decimal)_torus.BigRadius;
            set
            {
                if ((float)value > _torus.SmallRadius)
                {
                    _torus.BigRadius = (float)value;
                    OnPropertyChanged(nameof(BigRadius));
                    SceneViewModel.Refresh();
                }
            }
        }

        public TorusViewModel(Torus t, SceneViewModel geometryListViewModel) : base(t, geometryListViewModel)
        {
            _torus = t;
        }

        public override UserControl CreateView()
        {
            return new Views.Torus() {DataContext = this};
        }
    }
}
