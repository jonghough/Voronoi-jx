using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voronoi_jx
{
    public class DelaunayEdge
    {

        private VectorNode _from, _to;
        public DelaunayEdge(VectorNode from, VectorNode to)
        {
            _from = from;
            _to = to;
        }


        public VectorNode from
        {
            get
            {

                return _from;
            }
        }


        public VectorNode to
        {
            get
            {

                return _to;
            }
        }


    }
}
