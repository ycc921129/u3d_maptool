//------------------------------
// Author: yangchengchao
// Data:   2020
//------------------------------

using System;
using UnityEngine;
using UnityEngine.UI;

public class GridPoint : MonoBehaviour
{
    [Serializable]
    public struct GridIndex
    {
        public int xIndex;
        public int yIndex;
        public int type;
    }

    public GridIndex gridIndex;
    public Image typeImg;
    public InputField inputNumber; 
    public Text typeInfo;

    private void Awake()
    {
        inputNumber.onValueChanged.AddListener(num =>
        {
            if (num == string.Empty)
            {
                gridIndex.type = 0;
                return;  
            }
            gridIndex.type = int.Parse(num);  
        });
    }

    private void OnMouseDown()
    {
        SetInfo(GamePlayWord.groupIndex);  
    }    

    public void SetGridIndex(int xIndex, int yIndex, int type)
    {
        gridIndex.xIndex = xIndex;
        gridIndex.yIndex = yIndex;
        gridIndex.type = type;
    }

    public void SetInfo(int groupIndex,int num = 0)
    {
        Debug.Log("adb ; num = " + num); 
        gridIndex.type = groupIndex ;    
        typeInfo.text = gridIndex.type.ToString();  
        switch (gridIndex.type)  
        {
            case 1:
                {
                    typeImg.color = HexToColor("FFF600ff");  
                    break;
                } 
            case 2:
                {  
                    typeImg.color = HexToColor("05FF00ff");
                    break;
                }
            case 3:  
                {
                    typeImg.color = HexToColor("FF00CBff");
                    break;
                }
            case 4:
                {
                    typeImg.color = HexToColor("0098FFff");
                    break;
                }
            case 5:
                {
                    typeImg.color = HexToColor("FFA310ff");
                    break;
                }
        }
       
    }

    public Color HexToColor(string hex)
    {
        byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        float r = br / 255f;  
        float g = bg / 255f;
        float b = bb / 255f;  
        float a = cc / 255f;
        return new Color(r, g, b, a);
    }
}
