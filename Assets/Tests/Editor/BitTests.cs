using NUnit.Framework;
using System;
using UnityEngine;

public class BitTests
{
    [Test]
    public void BitArrayTests()
    {
        string bits = "10101010101010101111111111011111111101111111011111101111101111011101101";
        BitArray bitArray = new BitArray(bits);
        for(int i = 0; i < bits.Length; i++){
            Assert.AreEqual(bits[bits.Length - i - 1] == '1', bitArray[i]);
        }

        for(int i = 0; i < bits.Length; i++){
            for(int j = 0; j < 32 && (j + i) < bits.Length; j++){
                int result = Convert.ToInt32(bits.Substring(bits.Length - (i + j) - 1, j + 1), 2);
                Assert.AreEqual(Convert.ToString(result, 2), Convert.ToString(bitArray[i, i + j], 2));
            }
        }

        /*string substring = bits.Substring(bits.Length - 11, 11);
        Assert.AreEqual(substring, bitArray.GetSubArray(0, 10).ToString());

        for(int i = 0; i < bits.Length; i++){
            for(int j = i; j < bits.Length; j++){
                substring = bits.Substring(bits.Length - j - 1, j - i + 1);
                Debug.Log("i: " + i + " j: " + j);
                Assert.AreEqual(substring, bitArray.GetSubArray(i, j).ToString());
            }
        }*/
    }

    [Test]
    public void BitMatrixTests()
    {
        BitMatrix bitMatrix = new BitMatrix(10, 10);
        for(int i = 0; i < 10; i++){
            bitMatrix[i, 2] = true;
            bitMatrix[i, 6] = true;
        }
        Assert.AreEqual(Convert.ToString(0b1111111111, 2), Convert.ToString(bitMatrix[0, 2, 9, 2], 2));
        Assert.AreEqual(Convert.ToString(0b1111111111, 2), Convert.ToString(bitMatrix[0, 6, 9, 6], 2));
        Assert.AreEqual(Convert.ToString(0b000111000, 2), Convert.ToString(bitMatrix[0, 1, 2, 3], 2));
        Assert.AreEqual(Convert.ToString(0b000111000, 2), Convert.ToString(bitMatrix[3, 1, 5, 3], 2));
        Assert.AreEqual(Convert.ToString(0b000000111, 2), Convert.ToString(bitMatrix[3, 0, 5, 2], 2));
        Assert.AreEqual(Convert.ToString(0b000000111, 2), Convert.ToString(bitMatrix[3, 0, 5, 2], 2));
        Assert.AreEqual(Convert.ToString(0b111000000, 2), Convert.ToString(bitMatrix[5, 2, 7, 4], 2));
        Assert.AreEqual(Convert.ToString(0b111000000, 2), Convert.ToString(bitMatrix[7, 6, 9, 8], 2));
    }
}
