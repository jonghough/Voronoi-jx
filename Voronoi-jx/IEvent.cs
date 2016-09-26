using C5;
using System;

namespace Voronoi_jx
{
    public interface IEvent
    {
        float X();

        float Y();

        float GetDistanceToLine();

        IPriorityQueueHandle<IEvent> GetHandle();
        void SetHandle(IPriorityQueueHandle<IEvent> handle);
    }
}

