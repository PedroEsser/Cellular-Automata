using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smooth2DTransform : MonoBehaviour
{
    [SerializeField]
    private Vector2 position = Vector2.zero;
    private Vector2 smoothPosition = Vector2.zero;

    [SerializeField]
    private float scale = 1;
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

        if(Mathf.Abs(smoothScale - scale) < 0.0001f)
            smoothScale = scale;
        if((position - smoothPosition).magnitude < 0.01f)
            smoothPosition = position;
    }

    public void Initialize(Vector2 position = default, float scale = 1)
    {
        this.position = position;
        this.smoothPosition = position;
        this.scale = scale;
        this.smoothScale = scale;
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
