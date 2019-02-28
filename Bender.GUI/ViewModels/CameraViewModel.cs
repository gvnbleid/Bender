using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bender.ClassLibrary;

namespace Bender.GUI.ViewModels
{
    public class CameraViewModel : GeometryViewModel
    {
        private float _nearClippingPlane;
        private float _farClippingPlane;
        private float _fieldOfView;
        private float _canvasWidth;
        private float _canvasHeight;

        private Camera _camera;

        public CameraViewModel(Camera c)
        {
            _geometry = c;
            _camera = c;
        }

        public decimal NearClippingPlane
        {
            get { return (decimal) _nearClippingPlane; }
            set { _nearClippingPlane = value; }
        }

        public decimal FarClippingPlane
        {
            get { return (decimal) _farClippingPlane; }
            set { _farClippingPlane = value; }
        }

        public decimal FieldOfView
        {
            get { return (decimal) _fieldOfView; }
            set { _fieldOfView = value; }
        }
    }
}
