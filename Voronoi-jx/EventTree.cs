using System;
using System.Collections.Generic;

namespace Voronoi_jx
{
    public class EventTree
    {
        private double _maxY;
        private TreeNode _root = null;

        public List<HalfEdge> halfEdges = new List<HalfEdge>();


        public EventTree() {/* empty */}

        public TreeNode GetRoot()
        {
            return _root;
        }

        public void SetRoot(TreeNode root)
        {
            _root = root;
            _root.SetParent(null);
        }

        public void setMaxY(double y)
        {
            _maxY = y;
        }

        public TreeNode GetNext(TreeNode current)
        {
            if (current.RChild() != null)
            {
                TreeNode next = current.RChild();
                while (next.LChild() != null)
                {
                    next = next.LChild();
                }
                return next;
            }
            else if (current.Parent() != null && current.Parent().LChild() == current)
            {
                return current.Parent();
            }
            else if (current.Parent() != null)
            {
                TreeNode next = current.Parent();
                while (next.Parent() != null)
                {
                    if (next.Parent().LChild() == next)
                        return next.Parent();
                    next = next.Parent();
                }
                return null;
            }
            else return null;
        }


        public TreeNode GetNextLeaf(TreeNode current)
        {
            TreeNode next = GetNext(current);
            while (next != null)
            {
                if (next is LeafNode)
                {
                    return next;
                }
                next = GetNext(next);
            }
            return null;
        }


        public TreeNode GetPrevious(TreeNode current)
        {
            if (current.LChild() != null)
            {
                TreeNode prev = current.LChild();

                while (prev.RChild() != null)
                {
                    prev = prev.RChild();
                }
                return prev;
            }
            else if (current.Parent() != null && current.Parent().RChild() == current)
            {

                return current.Parent();

            }
            else if (current.Parent() != null)
            {
                TreeNode prev = current.Parent();
                while (prev.Parent() != null)
                {
                    if (prev.Parent().RChild() == prev)
                        return prev.Parent();
                    prev = prev.Parent();
                }
                return null;
            }
            else return null;
        }


        public TreeNode GetPreviousLeaf(TreeNode current)
        {
            TreeNode prev = GetPrevious(current);
            while (prev != null)
            {
                if (prev is LeafNode)
                {
                    return prev;
                }
                prev = GetPrevious(prev);
            }
            return null;
        }

        public TreeNode GetClosest(VoronoiGenerator vg, TreeNode current, SiteEvent siteEvent)
        {
            float x = siteEvent.X();
            float y = siteEvent.Y();
            if ((int)y == (int)_maxY)
            {
                TreeItem le = new TreeItem(siteEvent.V());
                LeafNode evt = new LeafNode(le);
                TreeNode next = _root;
                while (GetNextLeaf(next) != null)
                {
                    next = GetNextLeaf(next);
                }

                HalfEdge h1 = new HalfEdge(((LeafNode)next).GetListItem().GetNode());
                HalfEdge h2 = new HalfEdge(le.GetNode());

                h1.SetTwin(h2);
                h2.SetTwin(h1);
                halfEdges.Add(h1);
                halfEdges.Add(h2);

                if (((LeafNode)next).GetListItem().GetNode().halfEdge == null)
                {
                    ((LeafNode)next).GetListItem().GetNode().halfEdge = h1;
                }
                else
                {
                    HalfEdge n = ((LeafNode)next).GetListItem().GetNode().halfEdge;
                    while (n.Next() != null)
                    {
                        n = n.Next();
                    }
                    n.SetNext(h1);
                }

                ((LeafNode)next).GetListItem().SetHalfEdge(h1);
                le.SetHalfEdge(h1);

                le.GetNode().halfEdge = h2;

                Breakpoint b0;
                b0 = new Breakpoint(((LeafNode)next).GetListItem(), evt.GetListItem());
                BreakpointNode bpNode0 = new BreakpointNode(b0);
                ((LeafNode)next).SetBreakpointNode(bpNode0);
                if (next == _root)
                {
                    next.SetParent(bpNode0);
                    _root = bpNode0;
                    bpNode0.SetLChild(next);
                    bpNode0.SetRChild(evt);
                    evt.SetParent(bpNode0);
                }
                else
                {
                    TreeNode parent = next.Parent();
                    parent.SetRChild(bpNode0);
                    bpNode0.SetParent(parent);
                    bpNode0.SetLChild(next);
                    next.SetParent(bpNode0);
                    bpNode0.SetRChild(evt);
                    evt.SetParent(bpNode0);

                }

                b0.CalculateBreakpoint(y);
                b0.setX((x + next.GetX()) / 2);
                b0.setY(y + 9000 /* close to infinity */);

                CircleEvent ce = new CircleEvent(b0.getX(), b0.getY());

                h1.SetTarget(ce);
                ce.halfEdge = h1;

                vg.GetAllCircleEvents().Add(ce);

                return null;

            }
            else
            {

                current.CalculatePosition(y - 0.0001f);

                if (x > current.GetX())
                {
                    if (current.RChild() != null)
                    {
                        return GetClosest(vg, current.RChild(), siteEvent);
                    }
                    else return current;
                }
                else if (x < current.GetX())
                {
                    if (current.LChild() != null)
                    {
                        return GetClosest(vg, current.LChild(), siteEvent);
                    }
                    else return current;
                }
                else return current;
            }
        }

