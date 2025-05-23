using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class BitMatrix
{
    private int _width;
    public int Width => _width;
    private int _height;
    public int Height => _height;
    public BitArray BitArray;
    public uint[] Bits => BitArray.Bits;

    public BitMatrix(int width, int height)
    {
        _width = width;
        _height = height;
        BitArray = new BitArray((long)width * height);
    }

    public bool this[int x, int y]
    {
        get => BitArray[GetBitIndex(x, y)];
        set => BitArray[GetBitIndex(x, y)] = value;
    }

    public uint this[long xMin, long yMin, long xMax, long yMax]
    {
        get{
            uint result = 0;
            long dx = xMax - xMin;
            for(long i = yMin; i <= yMax; i++){
                long startBit = xMin + i * _width;
                result |= BitArray[startBit, startBit + dx] << (int)((yMax - i) * (dx + 1));
            }
            return result;
        }
    }

    public void Fill(int xMin = 0, int yMin = 0, int xMax = -1, int yMax = -1, bool value = true){
        if(xMax == -1) xMax = _width - 1;
        if(yMax == -1) yMax = _height - 1;
        for(int i = yMin; i <= yMax; i++){
            for(int j = xMin; j <= xMax; j++){
                this[j, i] = value;
            }
        }
    }

    public BitMatrix Clone(){
        BitMatrix clone = new BitMatrix(_width, _height);
        BitArray.CopyTo(clone.BitArray);
        return clone;
    }
    
    public void CopyTo(BitMatrix other) => BitArray.CopyTo(other.BitArray);

    public void SetBit(int x, int y, bool value) => BitArray[x + (long)y * _width] = value;

    public bool GetBit(int x, int y) => BitArray[x + (long)y * _width];

    public long GetBitIndex(int x, int y) => x + (long)y * _width;
    

    public long GetBitIndex(Vector2Int position) => GetBitIndex(position.x, position.y);
}
