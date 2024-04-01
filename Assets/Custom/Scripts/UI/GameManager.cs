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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(QuestDemo());
    }

    private IEnumerator QuestDemo()
    {
        questUI.OpenBox();
        
        yield return new WaitForSeconds(1.0f);

        questUI.WriteText("OBJECTIVE: kill all of the cylinders (evil)");

        yield return new WaitForSeconds(5.0f);

        questUI.CloseBox();

        // TODO: Space to advance dialog
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
