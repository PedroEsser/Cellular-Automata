using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeRule : Rule2D
{
    private List<Rule2D> rules;
    public CompositeType compositeType;

    public CompositeRule(CompositeType compositeType = CompositeType.And, int neighborhoodMask = (int)CellGroup.All) : base(neighborhoodMask){
        this.rules = new List<Rule2D>();
        this.compositeType = compositeType;
    }

    public CompositeRule(CompositeType compositeType, params Rule2D[] rules) : this(compositeType){
        foreach(Rule2D rule in rules){
            AddRule(rule);
        }
    }

    public void AddRule(Rule2D rule){
        rules.Add(rule);
    }

    public override bool satifies(int neighborhood){
        int maskedNeighborhood = GetMaskedNeighborhood(neighborhood);
        switch(compositeType){
            case CompositeType.And:
                foreach(Rule2D rule in rules){
                    if(!rule.satifies(maskedNeighborhood)){
                        return false;
                    }
                }
                return true;
            case CompositeType.Or:
                foreach(Rule2D rule in rules){
                    if(rule.satifies(maskedNeighborhood)){
                        return true;
                    }
                }
                return false;
            default:
                return false;
        }
    }

    public enum CompositeType{
        And,
        Or
    }
}
