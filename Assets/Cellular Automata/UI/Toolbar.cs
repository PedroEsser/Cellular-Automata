using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    public Controller controller;

    public ToolbarButton gridlinesButton;
    public bool showGridlines = false;
    
    void Start()
    {
        
    }

    public void HandleToggleGridLines(){
        showGridlines = !showGridlines;
        controller.gridVisualiser.material.SetFloat("_ShowGridlines", showGridlines ? 1 : 0);
        gridlinesButton.Highlight(showGridlines);
    }

}
