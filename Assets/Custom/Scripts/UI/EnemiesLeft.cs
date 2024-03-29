using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemiesLeft : MonoBehaviour
{
    int enemiesLeft;
    Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] enemyArray = GameObject.FindGameObjectsWithTag("enemy");
        enemiesLeft = enemyArray.Length;

        if (enemiesLeft == 0)
        {
            text.text = "you're did it !";
        }
        else
        {
            text.text = "Enemies Left: " + enemiesLeft;
        }
    }
}
