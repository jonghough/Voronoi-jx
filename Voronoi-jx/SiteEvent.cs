using System;
using C5;

namespace Voronoi_jx
{
    public class SiteEvent : IEvent
    {
        private IPriorityQueueHandle<IEvent> _handle;
        private VectorNode _vector;
        public SiteEvent(VectorNode vector)
        {
            _vector = vector;
        }

        public VectorNode V()
        {
            return _vector;
        }

        public float X()
        {
            return _vector.x;
        }

        public float Y()
        {
            return _vector.y;
        }

        public float GetDistanceToLine()
        {
            return Y();
        }

        public IPriorityQueueHandle<IEvent> GetHandle()
        {
            return _handle;
        }

        public void SetHandle(IPriorityQueueHandle<IEvent> handle)
        {
            _handle = handle;
        }
    }
}

