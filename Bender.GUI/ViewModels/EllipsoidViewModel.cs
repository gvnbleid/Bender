using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Bender.ClassLibrary.ImplicitGeometry;

namespace Bender.GUI.ViewModels
{
    public class EllipsoidViewModel : GeometryViewModel
    {
        private Ellipsoid _ellipsoid;

        public decimal A
        {
            get => (decimal) _ellipsoid.A;
            set
            {
                _ellipsoid.A = (float) value;
                OnPropertyChanged(nameof(A));
            }
        }

        public decimal B
        {
            get => (decimal) _ellipsoid.B;
            set
            {
                _ellipsoid.B = (float)value;
                OnPropertyChanged(nameof(B));
            }
        }

        public decimal C
        {
            get => (decimal) _ellipsoid.C;
            set
            {
                _ellipsoid.C = (float)value;
                OnPropertyChanged(nameof(C));
            }
        }

        public EllipsoidViewModel(Ellipsoid e, SceneViewModel svm) : base(e, svm)
        {
            _ellipsoid = e;
        }
        public override UserControl CreateView()
        {
            return new Views.Ellipsoid() {DataContext = this};
        }
    }
}
