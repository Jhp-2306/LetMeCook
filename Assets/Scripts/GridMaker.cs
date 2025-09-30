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
                GridCell _tempcell = new GridCell(new Vector2(x, z), new Vector2((x* GridSize) + 1, (z* GridSize) + 1), false);
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
            _tempcell.IsOccupied = (Physics.CheckBox(_tempcell.GetCoordinates3D(), Vector3.one * 0.1f, Quaternion.identity, notWalkable));
            Layout.EditIndex((int)_tempcell.GetIndex().x, (int)_tempcell.GetIndex().y, _tempcell);
        }
    }
    public bool ProcessCoordsList(List<Vector3> coordsList)
    {
        foreach (var pos in coordsList)
        {
            if (!processCoords(pos)) return false;
        }
        return true;
    }
    public bool processCoords(Vector3 pos)
    {
        var cell = GetGridFromPos(pos);
        if (cell == null) return false;
        Debug.Log($"Process coords{pos}{cell.GetCoordinates3D()}");
        if (cell.GetCoordinates2D() == new Vector2(pos.x,pos.z) && !cell.IsOccupied) return true;
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

    public void UpdateCellOccupied(Vector3 pos, bool isOcc)
    {
        var t = GetGridFromPos(pos);
        t.IsOccupied = isOcc;
        Layout.EditIndex((int)t.GetIndex().x, (int)t.GetIndex().y, t);
    }
    
    public void UpdateCellOccupied(List<Vector3> Coords, bool isOcc)
    {
        foreach (var pos in Coords)
        {
            UpdateCellOccupied(pos, isOcc);
        }
    }
    public Vector3 GetRoundAnchorPositionFromWorldPosiion(Vector3 pos)
    {
        var temppos= GetIndexFromAnchorPosition(new Vector2(pos.x,pos.z));
        if((int)temppos.x< GridWxH && (int)temppos.y < GridWxH)
        {

        var anchorpos= Layout.GetAtIndex((int)Mathf.Clamp(temppos.x,0,15), (int)Mathf.Clamp(temppos.y, 0, 15)).GetCoordinates2D();
        return new Vector3(anchorpos.x, pos.y, anchorpos.y);
        }
        return Vector3.zero;

    }
    public static Vector2 GetIndexFromAnchorPosition(Vector2 pos)
    {
        return new Vector2(Mathf.RoundToInt((pos.x-1)/ GridSize), Mathf.RoundToInt((pos.y-1)/ GridSize));
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

    public void IsTableintheLayout(int x,int y)
    {
        CustomLogs.CC_Log($"({x},{y})", "cyan");
    }
    public GridCell GetCell(Vector2 pos)
    {
        return Layout.GetAtIndex((int)pos.x, (int)pos.y);
    }
    
    public List<Vector3> GetAvailableNeighbourPosition(Vector3 worldpos)
    {
        var idx= GetIndexFromAnchorPosition(/*GetGridFromPos(worldpos).GetIndex()*/new Vector2(worldpos.x,worldpos.z));
        List<Vector3> availableposition= new List<Vector3>();  
        if(!GetCell(idx+Vector2.up).IsOccupied)availableposition.Add(GetCell(idx+Vector2.up).GetCoordinates3D());
        if(!GetCell(idx+Vector2.left).IsOccupied)availableposition.Add(GetCell(idx+Vector2.left).GetCoordinates3D());
        if(!GetCell(idx+Vector2.right).IsOccupied)availableposition.Add(GetCell(idx+Vector2.right).GetCoordinates3D());
        if(!GetCell(idx+Vector2.down).IsOccupied)availableposition.Add(GetCell(idx+Vector2.down).GetCoordinates3D());
        return availableposition;
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
            _grid = new T[_width,_height];
        }
        public void EditIndex(int x,int y,T val)
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

   

    public class GridCell
    {
        Vector2 Index;
        Vector2 AnchorCoordinates;
        bool isOccupied;
        public bool IsOccupied { get { return isOccupied; } set { isOccupied = value; } }
        public GridCell(Vector2 index, Vector2 coordinates, bool isOccupied)
        {
            Index = index;
            AnchorCoordinates = coordinates;
            this.isOccupied = isOccupied;
        }
        public Vector2 GetIndex() => Index;
        public Vector2 GetCoordinates2D() => AnchorCoordinates;
        public Vector3 GetCoordinates3D() =>new Vector3(AnchorCoordinates.x,0, AnchorCoordinates.y);
        public Vector3 GetCoordinates3D(float yAxisDisplacement) => new Vector3(AnchorCoordinates.x, yAxisDisplacement, AnchorCoordinates.y);
    }
}
