using UnityEngine;
using System;
using TMPro;

public class DialogueScript : MonoBehaviour
{
    // References

    [Serializable]
    public class DialogueLines
    {
        public string[] lines;
    }

    [SerializeField] private TMP_Text dialogueText; // To assign TMP
    [SerializeField] private GameObject dialogueBox; // To assign dialogue box
    [SerializeField] private DialogueLines dialogueLines; // Dialogue lines stored here
    private int lineCount = 0; // Sets current line

    [SerializeField] GameObject lightAOff;
    [SerializeField] GameObject lightBOff;
    [SerializeField] GameObject lightCOff;
    [SerializeField] GameObject lightDOff;
    [SerializeField] GameObject bioluminescenceOff;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // Checks if robot or creature

        if (gameObject.tag == "RobotDialogue")
        {
            bioluminescenceOff.SetActive(false);
        }

        if (gameObject.tag == "CreatureDialogue")
        {
            lightAOff.SetActive(false);
            lightBOff.SetActive(false);
            lightCOff.SetActive(false);
            lightDOff.SetActive(false);
        }

        // Canvas setup

        GameObject searchCanvas = GameObject.Find("Dialogue Canvas");
        searchCanvas.SetActive(false);
        dialogueBox.SetActive(true);
        ShowDialogue();
        
    }

    void ShowDialogue()
    {
        if (lineCount < dialogueLines.lines.Length)
        {
            dialogueText.text = dialogueLines.lines[lineCount];
        }
    }

    public void OnButtonPressed()
    {
        ContinueDialogue();
    }

    void ContinueDialogue()
    {
        lineCount++;

        if (lineCount < dialogueLines.lines.Length)
        {
            ShowDialogue();
        }

        else
        {
            GameObject searchCanvas = GameObject.Find("Dialogue Canvas");
            searchCanvas.SetActive(false);
        }
    }

}