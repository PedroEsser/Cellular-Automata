using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Rule2D
{
    public BitArray rules;

    public Rule2D(){
        rules = new BitArray(512);
        foreach(int neighbors in GetAllNeighbors()){
            rules[neighbors] = CalculateRuleAt(neighbors);
        }
    }

    public Rule2D(BitArray rules){
        if(rules.Size != 512){
            throw new System.Exception("Rules must be 512 bits long");
        }
        this.rules = rules;
    }

    protected abstract bool CalculateRuleAt(int neighbors);

    public static IEnumerable<int> GetAllNeighbors(){
        for(int i = 0; i < 512; i++){
            yield return i;
        }
    }

}
