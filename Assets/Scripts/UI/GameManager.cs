using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public QuestUI questUI;

    // Start is called before the first frame update
    void Start()
    {
        questUI.OpenBox();

        StartCoroutine(BeginQuest());
    }

     
    // from https://forum.unity.com/threads/waiting-for-input-in-a-custom-function.474387/
    private IEnumerator waitForKeyPress(KeyCode key)
    {
        bool done = false;
        while(!done) // essentially a "while true", but with a bool to break out naturally
        {
            if(Input.GetKeyDown(key))
            {
                done = true; // breaks the loop
            }
            yield return null; // wait until next frame, then continue execution from here (loop continues)
        }
 
        // now this function returns
    }

    private IEnumerator BeginQuest()
    {
        questUI.OpenBox();
        
        yield return new WaitForSeconds(1.0f);

        questUI.WriteText("Welcome to Project Teriyaki. Your first objective is to leave the restaurant.");

        yield return waitForKeyPress(KeyCode.Return);

        questUI.WriteText("Click on the doors to open them.");

        yield return waitForKeyPress(KeyCode.Return);

        questUI.CloseBox();

        // TODO: Space to advance dialog
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
