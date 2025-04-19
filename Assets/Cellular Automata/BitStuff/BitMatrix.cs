using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitMatrix
{
    private int _width;
    public int width => _width;
    private int _height;
    public int height => _height;
    public BitArray BitArray;

    public BitMatrix(int width, int height)
    {
        _width = width;
        _height = height;
        BitArray = new BitArray(width * height);
    }

    public bool this[int x, int y]
    {
        get => BitArray[x + y * _width];
        set => BitArray[x + y * _width] = value;
    }

    public uint this[int xMin, int yMin, int xMax, int yMax]
    {
        get{
            uint result = 0;
            int length = xMax - xMin;
            for(int i = yMin; i <= yMax; i++){
                int startBit = xMin + i * _width;
                result |= BitArray[startBit, startBit + length] << ((yMax - i) * (length + 1));
            }
            return result;
        }
    }

    public void Fill(int xMin, int yMin, int xMax, int yMax, bool value = true){
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
    
    public void CopyTo(BitMatrix other){
        BitArray.CopyTo(other.BitArray);
    }

    public void SetBit(int x, int y, bool value){   
        BitArray[x + y * _width] = value;
    }

    public bool GetBit(int x, int y){
        return BitArray[x + y * _width];
    }

    public int GetBitIndex(int x, int y){
        return x + y * _width;
    }
}
