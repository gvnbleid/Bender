using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Bender.ClassLibrary;

namespace Bender.GUI.ViewModels
{
    public class CameraViewModel : GeometryViewModel
    {
        private float _nearClippingPlane;
        private float _farClippingPlane;
        private float _fieldOfView;
        private float 

        private Camera _camera;

        public CameraViewModel(Camera c, SceneViewModel geometryListViewModel) : base(c, geometryListViewModel)
        {
            _camera = c;
        }

        public decimal NearClippingPlane
        {
            get => (decimal) _nearClippingPlane;
            set => _nearClippingPlane = (float) value;
        }

        public decimal FarClippingPlane
        {
            get => (decimal) _farClippingPlane;
            set => _farClippingPlane = (float) value;
        }

        public decimal FieldOfView
        {
            get => (decimal) _fieldOfView;
            set => _fieldOfView = (float) value;
        }

        public override UserControl CreateView()
        {
            return new Views.Camera {DataContext = this};
        }
    }
}
