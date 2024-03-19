using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DigitalRuby.Tween;
using UnityEngine.SceneManagement;

public class QuestUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_QuestText;
    [SerializeField] Transform m_QuestPanel;
    [SerializeField] GameObject m_ContinueText;

    public float panelTweenTime = 1f;

    public float panelOnScreenY = 92.0f;
    public float panelOffScreenY = -92.0f;

    // Amount of time between characters
    public float writingTick = 0.02f;
    // Amount of characters to add per tick
    public int writingCharacterIncrement = 1;

    // Coroutine used to write text
    private IEnumerator writingCoroutine;
    private string currentText;
    // Number of characters currently displayed
    private int substringAmount;

    void Start()
    {
        m_ContinueText.SetActive(false);
    }

    public void WriteText(string textToWrite) {
        currentText = textToWrite;
        substringAmount = 0;

        writingCoroutine = UpdateWriting(writingTick);
        StartCoroutine(writingCoroutine);
    }

    private IEnumerator UpdateWriting(float waitTime)
    {
        m_ContinueText.SetActive(false);

        while (substringAmount < currentText.Length)
        {
            substringAmount += writingCharacterIncrement;

            m_QuestText.text = currentText.Substring(0, substringAmount);

            yield return new WaitForSeconds(waitTime);
        }

        m_ContinueText.SetActive(true);
    }

    // Tween the box up onto the screen
    public void OpenBox() {


        System.Action<ITween<float>> updateBoxPosition = (t) =>
        {
            m_QuestPanel.position = new Vector3(m_QuestPanel.position.x, t.CurrentValue, m_QuestPanel.position.z);
        };

        System.Action<ITween<float>> boxMoveCompleted = (t) =>
        {
            Debug.Log("Open box completed");
        };

        // completion defaults to null if not passed in
        m_QuestPanel.gameObject.Tween("MoveBoxUp", panelOffScreenY, panelOnScreenY, panelTweenTime, TweenScaleFunctions.CubicEaseInOut, updateBoxPosition, boxMoveCompleted);

    }


    // Tween it down off the screen
    public void CloseBox() {


        System.Action<ITween<float>> updateBoxPosition = (t) =>
        {
            m_QuestPanel.position = new Vector3(m_QuestPanel.position.x, t.CurrentValue, m_QuestPanel.position.z);
        };

        System.Action<ITween<float>> boxMoveCompleted = (t) =>
        {
            Debug.Log("Close box completed");
        };

        // completion defaults to null if not passed in
        m_QuestPanel.gameObject.Tween("MoveBoxDown", panelOnScreenY, panelOffScreenY, panelTweenTime, TweenScaleFunctions.CubicEaseInOut, updateBoxPosition, boxMoveCompleted);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
