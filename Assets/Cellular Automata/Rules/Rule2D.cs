using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BitArray;

public abstract class Rule2D
{
    public int neighborhoodMask;

    public Rule2D(int neighborhoodMask = (int)CellGroup.All){
        this.neighborhoodMask = neighborhoodMask;
    }

    public abstract bool satifies(int neighborhood);

    public BitArray GenerateRuleSet(){
        BitArray ruleSet = new BitArray(512);
        foreach(int neighborhood in GetAllNeighborhoods()){
            ruleSet[neighborhood] = satifies(neighborhood);
        }
        return ruleSet;
    }

    public int GetMaskedNeighborhood(int neighborhood){
        return neighborhood & neighborhoodMask;
    }

    public int GetCellCount(int neighborhood){
        return BitArray.NumberOfSetBits(GetMaskedNeighborhood(neighborhood));
    }

    public static int GetMaskedNeighborhood(int neighborhood, CellGroup group){
        return neighborhood & (int)group;
    }

    public static int GetCellCount(int neighborhood, CellGroup group = CellGroup.All){
        return BitArray.NumberOfSetBits(GetMaskedNeighborhood(neighborhood, group));
    }

    public static IEnumerable<int> GetAllNeighborhoods(){
        for(int i = 0; i < 512; i++){
            yield return i;
        }
    }

    public enum CellGroup : int{
        Empty = 0b000000000, All = 0b111111111,
        BottomRight = 0b000000001, Bottom = 0b000000010,  BottomLeft = 0b000000100,
        Right = 0b000001000, Middle = 0b000010000,  Left = 0b000100000,
        TopRight = 0b001000000, Top = 0b010000000, TopLeft = 0b100000000,

        BottomRow = BottomRight | Bottom | BottomLeft,
        MiddleRow = Right | Middle | Left,
        TopRow = TopRight | Top | TopLeft,

        RightColumn = BottomRight | Right | TopRight,
        MiddleColumn = Bottom | Middle | Top,
        LeftColumn = BottomLeft | Left | TopLeft,

        Neighbors = All & ~Middle,
    }

    
}
