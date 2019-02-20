using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bender.ClassLibrary
{
    public interface IGeometry
    {
        Geometry Geometry { get; }

        void Draw();
    }
}
