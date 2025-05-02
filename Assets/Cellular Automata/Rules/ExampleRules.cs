using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Rule2D;
using static CompositeRule;

public static class ExampleRules
{

    public static Rule2D Empty(){
        return new CompositeRule(CompositeType.Or);
    }

    public static Rule2D GameOfLife(){
        CompositeRule whenAliveRule = new CompositeRule();
        whenAliveRule.AddRule(MatchRule.Alive);
        whenAliveRule.AddRule(new CountRule(2, 3, (int)CellGroup.Neighbors));

        CompositeRule whenDeadRule = new CompositeRule();
        whenDeadRule.AddRule(MatchRule.Dead);
        whenDeadRule.AddRule(new CountRule(3, (int)CellGroup.Neighbors));

        Rule2D gameOfLifeRule = new CompositeRule(CompositeType.Or, whenAliveRule, whenDeadRule);

        return gameOfLifeRule;
    }
    public static Rule2D Seeds(){
        CompositeRule seedsRule = new CompositeRule();
        seedsRule.AddRule(MatchRule.Dead);
        seedsRule.AddRule(new CountRule(2, 2, (int)CellGroup.Neighbors));

        return seedsRule;
    }

    public static Rule2D HighLife(){
        CompositeRule whenAliveRule = new CompositeRule();
        whenAliveRule.AddRule(MatchRule.Alive);
        whenAliveRule.AddRule(new CountRule(2, 3, (int)CellGroup.Neighbors));

        CompositeRule whenDeadRule = new CompositeRule();
        whenDeadRule.AddRule(MatchRule.Dead);
        CompositeRule countRule = new CompositeRule(CompositeType.Or);
        countRule.AddRule(new CountRule(3, (int)CellGroup.Neighbors));
        countRule.AddRule(new CountRule(6, (int)CellGroup.Neighbors));
        whenDeadRule.AddRule(countRule);

        Rule2D highLifeRule = new CompositeRule(CompositeType.Or, whenAliveRule, whenDeadRule);

        return highLifeRule;
    }

}
