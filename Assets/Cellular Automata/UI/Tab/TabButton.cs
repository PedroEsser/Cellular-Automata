using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    public TabGroup tabGroup;

    [SerializeField]
    private Button button;

    public Color color {
        get{
            return button.image.color;
        }
        set{
            button.image.color = value;
        }
    }

    void Start(){
        button.onClick.AddListener(OnClick);
        tabGroup.Subscribe(this);
    }

    public void OnClick(){
        tabGroup.OnTabSelected(this);
    }
}
