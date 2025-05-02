using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Rule2DUI : MonoBehaviour
{

    public Color SetColor;
    public Color UnsetColor;
    public Color DisabledColor;
    public Rule2D Rule;

    public Image[] Cells;

    void Start()
    {
        /*
        foreach(Image cell in Cells)
        {
            cell.color = DisabledColor;
        }
        */
    }

    public void SetRule(Rule2D rule)
    {
        Rule = rule;
        for (int i = 0; i < Cells.Length; i++)
        {
            if((rule.neighborhoodMask & (1 << i)) == 0)
            {
                Cells[i].color = DisabledColor;
                continue;
            }
            if(rule is MatchRule matchRule && (matchRule.matchValue & (1 << i)) != 0)
            {
                Cells[i].color = SetColor;
                continue;
            }
            Cells[i].color = UnsetColor;
        }
    }

    public void HandleClick(BaseEventData eventData)
    {
        PointerEventData pointerEventData = eventData as PointerEventData;
        int index = pointerEventData.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex();
        
        if(pointerEventData.button == PointerEventData.InputButton.Left)
        {
            if(Rule is MatchRule matchRule)
            {
                matchRule.matchValue ^= (1 << index);
                Debug.Log("neighborhoodMask: " + matchRule.neighborhoodMask);
                Debug.Log("matchValue: " + matchRule.matchValue);
            }
        }
        else if(pointerEventData.button == PointerEventData.InputButton.Right)
        {
            Rule.neighborhoodMask ^= (1 << index);
        }
        SetRule(Rule);
    }
}
