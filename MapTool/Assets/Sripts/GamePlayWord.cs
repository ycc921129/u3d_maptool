//------------------------------
// Author: yangchengchao
// Data:   2020
//------------------------------

using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class GamePlayWord : MonoBehaviour
{
    public static GamePlayWord Instance = null;
    public CreateMap map;
    public GameObject Save; 
    public GameObject NextLevel;
    public GameObject levelLayer;
    public GameObject passScoreObj;
    public GameObject groupObj;

    public Text titleText;
    public Text levelText;
    public ToggleGroup centerGroup;
    private CanvasGroup canvasGroup;
    private Dropdown levelLayerDropdown;
    private Button NextLevelBtn;
    private ToggleGroup group;
    private InputField passSocre;

    private bool isView = false;
    public int curSocre { get; private set; }
    public static int groupIndex; 

    private void Awake()
    {
        Instance = this;

        PauseLevelData();
        InitMap();
        InitEvent();
        InitToggle();
        InitCenterGroup();
        InitDroDown();
    }

    private void Update()
    {
        if (map == null || map.levelInfoList.Count == 0) return;
          
        NextLevelBtn.interactable = map.CurrentLevel <= map.levelInfoList.Count;
    }

    private void InitMap()
    {
        map.MapWidth = 10;
        map.MapHeight = 10;
        map.CurrentLevel = 1;
        map.gridPoints = null;

        map.InitMap();
    }

    private void InitToggle()
    {
        var toggles = group.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < toggles.Length; i++)
        {
            if (i == 0) toggles[i].isOn = true;
            else toggles[i].isOn = false;

            toggles[i].onValueChanged.AddListener(Taskf);
        }

        groupIndex = 0;
    }

    private void Taskf(bool arg0)
    {
        var toggles = group.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                groupIndex = i + 1;  
                return;
            }
        }
    }

    private void InitCenterGroup()
    {
        var toggles = centerGroup.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].onValueChanged.AddListener(GetCenter);
        }
    }

    private void GetCenter(bool arg0)
    {
        map.MapWidth = 10;
        map.MapHeight = 10;
        map.CreateMaps(true);
    }

    private void InitEvent()
    {
        levelLayerDropdown = levelLayer.GetComponent<Dropdown>(); 
        canvasGroup = titleText.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        group = groupObj.GetComponent<ToggleGroup>();
        passSocre = passScoreObj.GetComponent<InputField>();  
        NextLevelBtn = NextLevel.GetComponent<Button>();
        Save.GetComponent<Button>().onClick.AddListener(() => SaveEvent());   
        NextLevelBtn.onClick.AddListener(UpdateLevelEvent);
        levelLayerDropdown.onValueChanged.AddListener(levelLayerDropdownEvent);
        passSocre.onValueChanged.AddListener(score => this.curSocre = int.Parse(score));
    }    

    #region Event
    private void SaveEvent(bool isNextLevel = false)
    {
        NextLevelBtn.interactable = true;  
        SaveLevelData(); 
        SetTitltText("保存成功");
        map.PauseLevelFileByJson();
        map.CreateMaps(isNextLevel);  
        UpdateDrodowmState(map.CurrentLevel);  

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }    

    private void levelLayerDropdownEvent(int arg0)
    {
        map.CurrentLevel = arg0 + 1;
        map.CreateMaps();
    } 

    private void UpdateLevelEvent()  
    {
        map.CurrentLevel += 1;
        map.CreateMaps();
        UpdateDrodowmState(map.CurrentLevel);
    }    

    private void InitDroDown()
    {
        for (int i = 0; i < map.levelInfoList.Count; i++)
        {
            UpdateDrodowmState(i + 1);   
        }
    }

    private void UpdateDrodowmState(int level)  
    {
        Dropdown.OptionData data = new Dropdown.OptionData();
        data.text = string.Format("第{0}关", level);

        if(levelLayerDropdown.options.Count < map.levelInfoList.Count)  
            levelLayerDropdown.options.Add(data);
    }
    #endregion

    #region Common
    public void SetActiveObj(bool isActive, params object[] objs)
    {
        foreach (var obj in objs)
        {
            (obj as GameObject).SetActive(isActive);
        }
    }
    public void SetTitltText(string msg)
    {
        titleText.text = msg;
        canvasGroup.alpha = 1;
        Invoke("CloseTitle", 0.5f); 
    }
    private void CloseTitle()
    {
        DOTween.To(() => canvasGroup.alpha, tovalue => canvasGroup.alpha = tovalue, 0, 0.5f);
    }

    public void LogFloat(string tag, object obj)
    {
        Debug.Log(tag + " : " + obj.ToString());
    }    

    public void ViewNextLevel()
    {

    }
    #endregion

    #region Json
    private void SaveLevelData()
    {
        map.SaveLevelFileByJson(map.MapWidth, map.MapHeight);         
    }

    private void PauseLevelData()
    {
        map.PauseLevelFileByJson();
    }
    #endregion
}