        public List<CircleEvent> InsertNewSiteEvent(LeafNode current, TreeItem listItem)
        { 
            List<CircleEvent> circleEvents = new List<CircleEvent>();


            //==================== half edge =================
            VectorNode currentVector2 = current.GetListItem().GetNode();
            VectorNode newVector2 = listItem.GetNode();
            HalfEdge h1 = new HalfEdge(currentVector2);
            HalfEdge h2 = new HalfEdge(newVector2);
            h1.SetTwin(h2);
            h2.SetTwin(h1);
            halfEdges.Add(h1);
            halfEdges.Add(h2);
            if (currentVector2.halfEdge == null)
            {
                currentVector2.halfEdge = h1;

            }
            else
            {
                HalfEdge n = currentVector2.halfEdge;
            }

            //of course the new node doesnt have an edge yet.
            newVector2.halfEdge = h2;

            HalfEdge oldHE = current.GetListItem().GetHalfEdge();
            current.GetListItem().SetHalfEdge(h1);
            listItem.SetHalfEdge(h1);

            TreeNode prev = GetPreviousLeaf(current);
            TreeNode next = GetNextLeaf(current);

            LeafNode newNode = new LeafNode(listItem);
            TreeItem copy = ((LeafNode)current).GetListItem().Copy();
            LeafNode copyNode = new LeafNode(copy);

            Breakpoint bp1 = new Breakpoint(((LeafNode)current).GetListItem(), listItem);
            bp1.CalculateBreakpoint(listItem.GetNode().y - 1);


            Breakpoint bp2 = new Breakpoint(listItem, copy);
            bp2.CalculateBreakpoint(listItem.GetNode().y - 1);

            // add two new inner nodes.
            BreakpointNode bp1Node = new BreakpointNode(bp1);
            BreakpointNode bp2Node = new BreakpointNode(bp2);


            current.SetBreakpointNode(bp1Node);
            newNode.SetBreakpointNode(bp2Node);

            //case 1, current has a parent
            if (current.Parent() != null)
            {
                if (current == current.Parent().LChild())
                {
                    current.Parent().SetLChild(bp1Node);
                    bp1Node.SetParent(current.Parent());
                }
                else
                {
                    current.Parent().SetRChild(bp1Node);
                    bp1Node.SetParent(current.Parent());
                }
            }
            else
            {
                SetRoot(bp1Node);
                _root.SetParent(null);
            }

            bp1Node.SetLChild(current);
            current.SetParent(bp1Node);

            bp1Node.SetRChild(bp2Node);
            bp2Node.SetParent(bp1Node);

            bp2Node.SetLChild(newNode);
            newNode.SetParent(bp2Node);

            bp2Node.SetRChild(copyNode);
            copyNode.SetParent(bp2Node);

            if (oldHE != null)
            {
                copyNode.GetListItem().SetHalfEdge(oldHE);
                //copyNode.getListItem().rightHalfEdge = oldHE.mTwin;
                HalfEdge pre = copyNode.GetListItem().GetHalfEdge();

                if (pre.GetFace() != currentVector2)
                {
                    pre = pre.Twin();
                }


                HalfEdge onext = pre.Next();
                pre.SetNext(h1);
                h1.SetPrevious(pre);
                if (onext != null)
                {
                    onext.SetPrevious(h1);
                    h1.SetNext(onext);
                }
            }


            if (prev != null)
            {
                CircleEvent ce = new CircleEvent(((LeafNode)prev), ((LeafNode)current), newNode);
                bool canCalculateCircle = ce.CalculateCircle();
                Breakpoint bpP = new Breakpoint(((LeafNode)prev).GetListItem(), current.GetListItem());

                ((LeafNode)prev).GetBreakpointNode().SetBreakpoint(bpP);

                bp1Node.GetBreakpoint().CalculateBreakpoint(ce.GetDistanceToLine());
                bpP.CalculateBreakpoint(ce.GetDistanceToLine());

                if (canCalculateCircle && CircleEvent.DoBreakpointsConverge(ce))
                {
                    current.SetDisappearEvent(ce);
                    circleEvents.Add(ce);
                }
            }

            if (next != null)
            {
                CircleEvent ce = new CircleEvent(newNode, copyNode, ((LeafNode)next));
                bool canCalculateCircle = ce.CalculateCircle();

                Breakpoint bpN = new Breakpoint(copy, ((LeafNode)next).GetListItem());
                copyNode.SetBreakpointNode((BreakpointNode)GetNext(copyNode));
                copyNode.GetBreakpointNode().SetBreakpoint(bpN);

                bp2Node.GetBreakpoint().CalculateBreakpoint(ce.GetDistanceToLine());
                bpN.CalculateBreakpoint(ce.GetDistanceToLine());

                if (canCalculateCircle && CircleEvent.DoBreakpointsConverge(ce))
                {
                    copyNode.SetDisappearEvent(ce);
                    circleEvents.Add(ce);
                }

            }
            else
            {
                copyNode.GetListItem().SetHalfEdge(h1);

            }

            return circleEvents;
        }


