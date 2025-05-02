using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static ExampleRules;

public class Controller : MonoBehaviour
{
    public CellularAutomata2D ca;
    public GridVisualiser gridVisualiser;

    public TMP_InputField widthInput;
    public TMP_InputField heightInput;
    public Button setGridSizeButton;
    
    void Start()
    {
        ca = new CellularAutomata2D(40, 40, ExampleRules.GameOfLife());
        gridVisualiser.Initialize(ca);
        widthInput.text = ca.Width.ToString();
        heightInput.text = ca.Height.ToString();
    }


    public void Step()
    {
        ca.Step();
        gridVisualiser.material.SetBuffer("_GridBuffer", ca.currentBuffer);
    }

    void OnDestroy()
    {
        ca.Dispose();
    }

    public void HandleTextChange(){
        if(int.TryParse(widthInput.text, out int width) && int.TryParse(heightInput.text, out int height)){
            setGridSizeButton.interactable = width != ca.Width || height != ca.Height;
        }else{
            setGridSizeButton.interactable = false;
        }
    }

    public void SetGridSize(){
        if(int.TryParse(widthInput.text, out int width) && int.TryParse(heightInput.text, out int height)){
            ca.SetSize(width, height);
            gridVisualiser.Initialize(ca);
        }else{
            widthInput.text = ca.Width.ToString();
            heightInput.text = ca.Height.ToString();
        }
    }

    public void SetBorderMode(int borderMode){
        ca.SetBorderMode((CellularAutomata2D.BorderMode)borderMode);
    }
}
