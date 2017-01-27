using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour
{

    /*
 PROTIP
 MAKE SURE...
 scoreXAllign ~ 150
 in inspector, make sure GUIStyles include correct font and colour.
 styles with '_' are shadows of text and should be black
 make sure all fonts are alligned to center right
  */

    public int playerHealth = 0, score = 0;
    public float time = 0.5f;
    public Texture heartIcon;
    public GUIStyle scoreText;
    public GUIStyle timer;
    public PlayerControl player = null;

    [SerializeField]
    private int scoreXAlign, scoreYAlign,
        scoreXSize, scoreYSize,
        timerX, timerY,
        heartSpaceX, heartSpaceY;
    [SerializeField]
    private int ammoSeparation, heartSeparation;
    [SerializeField]
    private float heartSizeXY, ammoSizeXY;

    // Use this for initialization
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

    }


    void OnGUI()
    {
        //health draw
        for (int i = 0; i < playerHealth; i++)
        {
            GUI.DrawTexture(new Rect(((i + 1) * 50) + (heartSeparation * i) - 30 - heartSpaceX, Screen.height - heartSizeXY - heartSpaceY, heartSizeXY, heartSizeXY), heartIcon);
        }
        //score draw
        GUI.Label(new Rect((Screen.width) - scoreXAlign - scoreXSize, Screen.height - scoreYAlign + scoreYSize, scoreXSize, scoreYSize), "Items Collected: " + (score).ToString("####0"), scoreText);
        //timer draw
        // GUI.Label(new Rect((Screen.width) - timerX+2, timerY + 2, 100, 100), "TIME: " + time.ToString("####0"), timer_);
        // GUI.Label(new Rect((Screen.width) - timerX, timerY, 100, 100), "TIME: " + time.ToString("####0"), timer);
    }

}
