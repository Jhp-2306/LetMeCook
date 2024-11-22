using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ASPathFinding
{
    [RequireComponent(typeof(ASPFGrid))]
    public class ASPF : MonoBehaviour
    {
        
        PathManager _requestManager;
        ASPFGrid grid;
        private void Awake()
        {
            grid = GetComponent<ASPFGrid>();
            _requestManager = GetComponent<PathManager>();
        }
      
        public void StartFindPath(Vector3 _startpt,Vector3 _endpt)
        {
            StartCoroutine(FindPath(_startpt, _endpt));
        }
        IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Vector3[] wayPt=new Vector3[0];
            bool pahtSucces = false;
            ASPFNode startNode = grid.GetNodeFromWorldPosition(startPos);
            ASPFNode targetNode = grid.GetNodeFromWorldPosition(targetPos);

            if(startNode.IsWalkable&&targetNode.IsWalkable) { 
            Heap<ASPFNode> openSet = new Heap<ASPFNode>(grid.MaxSize);
            HashSet<ASPFNode> closedSet = new HashSet<ASPFNode>();
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                ASPFNode currentNode = openSet.RemoveAtFirst();
                closedSet.Add(currentNode);
                if (currentNode == targetNode)
                {
                    //RetracePath(startNode, targetNode);
                    pahtSucces=true;
                    break;
                }
                foreach (ASPFNode neighbour in grid.GetNearNodes(currentNode))
                {
                    if (!neighbour.IsWalkable || closedSet.Contains(neighbour)) continue;

                    int newCost = currentNode.Gcost + GetDistance(currentNode, neighbour);
                    if (newCost < neighbour.Gcost || !openSet.Contains(neighbour))
                    {
                        neighbour.Gcost = newCost;
                        neighbour.Hcost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;
                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }else
                                openSet.UpdateItem(neighbour);
                        }
                }
            }
            }

            yield return null;
            if(pahtSucces)
            {
                wayPt = RetracePath(startNode, targetNode);
            }
            _requestManager.FinishProcessingPath(wayPt, pahtSucces);
        }

        Vector3[] RetracePath(ASPFNode startnode, ASPFNode targetNode)
        {
            List<ASPFNode> path = new List<ASPFNode>();
            ASPFNode currentNode = targetNode;
            List<Vector3> vec = new List<Vector3>();
            while (currentNode != startnode)
            {
                path.Add(currentNode);
                vec.Add(currentNode.worldPosition);
                currentNode = currentNode.parent;
            }
            vec.Reverse();
            //Vector3[] waypt=SimplifyPath(path);
            //Array.Reverse(waypt);
            return vec.ToArray();
           
        }

        Vector3[] SimplifyPath(List<ASPFNode> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;
            for (int i = 1; i < path.Count; i++) {
            Vector2 directionNew= new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
                if(directionNew!=directionOld)
                {
                    waypoints.Add(path[i].worldPosition);
                }
                directionOld = directionNew;
            }
            return waypoints.ToArray();
        }
        int GetDistance(ASPFNode startnode, ASPFNode targetNode)
        {
            int distx = Mathf.Abs(startnode.gridX - targetNode.gridX);
            int disty = Mathf.Abs(startnode.gridY - targetNode.gridY);

            if (distx > disty)
            {
                return 14 * disty + 10 * (distx - disty);
            }
            return 14 * distx + 10 * (disty - distx);
        }
    }
}
