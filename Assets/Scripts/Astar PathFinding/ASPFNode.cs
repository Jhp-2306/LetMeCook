using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASPathFinding
{
    [System.Serializable]
    public class ASPFNode:IHeapItem<ASPFNode>
    {
        [SerializeField]
        private bool m_IsWalkable;
        [SerializeField]
        private Vector3 m_worldPosition;

        public int gridX;
        public int gridY;

        public int Gcost;
        public int Hcost;
        int heapIndex;
        public ASPFNode parent;
        public ASPFNode(bool isWalkable, Vector3 worldPosition, int gridX, int gridY)
        {
            m_IsWalkable = isWalkable;
            m_worldPosition = worldPosition;
            this.gridX = gridX;
            this.gridY = gridY;
        }

        public int Fcost { get=>Gcost+Hcost;  }
        public bool IsWalkable { get=>m_IsWalkable; }
        public Vector3 worldPosition { get=>m_worldPosition;  }
        public int HeapIndex { get => heapIndex; set { heapIndex = value; } }

        public int CompareTo(ASPFNode other)
        {
           int compare=Fcost.CompareTo(other.Fcost);
            if (compare == 0)
            {
                compare=Hcost.CompareTo(other.Hcost);
            }
            return -compare;
            
        }
    }

    public struct Line
    {
        const float VerticalLineGradient = 1e5f;

        float gradient;
        float y_intercept;
        Vector2 pointOnLine_1;
        Vector2 pointOnLine_2;

        float gradientPerpendicular;
        bool approachSide;
        public Line(Vector2 pointOnLine,Vector2 pointPerpendicularToLine)
        {
            float dx=pointOnLine.x - pointPerpendicularToLine.x;
            float dy=pointOnLine.y - pointPerpendicularToLine.y;

            if (dx == 0)
            {
                gradientPerpendicular = VerticalLineGradient;

            }
            else
            {
                gradientPerpendicular=dx/dy;

            }
            if (gradientPerpendicular == 0)
            {
                gradient = VerticalLineGradient;
            }
            else
            {
                gradient = -1 / gradientPerpendicular;
            }
            y_intercept = pointOnLine.y - gradient * pointOnLine.x;
            pointOnLine_1=pointOnLine; 
            pointOnLine_2=pointOnLine+new Vector2(1,gradient);
            approachSide = false;
            approachSide = GetSide(pointPerpendicularToLine);
        }

        bool GetSide(Vector2 p)
        {
            return (p.x-pointOnLine_1.x)*(pointOnLine_2.y-pointOnLine_1.y)>
                (p.y-pointOnLine_1.y)*(pointOnLine_2.x-pointOnLine_1.x);
        }

        public bool HasCrossedLine(Vector2 p)
        {
            return GetSide(p) != approachSide;
        }
        public float DistanceFromPoint(Vector2 p)
        {
            float yInterceptPerpendicular = p.y - gradientPerpendicular * p.x;
            float intersectX = (yInterceptPerpendicular - y_intercept) / (gradient - gradientPerpendicular);
            float intersectY = gradient * intersectX + y_intercept;
            return Vector2.Distance(p, new Vector2(intersectX, intersectY));
        }
        public void DrawWithGizmos(float lenght)
        {
            Vector3 lineDir = new Vector3(1, 0, gradient).normalized;
            Vector3 lineCentre= new Vector3(pointOnLine_1.x,0,pointOnLine_1.y)+Vector3.up;
            Gizmos.DrawLine(lineCentre-lineDir*lenght/2,lineCentre+lineDir*lenght/2);
        }
        }

    public class Path
    {
        public readonly Vector3[] lookPoints;
        public readonly Line[] turnBoundaries;
        public readonly int finishLineIndex;
        public readonly int slowDownIndex;
        public Path(Vector3[] wayPoint,Vector3 startPos,float turnDst, float stoppingDst)
        {
            lookPoints = wayPoint;
            turnBoundaries = new Line[lookPoints.Length];
            finishLineIndex = turnBoundaries.Length - 1;

            Vector2 previousPoint = V3ToV2(startPos);
            for (int i = 0; i < lookPoints.Length; i++)
            {
                Vector2 curremtPoint = V3ToV2(lookPoints[i]);
                Vector2 dirToCurrentPoint = (curremtPoint - previousPoint).normalized;
                Vector2 turnBoundaryPoint = i == finishLineIndex ? curremtPoint : curremtPoint - dirToCurrentPoint * turnDst;
                turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDst);
                previousPoint = turnBoundaryPoint;
            }
            float dstFromEndPoint = 0;
            for (int i = lookPoints.Length - 1; i > 0; i--)
            {
                dstFromEndPoint += Vector3.Distance(lookPoints[i], lookPoints[i - 1]);
                if (dstFromEndPoint > stoppingDst)
                {
                    slowDownIndex = i;
                    break;
                }
            }
            //turnBoundaries = wayPoint;
        }
        Vector2 V3ToV2(Vector3 p)
        {
            return new Vector2(p.x, p.z);
        }

        public void DrawWithGizmos()
        {
           Gizmos.color = Color.black;
            foreach (Vector3 p in lookPoints)
            {
                Gizmos.DrawCube(p + Vector3.up, Vector3.one);
            }
            Gizmos.color = Color.white;
            foreach(Line l in turnBoundaries)
            {
                l.DrawWithGizmos(1);
            }
        }
    }
}
