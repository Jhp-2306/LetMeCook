using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridMaker : MonoBehaviour
{
    CustomGrid<GridCell> Layout;
    int GridWxH = 15;
    static float GridSize = 2;

    [Header("Editor Code")]
    public bool isEditorGridActive;
    public bool isGridCreated;

    public GameObject PrefabGridUI;
    public GameObject PrefabGridUIParent;
    public Color Red, green;
    public LayerMask notWalkable;
    public LayerMask floor;
    public GameObject GridVisual;
    public void Start()
    {
        CheckAnchor();
        UpdateIsOccupied();
    }
    public void CheckAnchor()
    {
        Layout = new CustomGrid<GridCell>(GridWxH, GridWxH);
        for (int x = 0; x < GridWxH; x++)
        {
            for (int z = 0; z < GridWxH; z++)
            {
                var coods = new Vector2((x * GridSize) + 1, (z * GridSize) + 1);
                RaycastHit hit;
                string tag = "";
                if (Physics.Raycast(new Vector3(coods.x, 1, coods.y), Vector3.up * -1, out hit, 10f, floor)){
                    tag = hit.transform.tag;
                    //Debug.Log("Tag Set: " + tag);
                }
                GridCell _tempcell = new GridCell(new Vector2(x, z), coods, CellStatus.None, tag);
                Debug.Log("Tag Set: " + tag);
                Layout.EditIndex(x, z, _tempcell);
            }
        }

    }

    public void UpdateIsOccupied()
    {
        Debug.Log("Check ");
        var templist = Layout.GetAllCellsInList();
        foreach (var _tempcell in templist)
        {
            _tempcell.IsOccupied = (Physics.CheckBox(_tempcell.GetCoordinates3D(), Vector3.one * 0.1f, Quaternion.identity, notWalkable) ? CellStatus.Occupied : CellStatus.None);
            Layout.EditIndex((int)_tempcell.GetIndex().x, (int)_tempcell.GetIndex().y, _tempcell);
        }
    }
    //public bool ProcessCoordsList(List<Vector3> coordsList)
    //{
    //    foreach (var pos in coordsList)
    //    {
    //        if (!processCoords(pos)) return false;
    //    }
    //    return true;
    //}
    public bool processCoords(Vector3 pos,string tag)
    {
        var cell = GetGridFromPos(pos);
        //var fwdcell = GetGridFromPos(ForwardPos);
        if (cell == null) return false;
        //if (fwdcell == null) return false;
        Debug.Log($"Process coords{pos}{cell.GetCoordinates3D()}");
        if (cell.GetCoordinates2D() == new Vector2(pos.x, pos.z) && cell.IsOccupied == CellStatus.None&&cell.Tag==tag) return true;
        return false;
    }
    public GridCell GetGridFromPos(Vector3 pos)
    {
        foreach (var t in Layout.GetAllCellsInList())
        {
            if (t.GetCoordinates2D() == new Vector2(pos.x, pos.z)) return t;
        }
        return null;
    }

    public void UpdateCellOccupied(Vector3 pos, CellStatus isOcc, Vector3 fwdpos)
    {
        var t = GetGridFromPos(pos);
        t.IsOccupied = isOcc;
        Layout.EditIndex((int)t.GetIndex().x, (int)t.GetIndex().y, t);
        t = GetGridFromPos(fwdpos);
        t.IsOccupied = CellStatus.ParticalOccupied;
    }

    //public void UpdateCellOccupied(List<Vector3> Coords, CellStatus isOcc)
    //{
    //    foreach (var pos in Coords)
    //    {
    //        UpdateCellOccupied(pos, isOcc);
    //    }
    //}
    public Vector3 GetRoundAnchorPositionFromWorldPosiion(Vector3 pos)
    {
        var temppos = GetIndexFromAnchorPosition(new Vector2(pos.x, pos.z));
        if ((int)temppos.x < GridWxH && (int)temppos.y < GridWxH)
        {

            var anchorpos = Layout.GetAtIndex((int)Mathf.Clamp(temppos.x, 0, 15), (int)Mathf.Clamp(temppos.y, 0, 15)).GetCoordinates2D();
            return new Vector3(anchorpos.x, pos.y, anchorpos.y);
        }
        return Vector3.zero;

    }
    public static Vector2 GetIndexFromAnchorPosition(Vector2 pos)
    {
        return new Vector2(Mathf.RoundToInt((pos.x - 1) / GridSize), Mathf.RoundToInt((pos.y - 1) / GridSize));
    }
    //private void OnDrawGizmos()
    //{
    //    //if (isEditorGridActive)
    //    //{
    //    //    if (isGridCreated)
    //    //    {
    //    //        isGridCreated = false;
    //    //        for (int x = 0; x < GridWxH; x++)
    //    //        {
    //    //            for (int z = 0; z < GridWxH; z++)
    //    //            {
    //    //                var t = Instantiate(PrefabGridUI, PrefabGridUIParent.transform);
    //    //                t.transform.position = ((GridCell)Layout.GetAtIndex(x, z)).GetCoordinates3D();
    //    //                var temp = t.GetComponent<GridUI>();
    //    //                temp.DisplayTxt.text = $"({x},{z})";
    //    //                temp.Btn.onClick.RemoveAllListeners();
    //    //                temp.Btn.onClick.AddListener(delegate ()
    //    //                {
    //    //                    IsTableintheLayout(x, z);
    //    //                });
    //    //            }

    //    //        }
    //    //    }
    //    //}
    //    //CheckAnchor();
    //    for (int x = 0; x < GridWxH; x++)
    //    {
    //        for (int z = 0; z < GridWxH; z++)
    //        {
    //            //GridCell _tempcell = new GridCell(new Vector2(x, z), new Vector2(x + 1, z + 1), false);
    //            if(Layout.GetAtIndex(x,z).IsOccupied)
    //            Gizmos.DrawSphere(((GridCell)Layout.GetAtIndex(x, z)).GetCoordinates3D(), 0.2f);
    //            else
    //            Gizmos.DrawCube(((GridCell)Layout.GetAtIndex(x, z)).GetCoordinates3D(),Vector3.one* 0.2f);


    //        }

    //    }
    //}

    public void IsTableintheLayout(int x, int y)
    {
        CustomLogs.CC_Log($"({x},{y})", "cyan");
    }
    public GridCell GetCell(Vector2 pos)
    {
        return Layout.GetAtIndex((int)pos.x, (int)pos.y);
    }

    public List<Vector3> GetAvailableNeighbourPosition(Vector3 worldpositions)
    {
        var idx = GetIndexFromAnchorPosition(/*GetGridFromPos(worldpos).GetIndex()*/new Vector2(worldpositions.x, worldpositions.z));
        List<Vector2> dir = new List<Vector2>() { Vector2.up, Vector2.left, Vector2.right, Vector2.down };
        List<Vector3> availableposition = new List<Vector3>();
        foreach (Vector2 pos in dir)
        {
            if (GetCell(idx + pos).IsOccupied == CellStatus.None)
                availableposition.Add(GetCell(idx + pos).GetCoordinates3D());
        }
        return availableposition;
    }

    public bool CanPlaceHere(Vector3 pos, Vector3 fwdpos)
    {
        var cell = GetGridFromPos(pos);
        var fwdcell = GetGridFromPos(fwdpos);
        if (cell == null) return false;
        if (fwdcell == null) return false;
        Debug.Log($"Can Place Here {pos}{cell.GetCoordinates3D()}");
        if (/*cell.GetCoordinates2D() == new Vector2(pos.x, pos.z) && NEED TO CHECK WHY I PUT THIS CONDITION*/
            cell.IsOccupied == CellStatus.None)
        {
            if (fwdcell.IsOccupied == CellStatus.Occupied) return false;
            return true;
        }
        return false;
    }

    public class CustomGrid<T>
    {
        int _width;
        int _height;
        T[,] _grid;

        public CustomGrid(int width, int height)
        {
            _width = width;
            _height = height;
            _grid = new T[_width, _height];
        }
        public void EditIndex(int x, int y, T val)
        {
            _grid[x, y] = val;
        }
        public int getWidth() => _width;
        public int getHeight() => _height;
        public T GetAtIndex(int x, int y) => _grid[x, y];
        public List<T> GetAllCellsInList()
        {
            List<T> cells = new List<T>();
            foreach (var cell in _grid)
            {
                cells.Add(cell);
            }
            return cells;
        }

    }

    public enum CellStatus
    {
        None = 0,
        ParticalOccupied = 1,
        Occupied

    }
    public class GridCell
    {
        private Vector2 Index;
        private Vector2 AnchorCoordinates;
        //bool isOccupied;//TODO: Update this variable with CellStatus
        private CellStatus isOccupied;
        private string tag;
        public string Tag { get => tag; }
        public CellStatus IsOccupied { get { return isOccupied; } set { isOccupied = value; } }
        public GridCell(Vector2 index, Vector2 coordinates, CellStatus isOccupied, string _tag)
        {
            Index = index;
            AnchorCoordinates = coordinates;
            this.isOccupied = isOccupied;
            this.tag = _tag;
        }
        public Vector2 GetIndex() => Index;
        public Vector2 GetCoordinates2D() => AnchorCoordinates;
        public Vector3 GetCoordinates3D() => new Vector3(AnchorCoordinates.x, 0, AnchorCoordinates.y);
        public Vector3 GetCoordinates3D(float yAxisDisplacement) => new Vector3(AnchorCoordinates.x, yAxisDisplacement, AnchorCoordinates.y);
    }
}
