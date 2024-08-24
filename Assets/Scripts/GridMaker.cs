using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridMaker : MonoBehaviour
{   
    CustomGrid<GridCell> Layout;
    int GridWxH = 15;

    [Header("Editor Code")]
    public bool isEditorGridActive;
    public bool isGridCreated;

    public GameObject PrefabGridUI;
    public GameObject PrefabGridUIParent;
    public Color Red, green;

    public void Start()
    {
        CheckAnchor();
    }
    public void CheckAnchor()
    {
        Layout = new CustomGrid<GridCell>(GridWxH, GridWxH);
        for (int x = 0; x < GridWxH; x++)
        {
            for (int z = 0; z < GridWxH; z++)
            {
                GridCell _tempcell = new GridCell(new Vector2(x, z), new Vector2((x*2) + 1, (z*2) + 1), false);
                Layout.EditIndex(x, z, _tempcell);
            }
        }
        
    }
    
    
    private void OnDrawGizmos()
    {   
        if (isEditorGridActive )
        {
            if( isGridCreated )
            {
                isGridCreated = false;
                for (int x = 0; x < GridWxH; x++)
                {
                    for (int z = 0; z < GridWxH; z++)
                    {
                        var t = Instantiate(PrefabGridUI, PrefabGridUIParent.transform);
                        t.transform.position = ((GridCell)Layout.GetAtIndex(x, z)).GetCoordinates3D();
                        var temp = t.GetComponent<GridUI>();
                        temp.DisplayTxt.text = $"({x},{z})";
                        temp.Btn.onClick.RemoveAllListeners();
                        temp.Btn.onClick.AddListener(delegate (){
                            IsTableintheLayout(x, z);
                        });
                    }

                }
            }
        }
        CheckAnchor();
        for (int x = 0; x < GridWxH; x++)
        {           
            for (int z = 0; z < GridWxH; z++)
            {
                //GridCell _tempcell = new GridCell(new Vector2(x, z), new Vector2(x + 1, z + 1), false);                
                Gizmos.DrawSphere(((GridCell)Layout.GetAtIndex(x, z)).GetCoordinates3D(), 0.2f);
                
            }
           
        }
    }

    public void IsTableintheLayout(int x,int y)
    {
        CustomLogs.CC_Log($"({x},{y})", "cyan");
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
            //_grid[x, y] = new T();
            _grid[x, y] = val;
        }
        public int getWidth() => _width;
        public int getHeight() => _height;
        public T GetAtIndex(int x, int y) => _grid[x, y];
        
    }

    public class GridCell
    {
        Vector2 Index;
        Vector2 AnchorCoordinates;
        bool isOccupied;

        public GridCell(Vector2 index, Vector2 coordinates, bool isOccupied)
        {
            Index = index;
            AnchorCoordinates = coordinates;
            this.isOccupied = isOccupied;
        }

        public Vector2 GetCoordinates2D() => AnchorCoordinates;
        public Vector3 GetCoordinates3D() =>new Vector3(AnchorCoordinates.x,0, AnchorCoordinates.y);
    }
}
