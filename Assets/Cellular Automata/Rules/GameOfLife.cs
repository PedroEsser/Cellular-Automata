using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : Rule2D
{
    public GameOfLife() : base()
    {
        
    }

    protected override bool CalculateRuleAt(int neighbors)
    {
        int count = 0;
        for (int i = 0; i < 9; i++)
        {
            if (i == 4) continue; // skip center
            if ((neighbors & (1 << i)) != 0)
                count++;
        }

        bool isAlive = (neighbors & (1 << 4)) != 0;

        if (isAlive)
            return count == 2 || count == 3;
        else
            return count == 3;
    }
}