        public List<CircleEvent> RemoveNode(CircleEvent circleEventx, LeafNode prev, LeafNode removeNode, LeafNode next)
        {

            List<CircleEvent> circleEvents = new List<CircleEvent>();

            if (prev == null || next == null)
            {

            }

            //==================== half edge =================
            VectorNode currentVector2 = prev.GetListItem().GetNode();
            VectorNode newVector2 = next.GetListItem().GetNode();
            HalfEdge h1 = new HalfEdge(currentVector2);
            HalfEdge h2 = new HalfEdge(newVector2);
            h1.SetTwin(h2);
            h2.SetTwin(h1);
            halfEdges.Add(h1);
            halfEdges.Add(h2);


            h1.SetTarget(circleEventx);
            Breakpoint bp1 = new Breakpoint(prev.GetListItem(), next.GetListItem());

            if (prev.GetBreakpointNode() != null && prev.GetBreakpointNode() == removeNode.Parent())
            {
                prev.SetBreakpointNode(removeNode.GetBreakpointNode());
            }
            prev.GetBreakpointNode().SetBreakpoint(bp1);


            HalfEdge oldEdgeP = prev.GetListItem().GetHalfEdge();
            if (oldEdgeP.GetFace() != prev.GetListItem().GetNode())
            {
                oldEdgeP = oldEdgeP.Twin();
            }
            HalfEdge n = oldEdgeP.Next();
            oldEdgeP.SetNext(h1);
            h1.SetPrevious(oldEdgeP);
            if (n != null)
            {
                h1.SetNext(n);
                n.SetPrevious(h1);
            }

            HalfEdge oldEdgeN = next.GetListItem().GetHalfEdge();
            if (oldEdgeN.GetFace() != next.GetListItem().GetNode())
            {
                oldEdgeN = oldEdgeN.Twin();
            }

            n = oldEdgeN.Next();

            oldEdgeN.SetNext(h2);
            h2.SetPrevious(oldEdgeN);
            if (n != null)
            {
                h2.SetNext(n);
                n.SetPrevious(h2);

            }


            prev.GetListItem().SetHalfEdge(h1);

            //case where there is no grandparent
            if (removeNode.Parent().Parent() == null)
            {
                //must be root
                if (removeNode.Parent().LChild() == removeNode)
                {
                    _root = removeNode.Parent().RChild();

                }
                else
                    _root = removeNode.Parent().LChild();

                _root.SetParent(null);
            }
            else
            {
                TreeNode grandparent = removeNode.Parent().Parent();
                TreeNode parent = removeNode.Parent();
                if (removeNode == removeNode.Parent().LChild())
                {
                    if (removeNode.Parent() == grandparent.LChild())
                    {
                        grandparent.SetLChild(parent.RChild());
                        parent.RChild().SetParent(grandparent);
                    }
                    else
                    {
                        grandparent.SetRChild(parent.RChild());
                        parent.RChild().SetParent(grandparent);
                    }
                }

                //remove node is right child of parent...
                else
                {
                    if (removeNode.Parent() == grandparent.LChild())
                    {
                        grandparent.SetLChild(parent.LChild());
                        parent.LChild().SetParent(grandparent);
                    }
                    else
                    {
                        grandparent.SetRChild(parent.LChild());
                        parent.LChild().SetParent(grandparent);

                    }
                }
                removeNode.SetParent(null);
            }


            TreeNode nextNextTree = GetNextLeaf(next);
            if (nextNextTree != null)
            {
                LeafNode nextNext = (LeafNode)nextNextTree;
                CircleEvent circleEvent = new CircleEvent(prev, next, nextNext);
                bool canCalculateCircle = circleEvent.CalculateCircle();

                Breakpoint b2;
                b2 = new Breakpoint(next.GetListItem(), nextNext.GetListItem());

                next.GetBreakpointNode().SetBreakpoint(b2);

                bp1.CalculateBreakpoint(circleEvent.GetDistanceToLine());

                b2.CalculateBreakpoint(circleEvent.GetDistanceToLine());
                if (canCalculateCircle && CircleEvent.DoBreakpointsConverge(circleEvent))
                {

                    next.SetDisappearEvent(circleEvent);

                    circleEvents.Add(circleEvent);
                }
            }

            TreeNode prevPrevTree = GetPreviousLeaf(prev);
            if (prevPrevTree != null)
            {
                LeafNode prevPrev = (LeafNode)prevPrevTree;

                CircleEvent circleEvent = new CircleEvent(prevPrev, prev, next);
                bool canCalculateCircle = circleEvent.CalculateCircle();

                Breakpoint b2;
                b2 = new Breakpoint(prevPrev.GetListItem(), prev.GetListItem());
                prevPrev.GetBreakpointNode().SetBreakpoint(b2);
                bp1.CalculateBreakpoint(circleEvent.GetDistanceToLine());

                b2.CalculateBreakpoint(circleEvent.GetDistanceToLine());
                if (canCalculateCircle && CircleEvent.DoBreakpointsConverge(circleEvent))
                {

                    prev.SetDisappearEvent(circleEvent);

                    circleEvents.Add(circleEvent);
                }
            }

            return circleEvents;
        }

