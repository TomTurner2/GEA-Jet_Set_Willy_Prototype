using UnityEngine;
using System.Collections.Generic;

public class menuSystem : MonoBehaviour
{
    public UI ui;
    public PlayerControl player;
    public CameraFollow cam;
    public Transform mainMenuTarget;
    public Transform controlMenuTarget;


    private void Start()
    {
        ui.enabled = false;
        player.enabled = false;
        cam.setTarget(mainMenuTarget);
    }

    public void startGame()
    {
        ui.enabled = true;
        player.enabled = true;
        cam.setTarget(player.transform);
    }

    public void quitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void controlMenu()
    {
        cam.setTarget(controlMenuTarget);
    }

    public void backToMenu()
    {
        cam.setTarget(mainMenuTarget);
    }

}
