using System;

namespace Voronoi_jx
{
    public class VectorNode
    {
        private float _x, _y;
        public float x { get { return _x; } }
        public float y { get { return _y; } }

        private HalfEdge _halfEdge;
        public HalfEdge halfEdge { get { return _halfEdge; } set { _halfEdge = value; } }

        public VectorNode(float xval, float yval)
        {
            _x = xval;
            _y = yval;
        }


        public override bool Equals(Object other)
        {
            return other is VectorNode && Equals((VectorNode)other);
        }

        public bool Equals(VectorNode other)
        {
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            return (x.GetHashCode() ^ y.GetHashCode()) + ((int)(x * 34199) + (int)(y * 67011011));
        }
    }
}

