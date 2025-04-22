using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarButton : MonoBehaviour
{

    public Color highlightColor;
    public Color defaultColor;
    public Image Background;

    public void Highlight(bool highlight = true){
        Background.color = highlight ? highlightColor : defaultColor;
    }
    

}
