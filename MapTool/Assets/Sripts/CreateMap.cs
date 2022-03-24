//------------------------------
// Author: yangchengchao
// Data:   2020
//------------------------------

using LitJson;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CreateMap : MonoBehaviour
{
    //是否画线
    public bool isDrawLine = true;

    //地图宽高
    private int mapWidth;
    public int MapWidth { get { return mapWidth; } set { mapWidth = value; } }
    public float mapWidthHalf { get { return (mapWidth - 1) / 2.0f; } }


    private int mapHeight;
    public int MapHeight { get { return mapHeight; } set { mapHeight = value; } }
    public float mapHeightHalf { get { return (mapHeight - 1) / 2.0f; } }     

    public GameObject itemPrefab;
    public Transform itemParent;   

    private LevelInfo levelInfo;

    //全部的格子对象
    public GridPoint[,] gridPoints;

    public List<LevelInfo> levelInfoList;
    public List<GridPoint> items;

    private int currentLevel = 1;
    public int CurrentLevel {
        get { return currentLevel; }
        set {
            currentLevel = value;
            GamePlayWord.Instance.levelText.text = "Level " + currentLevel.ToString();   
        }
    } 



    private void Update()
    {


    }    

    public void InitMap()
    {
        CreateMaps();
    }

    public void CreateMaps(bool isChange = false) //isChange：是否是修改当前关卡的配置
    {
        ClearnItems();        

        gridPoints = null;
        if (MapWidth == 0 && MapHeight == 0) return;  
        
        //地图编辑
        gridPoints = new GridPoint[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {  
            for (int y = 0; y < mapHeight; y++)
            {
                InstantiateGameObject(x, y);
            }
        }
          
        if(levelInfoList != null && levelInfoList.Count >= this.CurrentLevel && !isChange)     
            UpdateMapState();
    }

    public void UpdateMapState()
    {
        if (levelInfoList == null 
            || levelInfoList.Count == 0  
            || levelInfoList.Count < this.CurrentLevel) return;      

        var gridIndes = levelInfoList[this.CurrentLevel - 1].gridIndexList;
        for (int i = 0; i < gridIndes.Count; i++)
        {
            items[i].gridIndex = gridIndes[i];
            items[i].SetInfo((int)items[i].gridIndex.type, gridIndes[i].type);        
        }
    }
      
    public void InstantiateGameObject(int x, int y)  
    {
        GameObject go = Instantiate(itemPrefab);
        go.transform.SetParent(itemParent);  
        var posX = (int)((x - mapWidthHalf) * 100);  
        var posY = (int)((y - mapHeightHalf) * 100);
        go.transform.position = new Vector3(x - mapWidthHalf, y - mapHeightHalf); 
        GridPoint gridPoint = go.GetComponent<GridPoint>();  
        gridPoint.SetGridIndex(posX, posY, 0);
        gridPoint.SetInfo(0);
        gridPoints[x, y] = gridPoint;
        items.Add(gridPoint);  
    } 
        
    public LevelInfo CreateLevelInfo(int xColumn, int yRaw)
    {
        LevelInfo level = new LevelInfo();
        level.level = this.CurrentLevel;
        level.gridIndexList = new List<GridPoint.GridIndex>();

        for (int x = 0; x < xColumn; x++)
        {
            for (int y = 0; y < yRaw; y++)
            {
                if (gridPoints == null || gridPoints[x, y] == null) continue;

                level.gridIndexList.Add(gridPoints[x, y].gridIndex);
            }
        }

        return level;
    }

    public void SaveLevelFileByJson(int xColumn, int yRaw)
    {  
        LevelInfo levelGo = CreateLevelInfo(xColumn, yRaw);
        if (levelGo == null) return;

#if UNITY_EDITOR
        //string filePath = Application.streamingAssetsPath + "/Level_" + CurrentLevel.ToString() + ".json";
        string filePath = Application.streamingAssetsPath + "/Level_" + CurrentLevel.ToString() + ".json"; 
#else
        string filePath = Application.persistentDataPath + "/Level_" + CurrentLevel.ToString() + ".json";
#endif
        string savaJsonStr = JsonMapper.ToJson(levelGo);       
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(savaJsonStr);
        sw.Close(); 
    }

    public void PauseLevelFileByJson()
    {
        if (levelInfoList == null)
            levelInfoList = new List<LevelInfo>();
        else  
            levelInfoList.Clear();

        var length = GetFileCount();
        for (int i = 0; i < length; i++)
        {
#if UNITY_EDITOR
            string filePath = Application.streamingAssetsPath + "/Level_" + (i + 1).ToString() + ".json";
#else        
            string filePath = Application.persistentDataPath + "/Level_" + (i + 1).ToString() + ".json";
#endif  
            LevelInfo li = new LevelInfo();
            if (File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath);
                string jsonstr = sr.ReadToEnd();
                sr.Close();
                li = JsonMapper.ToObject<LevelInfo>(jsonstr);
                levelInfoList.Add(li);
            }
        }
    }

    private int GetFileCount()
    {
        var length = 0;
#if UNITY_EDITOR
        var path = Application.dataPath + "/StreamingAssets";
#else
        var path = Application.persistentDataPath;
#endif    
        if (Directory.Exists(path))
        {
            DirectoryInfo info = new DirectoryInfo(path);
            FileInfo[] files = info.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta")) continue;
                length++;
            }
        }

        return length;
    }


    public void ClearnItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i].gameObject);
            items[i] = null;
        }

        items.Clear();
    }
}
