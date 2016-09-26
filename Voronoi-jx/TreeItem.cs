using System;

namespace Voronoi_jx
{
    public class TreeItem
    {

        private VectorNode _point;

        private HalfEdge _incidentHalfEdge;

        public TreeItem(VectorNode node)
        {
            this._point = node;
        }

        public VectorNode GetNode()
        {
            return _point;
        }

        public TreeItem Copy()
        {
            TreeItem ev = new TreeItem(_point);
            ev._incidentHalfEdge = _incidentHalfEdge;
            return ev;
        }

        public HalfEdge GetHalfEdge()
        {
            return _incidentHalfEdge;
        }

        public void SetHalfEdge(HalfEdge halfEdge)
        {
            _incidentHalfEdge = halfEdge;
        }
    }
}

