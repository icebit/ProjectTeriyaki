using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// source: https://www.youtube.com/watch?v=WPxWlYREzJI

public class PlayerHealthManager : MonoBehaviour
{
    // get information of player from PlayerInfo script
    public PlayerInfo playerInfo;
    // variable for the fill of the slider
    public Image fillImage;
    // variable to reference this slider
    public Slider slider;
    public Text healthText;

    // Start is called before the first frame update
    /*void Awake()
    {
        // store the component of this slider in variable
        slider = GetComponent<Slider>();
        healthText = GetComponent<Text>();
    }*/

    // Update is called once per frame
    void Update()
    {
        // hides the fill of the slider when the value is 0
        // sliders arent completely empty when their values are zero, unity is so silly!!
        if (slider.value <= slider.minValue)
        {
            fillImage.enabled = false;
        }
        else
        {
            fillImage.enabled = true;
        }

        // calculate health bar fill based on player health
        float fillValue = (float) playerInfo.playerHealth / (float) playerInfo.playerMaxHealth;
        slider.value = fillValue;

        healthText.text = playerInfo.playerHealth.ToString() + " / " + playerInfo.playerMaxHealth.ToString();
    }
}
