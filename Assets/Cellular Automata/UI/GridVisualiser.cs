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
        int maxX = SystemInfo.maxComputeWorkGroupSizeX;
        int maxY = SystemInfo.maxComputeWorkGroupSizeY;
        int maxZ = SystemInfo.maxComputeWorkGroupSizeZ;
        int maxTotal = SystemInfo.maxComputeWorkGroupSize; // Total threads (usually 1024)

        Debug.Log("maxX: " + maxX);
        Debug.Log("maxY: " + maxY);
        Debug.Log("maxZ: " + maxZ);
        Debug.Log("maxTotal: " + maxTotal);

        ca = new CellularAutomata2D(1000, 1000, new GameOfLife().rules);
        //ca.grid.Fill(256, 256, 1024 - 256, 1024 - 256);
        grid = ca.GetBits();
        smoothTransform.Position = new Vector2(ca.Grid.width / 2, ca.Grid.height / 2);
        smoothTransform.Scale = 10;

        gridBuffer = new ComputeBuffer(grid.Length, sizeof(int));
        gridBuffer.SetData(grid);
        material.SetBuffer("_GridBuffer", gridBuffer);
        material.SetVector("_GridDimensions", new Vector4(ca.Grid.width, ca.Grid.height, ca.Grid.width / 32, ca.Grid.width % 32));
    }

    void Update()
    {
        material.SetVector("_WindowSize", WindowSize);
        material.SetVector("_Position", smoothTransform.Position);
        material.SetFloat("_Scale", smoothTransform.Scale);
    }

    void OnDestroy()
    {
        gridBuffer?.Release();
        ca.Dispose();
    }

    public void NextGeneration()
    {
        ca.Step();
        UpdateBuffer();
    }

    public Vector2 PixelToWorld(Vector2 pixel)
    {
        return pixel / WindowSize.x * smoothTransform.Scale + smoothTransform.Position;
    }

    public void UpdateBuffer() => gridBuffer.SetData(ca.Grid.Bits);

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
        long cellIndex = GetCellAt(pixel);

        if(pointerEventData.button == PointerEventData.InputButton.Right){
            ca.Grid.BitArray[cellIndex] = !ca.Grid.BitArray[cellIndex];
            UpdateBuffer();
        }else if(pointerEventData.button == PointerEventData.InputButton.Middle){
            Debug.Log("Cell coordinates: " + Vector2Int.FloorToInt(PixelToWorld(pixel)));
            Debug.Log("CellIndex: " + cellIndex);
            Debug.Log("Int index: " + cellIndex / 32);
            Debug.Log("BitArray Int index: " + ca.Grid.BitArray.GetIntIndex(cellIndex));
            Debug.Log("Bit index: " + cellIndex % 32);
            Debug.Log("BitArray Bit index: " + ca.Grid.BitArray.GetBitIndex(cellIndex));
            Debug.Log("Bit: " + ca.Grid.BitArray[cellIndex]);
        }
    }

    public void HandleScroll(BaseEventData eventData)
    {
        smoothTransform.multiplyScale(Mathf.Pow(1.1f, -Input.mouseScrollDelta.y));
    }

}
