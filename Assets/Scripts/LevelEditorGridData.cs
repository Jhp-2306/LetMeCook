using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static GridMaker;

public class LevelEditorGridData : MonoBehaviour
{
   
    [System.Serializable]
    public class LE_CellData
    {
        public string name;
        GameObject go;
        Vector2 idx;
        Vector3 Coords;
        bool isTable;
        bool ispath;
        bool isEqiement;

        public LE_CellData(Vector2 idx, Vector3 coords, GameObject go, bool isTable = false, bool ispath = false, bool isEqiement = false)
        {
            name = $"({idx.x},{idx.y})";
            this.go = go;
            this.idx = idx;
            Coords = coords;
            this.isTable = isTable;
            this.ispath = ispath;
            this.isEqiement = isEqiement;
        }
        public GameObject GetCellsGameObject() => go;
    }
    [System.Serializable]
    public class Sector
    {
        public List<GameObject> cells;

        //public void AddtoCells()
        //{
        //    cells.EditIndex
        //}
        public void init()
        {
            cells = new List<GameObject>();
        }
        public void AddGameObject(GameObject go)
        {
            cells.Add(go);
        }
        public int PrintCount() => cells.Count;

    }


    public GameObject Prefab;
    public CustomGrid<LE_CellData> gos;
    public Material Gridmat;
    public int sectorval;
    
    //public Sector[] Sectors = new Sector[9];
    List<List<GameObject>> Sectors = new List<List<GameObject>>
    {
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>(),
    };


    public void CreateGridobjects()
    {
        if (this.gameObject.transform.childCount > 0) return;
        gos = new CustomGrid<LE_CellData>(15, 15);
        //Sector[] Sectors = new Sector[9];
        //foreach(var t in Sectors)
        //{
        //    t.init();
        //}
        // Sectors = new List<List<GameObject>>();
        Sectors = new List<List<GameObject>>
    {
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>(),
    };
        int xdis = 0;
        for (int x = 0; x < 15; x++)
        {
            int zdis = 0;
            xdis = x % 5 == 0 ? (xdis + 1) : xdis;
            for (int z = 0; z < 15; z++)
            {
                var getSector = x / 5 == 0 ? z / 5 == 0 ? 6 : z / 5 == 1 ? 3 : z / 5 == 2 ? 0 : 0 :
                    x / 5 == 1 ? z / 5 == 0 ? 7 : z / 5 == 1 ? 4 : z / 5 == 2 ? 1 : 0 :
                    x / 5 == 2 ? z / 5 == 0 ? 8 : z / 5 == 1 ? 5 : z / 5 == 2 ? 2 : 0 : 0;
                zdis = z % 5 == 0 ? (zdis + 1) : zdis;
                var temp = Instantiate(Prefab, new Vector3(((x * 2) + xdis) + 1, 0, ((z * 2) + zdis) + 1), Quaternion.identity);
                temp.name = $"({x},{z})";
                temp.transform.SetParent(this.gameObject.transform);
                var cell = new LE_CellData(new Vector2(x, z), new Vector3((x * 2) + 1, 0, (z * 2) + 1), temp);
                gos.EditIndex(x, z, cell);
                Sectors[getSector].Add(temp);
            }
        }
        
    }
    public void colortheSector()
    {
        //Sort Sector
        foreach(var t in Sectors[sectorval])
        {
            t.GetComponent<MeshRenderer>().material = Gridmat;
        }
    }
    public void cleartheGrid()
    {
        try
        {
            for (int i = 0; i < gos.getWidth(); i++)
            {
                for (int j = 0; j < gos.getHeight(); j++)
                {
                    DestroyImmediate(gos.GetAtIndex(i, j).GetCellsGameObject());
                }
            }
        }
        catch
        {
            CustomLogs.CC_Log(this.gameObject.transform.childCount.ToString(), "cyan");
            if (this.gameObject.transform.childCount == 0)
            {
                CustomLogs.CC_EventLog("CUSTOM ERROR:", "red", "No Data To Destroy", "cyan");
            }
            else
            {
                var totalval = this.gameObject.transform.childCount;
                for (int i = 0; i < totalval; i++)
                {
                    DestroyImmediate(this.gameObject.transform.GetChild(0).gameObject);

                }
            }
            //Debug.Log($"<color=red>CUSTOM ERROR:</color><color=cyan> No Data To Destroy </color>");
        }
    }

}
