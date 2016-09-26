using System;

namespace Voronoi_jx
{
    public class HalfEdge
    {
        private HalfEdge _next = null;
        private HalfEdge _previous = null;
        private HalfEdge _twin = null;
        private CircleEvent _target = null;
        private VectorNode _face = null;

        public HalfEdge(VectorNode face)
        {
            SetFace(face);
        }

        public VectorNode GetFace()
        {
            return _face;
        }

        public void SetFace(VectorNode node)
        {
            _face = node;
        }

        public HalfEdge Twin()
        {
            return _twin;
        }

        public void SetTwin(HalfEdge edge)
        {
            _twin = edge;
        }

        public HalfEdge Next()
        {
            return _next;
        }

        public void SetNext(HalfEdge edge)
        {
            _next = edge;
        }

        public HalfEdge Previous()
        {
            return _previous;
        }

        public void SetPrevious(HalfEdge edge)
        {
            _previous = edge;
        }

        public CircleEvent GetTarget()
        {
            return _target;
        }

        public void SetTarget(CircleEvent circleEvent)
        {
            _target = circleEvent;
        }
    }
}

