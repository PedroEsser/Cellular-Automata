using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public GridVisualiser gridVisualiser;

    public bool isPlaying = false;

    public Sprite PlaySprite;
    public Sprite PauseSprite;
    public Image PlayButton;

    public void TogglePlay()
    {
        isPlaying = !isPlaying;
        PlayButton.sprite = isPlaying ? PauseSprite : PlaySprite;
    }

    public void Update()
    {
        if (isPlaying)
            gridVisualiser.NextGeneration();
        
    }

    public void Step()
    {
        gridVisualiser.NextGeneration();
    }

}
