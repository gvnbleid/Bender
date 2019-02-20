using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bender.ClassLibrary
{
    public struct Edge
    {
        public int Beginning { get; }
        public int End { get; }

        public Edge(int begining, int end)
        {
            Beginning = begining;
            End = end;
        }
    }
}
