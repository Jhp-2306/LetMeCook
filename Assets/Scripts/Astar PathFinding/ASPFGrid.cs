using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using static GridMaker;


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

        public string file_name = "Nav_mesh";

        float nodeDiameter;
        [SerializeField]
        private Vector3 gridOffset=Vector3.zero;
        int gridSizeX, gridSizeY;
        bool isGridCreated;
        //public GameObject GuiPrefab;
        //public GameObject PrefabGridUIParent;
        
        public void Init()
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
            Vector3 worldBottomLeft=(transform.position+gridOffset)-((Vector3.right*m_GridSize.x)/2)-((Vector3.forward*m_GridSize.y)/2);
            Debug.Log(CustomLogs.CC_TagLog($"<color=cyan>{gameObject.name}</color>", $"{transform.position},{transform.position + gridOffset},"));
            for (int x = 0; x < gridSizeX; x++)
            {
                for(int y = 0; y < gridSizeY; y++)
                {
                    //Vector3 worldPoint = worldBottomLeft + Vector3.right * ((x * nodeDiameter) + m_PlayerRadius) + Vector3.forward * ((y * nodeDiameter) + m_PlayerRadius);
                    Vector3 worldPoint = worldBottomLeft + new Vector3((x * 2) + 1, 0,(y * 2) + 1);
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
        public void UpdateIsWalkableSpecificCell(int x,int y,bool iswalkable)
        {
            grid[x, y].IsWalkable = iswalkable;
            saveGridData() ;
        }
        public void UpdateIsWalkableSpecificCell(Vector3 worldposition, bool iswalkable)
        {
            var node=GetNodeFromWorldPosition(worldposition);
            grid[node.gridX, node.gridY].IsWalkable = iswalkable;
            saveGridData();
        }
        void saveGridData()
        {
            return;//Testing Only
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
            //float PercentX = (worldPosition.x /*+ m_GridSize.x / 2*/) / m_GridSize.x;
            //float PercentY = (worldPosition.z /*+ m_GridSize.y / 2*/) / m_GridSize.y;
            //PercentX = Mathf.Clamp01(PercentX);
            //PercentY = Mathf.Clamp01(PercentY);
            //int x=Mathf.RoundToInt((gridSizeX-1)* PercentX); 
            //int y=Mathf.RoundToInt((gridSizeY-1)* PercentY);
            Vector3 worldBottomLeft = (transform.position + gridOffset) - ((Vector3.right * m_GridSize.x) / 2) - ((Vector3.forward * m_GridSize.y) / 2);
            var _worldposition=worldPosition-worldBottomLeft;
            var t = new Vector3(Mathf.RoundToInt((_worldposition.x - 1) / 2), 0,Mathf.RoundToInt((_worldposition.z - 1) / 2));
            //Debug.Log(CustomLogs.CC_TagLog($"{gameObject.name}",$"{t}"));
            return grid[(int)t.x,(int)t.z];
        }
       public void ResetAllCost()
        {
            foreach (ASPFNode node in grid)
            {
                node.Gcost = 0;
                node.Hcost = 0;
                node.parent = null;
                //node.PrintDetails(Color.white);
            }

        }
        private void OnDrawGizmos()
        {
            if (!ShowGizmos) return;
            Gizmos.DrawWireCube((transform.position+gridOffset), new(m_GridSize.x,1,m_GridSize.y));
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
