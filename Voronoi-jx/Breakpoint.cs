using System;
using System.Collections.Generic;

namespace Voronoi_jx
{
    public class Breakpoint
    {

        private TreeItem _leftListEvent;
        private TreeItem _rightListEvent;
        private float _x;
        private float _y;

        public Breakpoint(TreeItem listEvent1, TreeItem listEvent2)
        {
            _leftListEvent = listEvent1;
            _rightListEvent = listEvent2;
        }

        public TreeItem getLeftListEvent()
        {
            return _leftListEvent;
        }

        public TreeItem getRightListEvent()
        {
            return _rightListEvent;
        }

        public float getX()
        {
            return _x;
        }

        public void setX(float x)
        {
            _x = x;
        }

        public float getY()
        {
            return _y;
        }

        public void setY(float y)
        {
            _y = y;
        }


        public bool CalculateBreakpoint(float sweepLine)
        {

            VectorNode p = _leftListEvent.GetNode();
            VectorNode q = _rightListEvent.GetNode();

            float alpha = 0.5f / (p.y - sweepLine);
            float beta = 0.5f / (q.y - sweepLine);
            float A = alpha - beta;
            float B = (-2.0f * (alpha * p.x - beta * q.x));
            float C = (alpha * (p.y * p.y + p.x * p.x - sweepLine * sweepLine) - beta * (q.y * q.y
                       + q.x * q.x - sweepLine * sweepLine));

            if (p.y == q.y)
            {
                _x = -C / B;
                _y = (1.0f / (2.0f * (p.y - sweepLine))) * (((_x - p.x) * (_x - p.x)) +
                p.y * p.y - sweepLine * sweepLine);

            }
            else
            {

                float disc = B * B - 4 * A * C;
                if (disc < 0)
                {
                    return false;
                }
                _x = (-B + 1.0f * (float)Math.Sqrt(B * B - 4 * A * C)) / (2 * A);
                _y = (1.0f / (2.0f * (p.y - sweepLine))) * (((_x - p.x) * (_x - p.x)) +
                p.y * p.y - sweepLine * sweepLine);

            }

            return true;

        }


    }
}

