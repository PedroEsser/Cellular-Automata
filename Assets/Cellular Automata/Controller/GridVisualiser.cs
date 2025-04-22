using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.EventSystems;

public class GridVisualiser : MonoBehaviour
{
    public CellularAutomata2D ca;
    public RectTransform rectTransform;
    public Smooth2DTransform smoothTransform;
    public Material material;

    public Vector2 WindowSize => new Vector2(rectTransform.rect.width, rectTransform.rect.height);

    void Update()
    {
        material.SetVector("_WindowSize", WindowSize);
        material.SetVector("_Position", smoothTransform.Position);
        material.SetFloat("_Scale", smoothTransform.Scale);
    }

    public void Initialize(CellularAutomata2D ca){
        this.ca = ca;
        smoothTransform.Initialize(new Vector2(ca.Grid.Width / 2, ca.Grid.Height / 2), 20);
        material.SetBuffer("_GridBuffer", ca.currentBuffer);
        material.SetVector("_GridDimensions", new Vector4(ca.Grid.Width, ca.Grid.Height, ca.Grid.Width / 32, ca.Grid.Width % 32));
    }

    public Vector2 PixelToWorld(Vector2 pixel)
    {
        return pixel / WindowSize.x * smoothTransform.Scale + smoothTransform.Position;
    }

    public long GetCellAt(Vector2 pixel)
    {
        Vector2 transformedPosition = pixel / WindowSize.x * smoothTransform.Scale + smoothTransform.Position;
        return ca.Grid.GetBitIndex(Vector2Int.FloorToInt(transformedPosition));
    }

    public Vector2 GetPointInWindow(Vector2 point)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            point,
            null,
            out localPoint
        );
        return localPoint;
    }

    public void HandleDrag(BaseEventData eventData)
    {
        PointerEventData pointerEventData = (PointerEventData)eventData;
        if(pointerEventData.button != PointerEventData.InputButton.Left)
            return;
	
        Vector2 dragDelta = -pointerEventData.delta / WindowSize.x * smoothTransform.Scale;
        smoothTransform.move(dragDelta);
    }

    public void HandleClick(BaseEventData eventData)
    {
        PointerEventData pointerEventData = (PointerEventData)eventData;
        Vector2 pixel = GetPointInWindow(pointerEventData.position);
        Vector2 worldPosition = PixelToWorld(pixel);

        if(pointerEventData.button == PointerEventData.InputButton.Right){
            ca.FlipCell(Vector2Int.FloorToInt(worldPosition));
        }else if(pointerEventData.button == PointerEventData.InputButton.Middle){
            Debug.Log("Cell coordinates: " + Vector2Int.FloorToInt(worldPosition));
        }
    }

    public void HandleScroll(BaseEventData eventData)
    {
        smoothTransform.multiplyScale(Mathf.Pow(1.1f, -Input.mouseScrollDelta.y));
    }

}
