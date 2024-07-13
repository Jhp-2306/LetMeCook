using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMaker : MonoBehaviour
{

   
    CustomGrid<GridCell> Layout;
    public void Start()
    {
        //Layout = new CustomGrid<GridCell>(15,15);
        CheckAnchor();
    }
    public void CheckAnchor()
    {
        Layout = new CustomGrid<GridCell>(15, 15);
        for (int x = 0; x < 15; x++)
        {
            for (int z = 0; z < 15; z++)
            {
                GridCell _tempcell = new GridCell(new Vector2(x, z), new Vector2((x*2) + 1, (z*2) + 1), false);
                Layout.EditIndex(x, z, _tempcell);
            }
        }
        //for (int x = 0; x < 15; x++)
        //{
        //    var String = "";
        //    for (int z = 0; z < 15; z++)
        //    {
        //        //GridCell _tempcell = new GridCell(new Vector2(x, z), new Vector2(x + 1, z + 1), false);                
        //       String=$"{String},{((GridCell)Layout.GetAtIndex(x, z)).GetCoordinates()}";
        //    }
        //    Debug.Log($"<color=red>{String}</color>");
        //}
    }
    private void OnDrawGizmos()
    {        
        CheckAnchor();
        for (int x = 0; x < 15; x++)
        {           
            for (int z = 0; z < 15; z++)
            {
                //GridCell _tempcell = new GridCell(new Vector2(x, z), new Vector2(x + 1, z + 1), false);                
                Gizmos.DrawSphere(((GridCell)Layout.GetAtIndex(x, z)).GetCoordinates3D(), 0.2f);
                
            }
           
        }
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
