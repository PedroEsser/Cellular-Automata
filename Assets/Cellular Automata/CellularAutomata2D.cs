using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

public class CellularAutomata2D
{

    private BitMatrix grid;
    private BitArray ruleSet;

    public BitMatrix Grid => grid; 
    public BitArray RuleSet => ruleSet;

    public ComputeShader computeShader;
    public ComputeBuffer currentBuffer;
    public ComputeBuffer resultBuffer;
    public ComputeBuffer rulesBuffer;
    public ComputeBuffer shapeBuffer;
    private int stepKernel;
    private int borderKernel;
    private int flipKernel;
    private int fillShapeKernel;
    

    private int width;
    private int height;
    public int Width => width;
    public int Height => height;

    private int step = 0;
    public int StepCount => step;

    private BorderMode borderMode;

    public Vector4 GridSize => new Vector4(width, height, grid.Width, grid.Height);

    public CellularAutomata2D(int width, int height, Rule2D rule, BorderMode borderMode = BorderMode.Wrap){
        computeShader = Resources.Load<ComputeShader>("CA2D_ComputeShader");
        stepKernel = computeShader.FindKernel("StepKernel");
        borderKernel = computeShader.FindKernel("BorderKernel");
        flipKernel = computeShader.FindKernel("FlipCellKernel");
        fillShapeKernel = computeShader.FindKernel("FillShapeKernel");

        SetRuleSet(rule);
        SetSize(width, height);
        SetBorderMode(borderMode);
    }

    public void Step(){
        //HandleBorder();

        int threadGroupsX = Mathf.CeilToInt(grid.Bits.Length / 256.0f);
        computeShader.SetBuffer(stepKernel, "CurrentBuffer", currentBuffer);
        computeShader.SetBuffer(stepKernel, "ResultBuffer", resultBuffer);

        computeShader.Dispatch(stepKernel, threadGroupsX, 1, 1);

        var temp = currentBuffer;
        currentBuffer = resultBuffer;
        resultBuffer = temp;

        HandleBorder();

        step++;
    }

    public void HandleBorder(){
        int threadGroupsX = Mathf.CeilToInt(grid.Bits.Length / 256.0f);
        computeShader.SetBuffer(borderKernel, "ResultBuffer", currentBuffer);
        computeShader.Dispatch(borderKernel, threadGroupsX, 1, 1);
    }

    public void FlipCell(Vector2Int position){
        computeShader.SetBuffer(flipKernel, "ResultBuffer", currentBuffer);
        computeShader.SetVector("FlipCell", new Vector4(position.x, position.y, 0, 0));
        computeShader.Dispatch(flipKernel, 1, 1, 1);
    }

    public void FillShape(Shape2D shape){
        if(shapeBuffer != null){
            shapeBuffer.Release();
        }
        shapeBuffer = new ComputeBuffer(shape.Shape.Bits.Length, sizeof(int));
        shapeBuffer.SetData(shape.Shape.Bits);
        computeShader.SetBuffer(fillShapeKernel, "ResultBuffer", currentBuffer);
        computeShader.SetBuffer(fillShapeKernel, "ShapeBuffer", shapeBuffer);
        computeShader.SetVector("FillArea", shape.FillArea);

        Debug.Log("FillArea: " + shape.FillArea);

        computeShader.Dispatch(fillShapeKernel, 1, 1, 1);
    }

    public void SetRuleSet(Rule2D rule){
        if(this.ruleSet != null){
            rulesBuffer.Release();
        }
        this.ruleSet = rule.GenerateRuleSet();
        rulesBuffer = new ComputeBuffer(this.ruleSet.Bits.Length, sizeof(int));
        rulesBuffer.SetData(this.ruleSet.Bits);
        computeShader.SetBuffer(stepKernel, "RuleBuffer", rulesBuffer);
    }

    public void SetSize(int width, int height){
        if(grid != null){
            currentBuffer.Release();
            resultBuffer.Release();
        }
        this.width = width;
        this.height = height;
        int gridWidth = width + 2;
        int gridHeight = height + 2;
        if(gridWidth % 32 != 0){
            gridWidth += 32 - gridWidth % 32;
        }
        grid = new BitMatrix(gridWidth, gridHeight);

        currentBuffer = new ComputeBuffer(grid.Bits.Length, sizeof(int));
        resultBuffer = new ComputeBuffer(grid.Bits.Length, sizeof(int));
        
        currentBuffer.SetData(grid.Bits);
        computeShader.SetVector("GridSize", GridSize);
    }

    public void SetBorderMode(BorderMode borderMode){
        this.borderMode = borderMode;
        computeShader.SetInt("BorderMode", (int)borderMode);
        HandleBorder();
    }

    public void LoadCurrentBuffer(){
        currentBuffer.GetData(grid.Bits);
    }

    public void Dispose(){
        currentBuffer.Release();
        resultBuffer.Release();
        rulesBuffer.Release();
        if(shapeBuffer != null){
            shapeBuffer.Release();
        }
    }

    public static void GetDataIntoNativeArray(ComputeBuffer buffer, NativeArray<uint> nativeArray)
    {
        uint[] temp = new uint[nativeArray.Length];
        buffer.GetData(temp);

        GCHandle handle = GCHandle.Alloc(temp, GCHandleType.Pinned);
        try
        {
            unsafe
            {
                void* srcPtr = (void*)handle.AddrOfPinnedObject();
                void* dstPtr = NativeArrayUnsafeUtility.GetUnsafePtr(nativeArray);
                UnsafeUtility.MemCpy(dstPtr, srcPtr, nativeArray.Length * sizeof(uint));
            }
        }
        finally
        {
            handle.Free();
        }
    }

    public enum BorderMode : int{
        Dead = 0,
        Alive = 1,
        Wrap = 2,
        Mirror = 3
    }

}
