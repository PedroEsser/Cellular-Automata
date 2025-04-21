using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class CellularAutomata2D
{
    private BitMatrix grid;
    private BitMatrix nextGrid;
    public BitArray rules;

    private int steps = 0;
    public int Steps => steps;
    public BitMatrix Grid => steps % 2 == 0 ? grid : nextGrid;
    //public BitMatrix Grid => grid;

    public CellularAutomata2D(int width, int height, BitArray rules){
        grid = new BitMatrix(width, height);
        nextGrid = new BitMatrix(width, height);
        this.rules = rules;
    }

    public void Step(){
        steps++;
        if(steps % 2 == 1){
            for(int x = 1; x < grid.width - 1; x++){
                for(int y = 1; y < grid.height - 1; y++){
                    uint neighbors = grid[x - 1, y - 1, x + 1, y + 1];
                    nextGrid[x, y] = rules[(int)neighbors];
                }
            }
        }
        else{
            for(int x = 1; x < grid.width - 1; x++){
                for(int y = 1; y < grid.height - 1; y++){
                uint neighbors = nextGrid[x - 1, y - 1, x + 1, y + 1];
                grid[x, y] = rules[(int)neighbors];
                }
            }
        }
    }

    /*public void Step(){
        for(int x = 1; x < grid.width - 1; x++){
            for(int y = 1; y < grid.height - 1; y++){
                uint neighbors = grid[x - 1, y - 1, x + 1, y + 1];
                nextGrid[x, y] = rules[(int)neighbors];
            }
        }
            
        nextGrid.CopyTo(grid);
    }*/

    public NativeArray<uint> GetBits(){
        return grid.BitArray.Bits;
    }

    public void Dispose(){
        grid.BitArray.Bits.Dispose();
        nextGrid.BitArray.Bits.Dispose();
        rules.Bits.Dispose();
    }
}
