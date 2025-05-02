using UnityEngine;
using Unity.Collections;

public class BitArray
{

    static readonly int _bitsPerInt = 32;

    private uint[] _bits;
    private long _size;

    public uint[] Bits => _bits;

    public BitArray(uint[] bits, long size)
    {
        _bits = bits;
        _size = size;
    }

    public BitArray(long size)
    {
        _bits = new uint[(int)(size / _bitsPerInt + 1)];
        _size = size;
    }

    public BitArray(string bitString) : this(bitString.Length)
    {
        for (int i = 0; i < _size; i++)
            this[i] = bitString[(int)(_size - i - 1)] == '1';
    }

    public long Size => _size;

    public int GetIntIndex(long index){
        return (int)(index / _bitsPerInt);
    }

    public int GetBitIndex(long index){
        return (int)(index % _bitsPerInt);
    }

    public bool this[long index]
    {
        get => (_bits[GetIntIndex(index)] & (1u << GetBitIndex(index))) != 0;
        set
        {
            if (value)  _bits[GetIntIndex(index)] |= 1u << GetBitIndex(index);
            else        _bits[GetIntIndex(index)] &= ~(1u << GetBitIndex(index));
        }
    }

    public uint this[long start, long end]  //assumes length is less than 32
    {
        get
        {
            int intIndex = GetIntIndex(start);
            int startBitIndex = GetBitIndex(start);
            int endBitIndex = GetBitIndex(end);

            if(startBitIndex > endBitIndex){
                uint result = _bits[intIndex] >> startBitIndex;
                return result | (_bits[intIndex + 1] << (_bitsPerInt - endBitIndex - 1) >> (startBitIndex - endBitIndex - 1));
            }
            return (_bits[intIndex] << (_bitsPerInt - endBitIndex - 1)) >> (_bitsPerInt - endBitIndex - 1 + startBitIndex);
        }
    }

    public int PopCount(){
        int count = 0;
        foreach(var bit in _bits){
            count += NumberOfSetBits((int)bit);
        }
        return count;
    }

    public void SetInts(uint[] ints, int start){
        for(int i = 0; i < ints.Length; i++){
            _bits[start + i] = ints[i];
        }
    }


    public void CopyTo(BitArray other){
        for(int i = 0; i < _bits.Length; i++){
            other._bits[i] = _bits[i];
        }
    }


    public override string ToString(){
        string result = "";
        for(int i = 0; i < _size; i++){
            result += this[(int)(_size - i - 1)] ? "1" : "0";
        }
        return result;
    }

    public static int NumberOfSetBits(int i)
    {
        i = i - ((i >> 1) & 0x55555555);
        i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
        return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
    }

}