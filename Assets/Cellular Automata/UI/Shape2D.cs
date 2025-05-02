using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape2D
{
    public BitMatrix Shape;
    public Vector2Int Position;

    public Vector4 FillArea => new Vector4(Position.x, Position.y, Shape.Width, Shape.Height);

    public Shape2D(BitMatrix shape, Vector2Int position){
        Shape = shape;
        Position = position;
    }
    
    


    public static Shape2D Empty(int x, int y, int width, int height){
        BitMatrix shape = new BitMatrix(width, height);
        return new Shape2D(shape, new Vector2Int(x, y));
    }

    public static Shape2D Fill(int x, int y, int width, int height){
        BitMatrix shape = new BitMatrix(width, height);
        shape.Fill();
        return new Shape2D(shape, new Vector2Int(x, y));
    }

}
