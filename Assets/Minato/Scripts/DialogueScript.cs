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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // Checks if robot or creature

        if (gameObject.tag == "RobotDialogue")
        {
            
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