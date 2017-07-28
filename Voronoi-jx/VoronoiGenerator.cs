using System;
using System.Collections.Generic;
using C5; // for the priority queue

/***********************************************************************
 * 
 *                --------- Voronoi Generator ---------
 * Generates the Voronoi Diagram from a given list of (x,y) coordinates,
 * using Fortune's Algorithm.
 * To do this, the coordinates are wrapped in SiteEvent objects, and
 * pushed into a priority queue.
 * Each SiteEvent is removed from the queue in order (ordered by x, then y),
 * and added to an EventTree (binary tree). Adjacent SiteEvents are
 * calculated from the binary tree, and if a CircleEvent (equidistant point
 * between 3 coordinates) is found, a CircleEvent is added to the Priority
 * Queue. Once a Circle Event is removed from the priority queue, a point
 * on the Voronoi Diagram is found. This is repeated until the queue is 
 * empty.
 * *********************************************************************/
namespace Voronoi_jx
{
    /// <summary>
    /// Comparer for IEvents. 
    /// </summary>
    public class EventComparer : IComparer<IEvent>
    {
        public int Compare(IEvent o1, IEvent o2)
        {
            if (o1.GetDistanceToLine() < o2.GetDistanceToLine()) return 1;
            else if (o1.GetDistanceToLine() > o2.GetDistanceToLine()) return -1;
            else
            {
                if (o1.X() < o2.X()) return -1;
                else if (o1.X() > o2.X()) return 1;
                else return 0;
            }
        }
    }

    public class VoronoiGenerator
    {

        private List<VectorNode> _nodeList;

        IntervalHeap<IEvent> _eventQueue;

        private List<CircleEvent> _allCircleEvents = new List<CircleEvent>();


        private float _openEdgeLimit = 9000f;


        private EventTree _eventTree = new EventTree();

        public VoronoiGenerator(List<VectorNode> nodeList)
        {
            _nodeList = nodeList;
            _eventQueue = new IntervalHeap<IEvent>(new EventComparer());

            RemoveDuplicatePoints();
            foreach (var node in _nodeList)
            {
                var se = new SiteEvent(node);
                IPriorityQueueHandle<IEvent> h = null;
                _eventQueue.Add(ref h, se);
                se.SetHandle(h);
            }



        }

        public void Rebuild()
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            _eventQueue = null;
            _eventTree = new EventTree();
            _allCircleEvents.Clear();
            _eventQueue = new IntervalHeap<IEvent>(new EventComparer());

            RemoveDuplicatePoints();
            foreach (var node in _nodeList)
            { 
                node.halfEdge = null;
                var se = new SiteEvent(node);
                IPriorityQueueHandle<IEvent> h = null;
                _eventQueue.Add(ref h, se);
                se.SetHandle(h);
            }
            CreateDiagram();
            watch.Stop();
        }

        public List<VectorNode> GetNodes()
        {
            return _nodeList;
        }

        public List<CircleEvent> GetAllCircleEvents()
        {
            return _allCircleEvents;
        }

        public void SetOpenEdgeLength(float length)
        {
            _openEdgeLimit = Math.Abs(length);
        }



        public void CreateDiagram()
        {
            while (_eventQueue.Count != 0)
            {
                IEvent nextEvent = _eventQueue.DeleteMin();

                //create the initial node
                if (_eventTree.GetRoot() == null)
                {
                    _eventTree.SetRoot(new EventTree.LeafNode(new TreeItem(((SiteEvent)nextEvent).V())));
                    _eventTree.setMaxY(((SiteEvent)nextEvent).Y());

                }
                else
                {
                    if (nextEvent is SiteEvent)
                    {
                        HandleSiteEvent((SiteEvent)nextEvent);
                    }
                    else
                    {

                        HandleCircleEvent((CircleEvent)nextEvent);
                        _allCircleEvents.Add((CircleEvent)nextEvent);
                    }
                }

          
            }

            //remaining breakpoints...
            FinishUp();
        }


        private void HandleSiteEvent(SiteEvent siteEvent)
        {

            EventTree.TreeNode node = _eventTree.GetClosest(this, _eventTree.GetRoot(), siteEvent);


            if (node == null || node is EventTree.BreakpointNode)
            {
                return;
            }
            EventTree.LeafNode closest = (EventTree.LeafNode)node;

            if (closest.GetDisappearEvent() != null)
            {
                _eventQueue.Delete(closest.GetDisappearEvent().GetHandle());

                closest.SetDisappearEvent(null);
            }

            List<CircleEvent> circleEvents2 = _eventTree.InsertNewSiteEvent(closest, new TreeItem(siteEvent.V()));
            foreach (var ce in circleEvents2)
            {
                IPriorityQueueHandle<IEvent> h = null;
                _eventQueue.Add(ref h, ce);
                ce.SetHandle(h);

            }
        }


