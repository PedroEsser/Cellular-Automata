using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountRule : Rule2D
{
    public int minCount;
    public int maxCount;    

    public CountRule(int minCount, int maxCount, int neighborhoodMask) : base(neighborhoodMask){
        this.minCount = minCount;
        this.maxCount = maxCount;
    }

    public CountRule(int count, int neighborhoodMask = (int)CellGroup.All) : this(count, count, neighborhoodMask){}

    public override bool satifies(int neighborhood){
        return GetCellCount(neighborhood) >= minCount && GetCellCount(neighborhood) <= maxCount;
    }
}

