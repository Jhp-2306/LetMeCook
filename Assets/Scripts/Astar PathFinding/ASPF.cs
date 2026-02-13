using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASPathFinding
{
    [RequireComponent(typeof(ASPFGrid))]
    public class ASPF : MonoBehaviour
    {

        PathManager _requestManager;
        public static ASPFGrid grid;
        public ASPFGrid AIGrid;
        public bool DebugMode;
        private void Awake()
        {
            grid = GetComponent<ASPFGrid>();
            _requestManager = GetComponent<PathManager>();
        }

        public static void updateGrid()
        {
            grid.UpdateGrid();
        }
        public static void CreateGrid()
        {
            grid.CreateGrid();
        }
        public void StartFindPath(Vector3 _startpt, Vector3 _endpt)
        {
            //Debug.Log(CustomLogs.CC_TagLog("Astar Path Finding(ASPF class)", "Player Moving"));
            StartCoroutine(FindPath(_startpt, _endpt, grid, false));
        }
        public void StartForAIFindPath(Vector3 _startpt, Vector3 _endpt)
        {
            //Debug.Log(CustomLogs.CC_TagLog("Astar Path Finding(ASPF class)", "AI Moving"));
            StartCoroutine(FindPath(_startpt, _endpt, AIGrid, true));
        }
        IEnumerator FindPath(Vector3 startPos, Vector3 targetPos, ASPFGrid _grid, bool isAI)
        {
            
            var _Grid = _grid;
            _Grid.ResetAllCost();
            Vector3[] wayPt = new Vector3[0];
            bool pahtSucces = false;
            ASPFNode startNode = _Grid.GetNodeFromWorldPosition(startPos);
            ASPFNode targetNode = _Grid.GetNodeFromWorldPosition(targetPos);
            //Debug.Log($"Start Position{startNode.worldPosition},EndPosition{targetNode.worldPosition}");
            // Debug.Log(CustomLogs.CC_TagLog($"<color=cyan> ASPF</color>", $"current node Wpos{targetNode.worldPosition}"));
            if (startNode.IsWalkable && targetNode.IsWalkable)
            {
                Heap<ASPFNode> openSet = new Heap<ASPFNode>(_Grid.MaxSize);
                HashSet<ASPFNode> closedSet = new HashSet<ASPFNode>();
                openSet.Add(startNode);
                while (openSet.Count > 0)
                {

                    if (DebugMode && !isAI)
                        Debug.Break();
                    ASPFNode currentNode = openSet.RemoveAtFirst();
                    //Debug.Log(CustomLogs.CC_TagLog($"<color=cyan> ASPF</color>", $"current node Wpos{currentNode.worldPosition}"));
                    closedSet.Add(currentNode);
                    //currentNode.PrintDetails(Color.red);
                    if (currentNode == targetNode)
                    {
                        //RetracePath(startNode, targetNode);
                        pahtSucces = true;
                        break;
                    }
                    //Debug.Log(CustomLogs.CC_TagLog($"<color=cyan> ASPF</color>", $"Grid neighbours{_Grid.GetNearNodes(currentNode).Count}"));
                    foreach (ASPFNode neighbour in _Grid.GetNearNodes(currentNode))
                    {
                      
                        if (!neighbour.IsWalkable || closedSet.Contains(neighbour)) continue;

                        int newCost = currentNode.Gcost + GetDistance(currentNode, neighbour);
                        if (newCost <= neighbour.Gcost || !openSet.Contains(neighbour))
                        {
                            neighbour.Gcost = newCost;
                            neighbour.Hcost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;
                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                            else
                                openSet.UpdateItem(neighbour);
                        }
                        //neighbour.PrintDetails(Color.green);
                    }
                    if (DebugMode && !isAI)
                        yield return null;
                }
            }

            yield return null;
            if (pahtSucces)
            {
                wayPt = RetracePath(startNode, targetNode);
            }
            if (isAI)
                _requestManager.NPCFinishProcessingPath(wayPt, pahtSucces);
            else
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
            Vector3[] waypt = SimplifyPath(path);
            Array.Reverse(waypt);
            return vec.ToArray();

        }

        Vector3[] SimplifyPath(List<ASPFNode> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;
            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
                if (directionNew != directionOld)
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
            int remaining = Mathf.Abs(distx - disty);
            //if (distx > disty)
            //{
            //    return 14 * disty + 10 * (distx - disty);
            //}
            return/* 14 **/ Mathf.Min(distx, disty) + 10 * remaining;
            //return 10 * (distx + disty);
        }
    }
}
