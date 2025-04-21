using UnityEngine;
using Unity.Collections;

public class BitArray
{

    static readonly int _bitsPerInt = 32;

    private NativeArray<uint> _bits;
    private long _size;

    public NativeArray<uint> Bits => _bits;

    public BitArray(NativeArray<uint> bits, long size)
    {
        _bits = bits;
        _size = size;
    }

    public BitArray(long size)
    {
        _bits = new NativeArray<uint>((int)(size / _bitsPerInt + 1), Allocator.Persistent);
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

    /*public BitArray GetSubArray(int start, int end){
        BitArray result = new BitArray(end - start + 1);
        int startIntIndex = start / _bitsPerInt;
        int startBitIndex = start % _bitsPerInt;
        int endIntIndex = (end - start) / _bitsPerInt;
        int endBitIndex = (end - start) % _bitsPerInt;

        uint acc = _bits[startIntIndex] >> startBitIndex;
        result._bits[0] = acc;
        for(int i = 1; i <= endIntIndex; i++){
            acc = _bits[startIntIndex + i] << (_bitsPerInt - startBitIndex);
            result._bits[i - 1] |= acc;
            acc = _bits[startIntIndex + i] >> startBitIndex;
            result._bits[i] = acc;
        }

        result._bits[endIntIndex] = (result._bits[endIntIndex] << (_bitsPerInt - endBitIndex - 1)) >> (_bitsPerInt - endBitIndex - 1);
        return result;
    }*/

    /*public BitArray GetSubArray(int start, int end)
    {
        int bitLength = end - start + 1;
        int resultIntCount = (bitLength + _bitsPerInt - 1) / _bitsPerInt;

        NativeArray<uint> result = new NativeArray<uint>(resultIntCount, Allocator.Temp);

        int startIntIndex = start / _bitsPerInt;
        int startBitIndex = start % _bitsPerInt;

        int endIntIndex = end / _bitsPerInt;
        int endBitIndex = end % _bitsPerInt;

        for (int i = 0; i < resultIntCount; i++)
        {
            uint currentInt = _bits[startIntIndex + i];

            // Shift the first chunk
            if (i == 0)
            {
                currentInt >>= startBitIndex; // Align the start bit
            }

            // For the last chunk, we mask the extra bits
            if (i == resultIntCount - 1)
            {
                int remainingBits = endBitIndex + 1;
                uint mask = (1u << remainingBits) - 1;
                currentInt &= mask;
            }
            
            // Store the result in the corresponding position
            result[i] = currentInt;
        }

        return new BitArray(result, bitLength);
    }

    public void SetSubArray(int start, int end, BitArray subArray){
        int bitLength = end - start + 1;
        int resultIntCount = (bitLength + _bitsPerInt - 1) / _bitsPerInt;

        NativeArray<uint> result = new NativeArray<uint>(resultIntCount, Allocator.Temp);
    }*/

    public void SetInts(NativeArray<uint> ints, int start){
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

}