using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ASPathFinding
{

    public class ASPFGrid : MonoBehaviour
    {
        public bool ShowGizmos;
        public LayerMask notWalkable;
        [SerializeField] 
        private float m_PlayerRadius;
        [SerializeField] 
        private Vector2 m_GridSize;
        ASPFNode[,] grid;

        private const string file_name = "Nav_mesh";

        float nodeDiameter;
        [SerializeField]
        private float gridOffset=0.5f;
        int gridSizeX, gridSizeY;
        bool isGridCreated;
        private void Start()
        {
            nodeDiameter = m_PlayerRadius * 2;
            gridSizeX = Mathf.RoundToInt(m_GridSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(m_GridSize.y / nodeDiameter);
            if(SerializationManager.Load(file_name)==null)
            {
            CreateGrid();
            }
            else
            {
                grid = new ASPFNode[gridSizeX, gridSizeY];
                grid = (ASPFNode[,])SerializationManager.Load(file_name);
                isGridCreated = true;
            }
        }
        
        /// <summary>
        /// Create the Grid in Reference to gridSizeX and gridSizeY
        /// </summary>
        public void CreateGrid()
        {
            grid = new ASPFNode[gridSizeX, gridSizeY];
            Vector3 worldBottomLeft=transform.position-Vector3.right*m_GridSize.x/2-Vector3.forward*m_GridSize.y/2;
            for (int x = 0; x < gridSizeX; x++)
            {
                for(int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + m_PlayerRadius) + Vector3.forward * (y * nodeDiameter + m_PlayerRadius);
                    //bool Walkable=!(Physics.CheckSphere(worldPoint, m_PlayerRadius, notWalkable));
                    bool Walkable=!(Physics.CheckBox(worldPoint, Vector3.one* m_PlayerRadius/2, Quaternion.identity, notWalkable));
                    grid[x,y]=new ASPFNode(Walkable, worldPoint,x,y);
                }
            }
            //Save the Node Data
           saveGridData();
            isGridCreated = true;
        }
        public void UpdateGrid()
        {
            foreach (ASPFNode temp in grid)
            {
                var node = temp;
                node.IsWalkable = !(Physics.CheckBox(node.worldPosition, Vector3.one * m_PlayerRadius / 2, Quaternion.identity, notWalkable));
                grid[node.gridX,node.gridY] = node;
            }
            saveGridData() ;
        }
        void saveGridData()
        {
            SerializationManager.Save(file_name, grid);
        }
        /// <summary>
        /// Only returns number of node (gridSizeX*gridSizeY)
        /// </summary>
        public int MaxSize
        {
            get => gridSizeX * gridSizeY;
        }

        /// <summary>
        /// Returns a List of node which are near to the passed node
        /// note: this only gives the top,bottom,left and right nodes
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>

        public List<ASPFNode> GetNearNodes(ASPFNode node) {
        List<ASPFNode> neighbours= new List<ASPFNode>();
            for(int x = -1;x<=1;x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0||Mathf.Abs(x)+ Mathf.Abs(y) != 1)
                    {
                        //Debug.Log("check here and jump");
                            continue;
                    }
                    
                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if(checkX >= 0 && checkX<gridSizeX&& checkY >= 0 && checkY < gridSizeY)
                        neighbours.Add(grid[checkX,checkY]);
                }
            }
            return neighbours;
        }

        public ASPFNode GetNodeFromWorldPosition(Vector3 worldPosition)
        {
            float PercentX = (worldPosition.x /*+ m_GridSize.x / 2*/) / m_GridSize.x;
            float PercentY = (worldPosition.z /*+ m_GridSize.y / 2*/) / m_GridSize.y;
            PercentX = Mathf.Clamp01(PercentX);
            PercentY = Mathf.Clamp01(PercentY);
            int x=Mathf.RoundToInt((gridSizeX-1)* PercentX); 
            int y=Mathf.RoundToInt((gridSizeY-1)* PercentY); 
            return grid[x,y];
        }
       
        private void OnDrawGizmos()
        {
            if (!ShowGizmos) return;
            Gizmos.DrawWireCube(transform.position, new(m_GridSize.x,1,m_GridSize.y));
            if (grid != null)
            {
                foreach (ASPFNode n in grid)
                {
                    Gizmos.color = (n.IsWalkable) ? Color.green : Color.red;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one *2* (m_PlayerRadius-.1f));
                }
            }
        }

        
    }
}
