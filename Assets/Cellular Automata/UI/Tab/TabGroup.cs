using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;

    public Color selectedColor;
    public Color unselectedColor;


    public void Subscribe(TabButton button){
        if(tabButtons == null){
            tabButtons = new List<TabButton>();
        }
        tabButtons.Add(button);
    }

    public void OnTabSelected(TabButton button){
        foreach(TabButton tabButton in tabButtons){
            tabButton.color = unselectedColor;
        }
        button.color = selectedColor;
    }
    

}
