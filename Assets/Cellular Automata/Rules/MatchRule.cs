using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchRule : Rule2D
{

    public int matchValue;

    public MatchRule(int matchValue, int neighborhoodMask = -1) : base(neighborhoodMask == -1 ? matchValue : neighborhoodMask){
        this.matchValue = matchValue;
    }

    public override bool satifies(int neighborhood){
        return GetMaskedNeighborhood(neighborhood) == matchValue;
    }

    public static MatchRule Alive => new MatchRule((int)CellGroup.Middle);
    public static MatchRule Dead => new MatchRule(0, (int)CellGroup.Middle);
}
