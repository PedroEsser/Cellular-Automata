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
        smoothTransform.Initialize(new Vector2(ca.Width, ca.Height) / 2, ca.Width);
        material.SetBuffer("_GridBuffer", ca.currentBuffer);
        material.SetVector("_GridDimensions", ca.GridSize);
    }

    public Vector2 PixelToWorld(Vector2 pixel)
    {
        return pixel / WindowSize.x * smoothTransform.Scale + smoothTransform.Position;
    }

    public long GetCellIndexAt(Vector2 pixel)
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

    public Vector2Int GetCellAt(Vector2 pixel){
        return Vector2Int.FloorToInt(PixelToWorld(GetPointInWindow(pixel)));
    }

    public void HandleDrag(BaseEventData eventData)
    {
        PointerEventData pointerEventData = (PointerEventData)eventData;
        if(pointerEventData.button == PointerEventData.InputButton.Right){
            Vector2Int cell = GetCellAt(pointerEventData.position);
            ca.FlipCell(cell);
            return;
        }

        if(pointerEventData.button != PointerEventData.InputButton.Left)
            return;
	
        Vector2 dragDelta = -pointerEventData.delta / WindowSize.x * smoothTransform.Scale;
        smoothTransform.move(dragDelta);
    }

    Vector2Int? fillStart = null;

    public void HandleClick(BaseEventData eventData)
    {
        PointerEventData pointerEventData = (PointerEventData)eventData;
        Vector2 pixel = GetPointInWindow(pointerEventData.position);
        Vector2 worldPosition = PixelToWorld(pixel);

        if(pointerEventData.button == PointerEventData.InputButton.Right){
            ca.FlipCell(Vector2Int.FloorToInt(worldPosition));
        }else if(pointerEventData.button == PointerEventData.InputButton.Middle){
            if(fillStart == null){
                fillStart = Vector2Int.FloorToInt(worldPosition);
            }else{
                /*Vector2Int fillDimensions = Vector2Int.FloorToInt(worldPosition) - fillStart.Value;
                ca.FillShape(Shape2D.Fill(fillStart.Value.x, fillStart.Value.y, fillDimensions.x + 1, fillDimensions.y + 1));
                fillStart = null;*/
                Debug.Log("Cell position: " + Vector2Int.FloorToInt(worldPosition));
            }
        }
    }

    public void HandleScroll(BaseEventData eventData)
    {
        smoothTransform.multiplyScale(Mathf.Pow(1.1f, -Input.mouseScrollDelta.y));
    }

}