        public interface TreeNode
        {
            float GetX();

            float GetY();

            TreeNode LChild();

            void SetLChild(TreeNode child);

            void SetRChild(TreeNode child);

            TreeNode RChild();

            TreeNode Parent();

            void SetParent(TreeNode parent);

            void CalculatePosition(float sweepLine);

        }

        public class BreakpointNode : TreeNode
        {

            private Breakpoint _breakpoint;
            private TreeNode _leftChild;
            private TreeNode _rightChild;
            private TreeNode _parent = null;

            public BreakpointNode(Breakpoint breakpoint)
            {
                _breakpoint = breakpoint;
            }

            public Breakpoint GetBreakpoint()
            {
                return _breakpoint;
            }

            public void SetBreakpoint(Breakpoint bp)
            {
                _breakpoint = bp;
            }

            public float GetX()
            {
                return _breakpoint.getX();
            }

            public float GetY()
            {
                return _breakpoint.getY();
            }

            public TreeNode LChild()
            {
                return _leftChild;
            }

            public void SetLChild(TreeNode child)
            {
                _leftChild = child;
            }

            public void SetRChild(TreeNode child)
            {
                _rightChild = child;
            }

            public TreeNode RChild()
            {
                return _rightChild;
            }

            public TreeNode Parent()
            {
                return _parent;
            }

            public void SetParent(TreeNode parent)
            {
                _parent = parent;
            }

            public void CalculatePosition(float sweepLine)
            {
                _breakpoint.CalculateBreakpoint(sweepLine);
            }

        }

        public class LeafNode : TreeNode
        {

            private TreeItem _listItem;
            private CircleEvent _disappearEvent;
            private TreeNode _parent;
            private BreakpointNode _breakpoint;

            public LeafNode(TreeItem item)
            {
                _listItem = item;
            }

            public TreeItem GetListItem()
            {
                return _listItem;
            }

            public void SetBreakpointNode(BreakpointNode bpn)
            {
                _breakpoint = bpn;
            }

            public BreakpointNode GetBreakpointNode()
            {
                return _breakpoint;
            }

            public void SetDisappearEvent(CircleEvent evt)
            {
                _disappearEvent = evt;
            }

            public CircleEvent GetDisappearEvent()
            {
                return _disappearEvent;
            }


            public float GetX()
            {
                return _listItem.GetNode().x;
            }


            public float GetY()
            {
                return _listItem.GetNode().y;
            }

            public TreeNode LChild()
            {
                return null;
            }

            public void SetLChild(TreeNode child)
            {
                // do nothing.
            }

            public void SetRChild(TreeNode child)
            {
                // do nothing.
            }

            public TreeNode RChild()
            {
                return null;
            }

            public TreeNode Parent()
            {
                return _parent;
            }

            public void SetParent(TreeNode parent)
            {
                _parent = parent;
            }

            public void CalculatePosition(float sweepLine)
            {
                // do nothing
            }
        }
    }
}

