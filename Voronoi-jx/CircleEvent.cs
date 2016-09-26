using C5;
using System;

namespace Voronoi_jx
{
    public class CircleEvent : IEvent
    {
        public HalfEdge halfEdge; //arbitrary half edge originating here.

        private IPriorityQueueHandle<IEvent> _handle;
        private float _R; //radius
        private float _X;
        private float _Y;
        private TreeItem _left, _center, _right;

        private EventTree.LeafNode _leftNode, _rightNode, _centerNode;

        public CircleEvent(TreeItem left, TreeItem center, TreeItem right)
        {
            this._left = left;
            this._right = right;
            this._center = center;

            CalculateCircle();
        }

        public CircleEvent(float x, float y)
        {
            _X = x;
            _Y = y;
        }

        public IPriorityQueueHandle<IEvent> GetHandle()
        {
            return _handle;
        }

        public void SetHandle(IPriorityQueueHandle<IEvent> handle)
        {
            _handle = handle;
        }

        public CircleEvent(EventTree.LeafNode left, EventTree.LeafNode center, EventTree.LeafNode right)
        {
            this._leftNode = left;
            this._rightNode = right;
            this._centerNode = center;

            _left = _leftNode.GetListItem().Copy();
            _right = _rightNode.GetListItem().Copy();
            _center = _centerNode.GetListItem().Copy();

            CalculateCircle();
        }


        public TreeItem L()
        {
            return _left;
        }

        public TreeItem C()
        {
            return _center;
        }

        public TreeItem R()
        {
            return _right;
        }

        public EventTree.LeafNode GetLeftLeafNode()
        {
            return _leftNode;
        }

        public EventTree.LeafNode GetCenterLeafNode()
        {
            return _centerNode;
        }

        public EventTree.LeafNode GetRightLeafNode()
        {
            return _rightNode;
        }

        public float GetR()
        {
            return _R;
        }

        public float X()
        {
            return _X;
        }

        public float Y()
        {
            return _Y;
        }


        public float GetDistanceToLine()
        {
            // the bottom of the circle.
            return _Y - _R;
        }



        public bool CalculateCircle()
        {
            VectorNode point1 = _left.GetNode();
            VectorNode point2 = _center.GetNode();
            VectorNode point3 = _right.GetNode();
            float epsilon = 0.000001f;

            if (point2.x == point3.x && point2.x == point1.x)
            {
                return false;
            }
            else
            {
                double grad12 = (point2.y - point1.y) * 1f / (point2.x - point1.x);
                double grad23 = (point3.y - point2.y) * 1f / (point3.x - point2.x);

                if (Math.Abs(grad12 - grad23) < epsilon)
                {
                    return false;
                }
            }

            double normal12;
            double normal23;
            if (point2.y == point3.y)
            {
                point1 = _right.GetNode();
                point3 = _left.GetNode();
            }
            if (Math.Abs(point2.x - point1.x) < epsilon || Math.Abs(point2.y - point1.y) < epsilon
                || Math.Abs(point2.x - point3.x) < epsilon || Math.Abs(point2.y - point3.y) < epsilon
                || Math.Abs(point1.x - point3.x) < epsilon || Math.Abs(point1.y - point3.y) < epsilon)
            {

                return RotateAndCalculateCircle(11.3f);
            }

            else
            {
                if (Math.Abs(point2.x - point1.x) < epsilon || Math.Abs(point2.y - point1.y) < epsilon
                    || Math.Abs(point2.x - point3.x) < epsilon || Math.Abs(point2.y - point3.y) < epsilon
                    || Math.Abs(point1.x - point3.x) < epsilon || Math.Abs(point1.y - point3.y) < epsilon)
                {

                    return RotateAndCalculateCircle(11.3);
                }
                else
                {
                    // first pair perpendicular bisector
                    double grad12 = (point2.y - point1.y) / (point2.x - point1.x);
                    normal12 = -1.0 / grad12;

                    double grad23 = (point3.y - point2.y) / (point3.x - point2.x);
                    normal23 = -1.0 / grad23;

                    double const12 = 0.5 * (point1.y + point2.y) - 0.5 * normal12 * (point1.x + point2.x);
                    double const23 = 0.5 * (point2.y + point3.y) - 0.5 * normal23 * (point2.x + point3.x);


                    double centerX = (const23 - const12) / (normal12 - normal23);
                    double centerY = normal23 * centerX + const23;


                    float radius1 = (float)Math.Sqrt((point1.x - centerX) * (point1.x - centerX) +
                        (point1.y - centerY) * (point1.y - centerY));

                    _R = radius1;
                    _X = (float)centerX;
                    _Y = (float)centerY;
                }
            }

            return true;
        }


        public bool RotateAndCalculateCircle(double degrees)
        {
            double COS = Math.Cos(degrees * Math.PI / 180);
            double SIN = Math.Sin(degrees * Math.PI / 180);

            VectorNode point1 = _left.GetNode();
            VectorNode point2 = _center.GetNode();
            VectorNode point3 = _right.GetNode();

            float p1x = (float)(COS * point1.x - SIN * point1.y);
            float p1y = (float)(SIN * point1.x + COS * point1.y);

            point1 = new VectorNode(p1x, p1y);

            float p2x = (float)(COS * point2.x - SIN * point2.y);
            float p2y = (float)(SIN * point2.x + COS * point2.y);

            point2 = new VectorNode(p2x, p2y);

            float p3x = (float)(COS * point3.x - SIN * point3.y);
            float p3y = (float)(SIN * point3.x + COS * point3.y);

            point3 = new VectorNode(p3x, p3y);



            double normal12;
            double normal23;


            double grad12 = (point2.y - point1.y) / (point2.x - point1.x);
            normal12 = -1.0 / grad12;


            double grad23 = (point3.y - point2.y) / (point3.x - point2.x);
            normal23 = -1.0 / grad23;


            double const12 = 0.5 * (point1.y + point2.y) - 0.5 * normal12 * (point1.x + point2.x);
            double const23 = 0.5 * (point2.y + point3.y) - 0.5 * normal23 * (point2.x + point3.x);


            double centerX = (const23 - const12) / (normal12 - normal23);
            double centerY = normal23 * centerX + const23;


            float radius1 = (float)Math.Sqrt((point1.x - centerX) * (point1.x - centerX) +
                (point1.y - centerY) * (point1.y - centerY));

            float centerX2 = (float)(COS * centerX + SIN * centerY);
            float centerY2 = (float)(-SIN * centerX + COS * centerY);
            _R = radius1;
            _X = centerX2;
            _Y = centerY2;

            return true;
        }

        public static bool DoBreakpointsConverge(CircleEvent circleEvent)
        {
            bool clockwise = (circleEvent.L().GetNode().y - circleEvent.C().GetNode().y)
                * (circleEvent.R().GetNode().x - circleEvent.C().GetNode().x) <=
                (circleEvent.L().GetNode().x - circleEvent.C().GetNode().x)
                * (circleEvent.R().GetNode().y - circleEvent.C().GetNode().y);

            if (clockwise)
                return true;
            else
                return false;
        }

    }
}

