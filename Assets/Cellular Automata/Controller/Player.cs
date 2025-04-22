using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Player : MonoBehaviour
{

    public Controller controller;

    public bool isPlaying = false;

    public Sprite PlaySprite;
    public Sprite PauseSprite;
    public Image PlayButton;
    public Text StepCounter;

    public TMP_InputField fpsInput;
    public int targetFPS = 30;
    private float timeLeft = 0;

    public void TogglePlay()
    {
        isPlaying = !isPlaying;
        PlayButton.sprite = isPlaying ? PauseSprite : PlaySprite;
    }

    public void Update()
    {
        if (isPlaying){
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0){
                Step();
                timeLeft = 1.0f / targetFPS;
            }
        }
    }

    public void Step()
    {
        controller.Step();
        StepCounter.text = controller.ca.StepCount + "";
    }

    public void SetFPS(){
        if(int.TryParse(fpsInput.text, out int fps)){
            targetFPS = fps;
            timeLeft = 1.0f / targetFPS;
        }else{
            fpsInput.text = targetFPS + "";
        }
    }

}
