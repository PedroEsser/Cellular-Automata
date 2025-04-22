using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

public class CellularAutomata2D
{

    private BitMatrix grid;
    private BitArray rules;

    public BitMatrix Grid => grid; 
    public BitArray Rules => rules;

    public ComputeShader computeShader;
    public ComputeBuffer currentBuffer;
    public ComputeBuffer resultBuffer;
    public ComputeBuffer rulesBuffer;
    public ComputeBuffer shapeBuffer;
    private int stepKernel;
    private int flipKernel;
    private int fillShapeKernel;
    

    public int Width => grid.Width;
    public int Height => grid.Height;

    private int step = 0;
    public int StepCount => step;

    public CellularAutomata2D(int width, int height, BitArray rules){
        computeShader = Resources.Load<ComputeShader>("CA2D_ComputeShader");
        stepKernel = computeShader.FindKernel("StepKernel");
        flipKernel = computeShader.FindKernel("FlipCellKernel");
        fillShapeKernel = computeShader.FindKernel("FillShapeKernel");

        SetRules(rules);
        SetSize(width, height);
    }

    public void Step(){
        computeShader.SetBuffer(stepKernel, "CurrentBuffer", currentBuffer);
        computeShader.SetBuffer(stepKernel, "ResultBuffer", resultBuffer);

        int threadGroupsX = Mathf.CeilToInt(grid.Bits.Length / 256.0f);
        computeShader.Dispatch(stepKernel, threadGroupsX, 1, 1);

        var temp = currentBuffer;
        currentBuffer = resultBuffer;
        resultBuffer = temp;

        step++;
    }

    public void FlipCell(Vector2Int position){
        computeShader.SetBuffer(flipKernel, "CurrentBuffer", currentBuffer);
        computeShader.SetVector("FlipCell", new Vector4(position.x, position.y, 0, 0));
        computeShader.Dispatch(flipKernel, 1, 1, 1);
    }

    public void SetRules(BitArray rules){
        if(this.rules != null){
            this.rules.Bits.Dispose();
            rulesBuffer.Release();
        }
        this.rules = rules;
        rulesBuffer = new ComputeBuffer(rules.Bits.Length, sizeof(int));
        rulesBuffer.SetData(rules.Bits);
        computeShader.SetBuffer(stepKernel, "RuleBuffer", rulesBuffer);
    }

    public void SetSize(int width, int height){
        if(grid != null){
            grid.BitArray.Bits.Dispose();
            currentBuffer.Release();
            resultBuffer.Release();
        }
        grid = new BitMatrix(width, height);

        currentBuffer = new ComputeBuffer(grid.Bits.Length, sizeof(int));
        resultBuffer = new ComputeBuffer(grid.Bits.Length, sizeof(int));
        
        currentBuffer.SetData(grid.Bits);
        computeShader.SetBuffer(stepKernel, "CurrentBuffer", currentBuffer);
        computeShader.SetBuffer(flipKernel, "CurrentBuffer", currentBuffer);
        computeShader.SetBuffer(stepKernel, "ResultBuffer", resultBuffer);
        computeShader.SetVector("GridSize", new Vector4(width, height, width / 32, width % 32));
    }

    public void LoadCurrentBuffer(){
        GetDataIntoNativeArray(currentBuffer, grid.Bits);
    }

    public void Dispose(){
        grid.BitArray.Bits.Dispose();
        rules.Bits.Dispose();
        currentBuffer.Release();
        resultBuffer.Release();
        rulesBuffer.Release();
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

}
