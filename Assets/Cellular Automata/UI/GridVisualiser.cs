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

    public ComputeBuffer gridBuffer;
    public NativeArray<uint> grid;
    public Material material;

    public Vector2 WindowSize => new Vector2(rectTransform.rect.width, rectTransform.rect.height);
    
    void Start()
    {
        ca = new CellularAutomata2D(1024, 1024, new GameOfLife().rules);
        grid = ca.GetBits();
        smoothTransform.Position = new Vector2(ca.grid.width / 2, ca.grid.height / 2);
        smoothTransform.Scale = 10;

        gridBuffer = new ComputeBuffer(grid.Length, sizeof(int));
        gridBuffer.SetData(grid);
        material.SetBuffer("_GridBuffer", gridBuffer);
        material.SetVector("_GridDimensions", new Vector4(ca.grid.width, ca.grid.height, 0, 0));
    }

    void Update()
    {
        material.SetVector("_WindowSize", WindowSize);
        material.SetVector("_Position", smoothTransform.Position);
        material.SetFloat("_Scale", smoothTransform.Scale);

        smoothTransform.multiplyScale(Mathf.Pow(1.1f, -Input.mouseScrollDelta.y));
    }

    void OnDestroy()
    {
        gridBuffer?.Release();
        ca.Dispose();
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
        if(pointerEventData.button != PointerEventData.InputButton.Right)
            return;
        Vector2 pixel = GetPointInWindow(pointerEventData.position);
        int cellIndex = GetCellAt(pixel);
        ca.grid.BitArray[cellIndex] = !ca.grid.BitArray[cellIndex];
        UpdateBuffer();
    }

    public void NextGeneration()
    {
        ca.Step();
        UpdateBuffer();
    }

    public void UpdateBuffer() => gridBuffer.SetData(grid);

    public int GetCellAt(Vector2 pixel)
    {
        Vector2 transformedPosition = pixel / WindowSize.x * smoothTransform.Scale + smoothTransform.Position;
        return ca.grid.GetBitIndex(Mathf.FloorToInt(transformedPosition.x), Mathf.FloorToInt(transformedPosition.y));
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

}