        private void HandleCircleEvent(CircleEvent circleEvent)
        {

            HalfEdge CL = circleEvent.L().GetHalfEdge();

            if (CL.GetFace() == circleEvent.L().GetNode())
            {
                CL = CL.Twin();
            }

            HalfEdge CR = circleEvent.C().GetHalfEdge();
            if (CR.GetFace() == circleEvent.R().GetNode())
            {
                CR = CR.Twin();

            }

            HalfEdge RC = CR.Twin();
            RC.SetTarget(circleEvent);
            CL.SetTarget(circleEvent);

            circleEvent.halfEdge = CR;


            EventTree.LeafNode prev = (EventTree.LeafNode)_eventTree.GetPreviousLeaf(circleEvent.GetCenterLeafNode());
            EventTree.LeafNode next = (EventTree.LeafNode)_eventTree.GetNextLeaf(circleEvent.GetCenterLeafNode());

            if (prev != null)
            {
                if (prev.GetDisappearEvent() != null)
                {
                    _eventQueue.Delete(prev.GetDisappearEvent().GetHandle());
                    prev.SetDisappearEvent(null);
                }
            }

            if (next != null)
            {
                if (next.GetDisappearEvent() != null)
                {
                    _eventQueue.Delete(next.GetDisappearEvent().GetHandle());
                    next.SetDisappearEvent(null);

                }
            }

            List<CircleEvent> newCircles = _eventTree.RemoveNode(circleEvent, prev, circleEvent.GetCenterLeafNode(), next);
            if (newCircles != null)
            {
                foreach (CircleEvent ce in newCircles)
                {
                    IPriorityQueueHandle<IEvent> h = null;
                    _eventQueue.Add(ref h, ce);
                    ce.SetHandle(h);
                }
            }

        }


        private void RemoveDuplicatePoints()
        {
            if (_nodeList == null)
                return;
            Dictionary<long, VectorNode> compressor = new Dictionary<long, VectorNode>();
            foreach (VectorNode n in _nodeList)
            {
                compressor[n.GetHashCode()] = n;
            }
            _nodeList.Clear();
            _nodeList.AddRange(compressor.Values);

        }

        private void FinishUp()
        {
            GetFinalNodePoint(_eventTree.GetRoot());
        }


        private void GetFinalNodePoint(EventTree.TreeNode node)
        {


            if (node is EventTree.LeafNode)
            {
                if (((EventTree.LeafNode)node).GetBreakpointNode() == null)
                    return;
                Breakpoint b = ((EventTree.LeafNode)node).GetBreakpointNode().GetBreakpoint();


                VectorNode n1 = b.getLeftListEvent().GetHalfEdge().GetFace();
                VectorNode n2 = b.getLeftListEvent().GetHalfEdge().Twin().GetFace();


                float centerx = 0.5f * (b.getLeftListEvent().GetHalfEdge().GetFace().x + b.getLeftListEvent().GetHalfEdge().Twin().GetFace().x);
                float centery = 0.5f * (b.getLeftListEvent().GetHalfEdge().GetFace().y + b.getLeftListEvent().GetHalfEdge().Twin().GetFace().y);

                if (n1.y == n2.y)
                {
                    HalfEdge he = b.getLeftListEvent().GetHalfEdge();
                    CircleEvent ce = new CircleEvent(centerx, -_openEdgeLimit /* neg infinity */);
                    if (he.GetTarget() == null)
                    {
                        he.SetTarget(ce);
                        ce.halfEdge = he;
                    }
                    else
                    {
                        he.Twin().SetTarget(ce);
                        ce.halfEdge = he.Twin();
                    } 
                    _allCircleEvents.Add(ce);
                }
                else
                {

                    float grad = (n2.y - n1.y) * 1.0f / (n2.x - n1.x);
                    float realGrad = -1.0f / grad;

                    float constant = centery - realGrad * centerx;

                    float bpx = b.getX() - centerx;
                    float bpy = b.getY() - centery;

                    //if x = bpx...
                    float testx = centerx + 10000f;
                    float testy = testx * realGrad + constant;
                    CircleEvent ce;
                    if (testx * bpx + testy * bpy > 0)
                        ce = new CircleEvent(testx, testy);
                    else
                        ce = new CircleEvent(centerx - 10000, (centerx - 10000) * realGrad + constant);


                    HalfEdge he = b.getLeftListEvent().GetHalfEdge();
                    if (he.GetFace() != b.getLeftListEvent().GetNode())
                    {
                        he = he.Twin();
                    }


                    if (he.GetTarget() == null)
                    {
                        he.SetTarget(ce);
                    }
                    else if (he.Twin().GetTarget() == null)
                    {
                        he.Twin().SetTarget(ce);
                    }
                    else
                    {
                        // big problem... should never happen
                    }
                    _allCircleEvents.Add(ce);
                }

                return;
            }
            else
            {
                Breakpoint b = ((EventTree.BreakpointNode)node).GetBreakpoint();

                b.CalculateBreakpoint(_openEdgeLimit);


                if (node.LChild() != null)
                {
                    GetFinalNodePoint(node.LChild());

                }
                if (node.RChild() != null)
                {
                    GetFinalNodePoint(node.RChild());
                }
            }
        }

        public List<DelaunayEdge> generateDelaunayGraph()
        {
            List<DelaunayEdge> dEdges = new List<DelaunayEdge>();

            foreach (HalfEdge he in _eventTree.halfEdges)
            {
                if (he.GetTarget().GetHashCode() >= he.Twin().GetTarget().GetHashCode())
                {
                    dEdges.Add(new DelaunayEdge(he.GetFace(), he.Twin().GetFace()));
                }

            }

            return dEdges;
        }
    }
}

