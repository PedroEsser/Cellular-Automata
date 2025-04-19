using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smooth2DTransform : MonoBehaviour
{

    private Vector2 position = Vector2.zero;
    [SerializeField]
    private Vector2 smoothPosition = Vector2.zero;

    private float scale = 1;
    [SerializeField]
    private float smoothScale = 1;

    public Vector2 Position {
        get => smoothPosition;
        set => position = value;
    }

    public float Scale {
        get => smoothScale;
        set => scale = value;
    }
    

    void Update()
    {
        smoothPosition = Vector2.Lerp(smoothPosition, position, Time.deltaTime * 10);
        smoothScale = Mathf.Lerp(smoothScale, scale, Time.deltaTime * 10);
    }

    public void multiplyScale(float scalar)
    {
        scale *= scalar;
    }

    public void move(Vector2 delta)
    {
        position += delta;
    }
}
