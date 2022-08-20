using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Dialogue : MonoBehaviour
{
    /* DIALOGUE SCRIPTABLE OBJECTS */
    [SerializeField] private DialogueContainerSO dialogueContainer;
    [SerializeField] private DialogueGroupSO dialogueGroup;
    [SerializeField] private DialogueSO dialogue;
    
    /* FILTERS */
    [SerializeField] private bool groupedDialogues;
    [SerializeField] private bool startingDialoguesOnly;
    
    /* INDEXES */
    [SerializeField] private int selectedDialogueGroupIndex;
    [SerializeField] private int selectedDialogueIndex;
    
    /* UI */
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private GameObject skipButtons;
    
    /* Private UI Variables */
    private DialogueSO currentDialogue;
    private bool isTyping;
    private List<DialogueSO> multipleStartingNodes;
    private AudioSource audioSource;
    private Color playerColor = new Color(0.5f,1,0,1);
    private Color botColor = new Color(0f, 0.5f, 1, 1);
    
    
    /// <summary>
    /// TODO
    /// * 
    /// * 
    /// </summary>
    private void Start()
    {
        multipleStartingNodes = new List<DialogueSO>();
        audioSource = GetComponent<AudioSource>();
        StartDialogue();
    }

    public void StartDialogue()
    {
        currentDialogue = HasMultipleStartingNodes() ? multipleStartingNodes[Random.Range(0, multipleStartingNodes.Count)] : dialogue;

        DrawDialogue();
    }

    private void DrawDialogue()
    {
        SetupSettings();
        
        
        DisplayNextDialogueButtons(false);
        buttonPanel.SetActive(currentDialogue.DialogueType == DialogueType.MultipleChoice);
        if (buttonPanel.activeSelf)
        {
            Button[] buttons = GetComponentsInChildren<Button>();

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = dialogue.Choices[i].Text;
            }
        }

        StartCoroutine(TypeText());
    }

    private void SetupSettings()
    {
        if (currentDialogue.AudioClipToPlay != null)
        {
            audioSource.PlayOneShot(currentDialogue.AudioClipToPlay);
        }

        Debug.Log(currentDialogue.Speaker);
        if (currentDialogue.Speaker == SpeakingColor.Bot)
        {
            textUI.color = botColor;
        }
        else if (dialogue.Speaker == SpeakingColor.Player)
        {
            
            textUI.color = playerColor;
        }
    }

    public void OnChoiceButtonClicked(int index)
    {
        currentDialogue = currentDialogue.Choices[index].NextDialogue;
        DrawDialogue();
    }

    IEnumerator TypeText()
    {
        isTyping = true;
        textUI.text = "";

        foreach (char c in currentDialogue.Text)
        {
            textUI.text += c;

            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;
        if (!buttonPanel.activeSelf)
        {
            DisplayNextDialogueButtons(true);
        }
    }

    private void DisplayNextDialogueButtons(bool display)
    {
        skipButtons.SetActive(display);
    }

    private bool HasMultipleStartingNodes()
    {
        multipleStartingNodes.Clear();
        int startingDialogueInt = 0;
        if (dialogueContainer.DialogueGroup.TryGetValue(dialogueGroup, out var dialoguesInGroup))
        {
            foreach (DialogueSO dialogueSo in dialoguesInGroup)
            {
                if (dialogueSo.IsStartingDialogue)
                {
                    multipleStartingNodes.Add(dialogueSo);
                    startingDialogueInt++;
                }
            }
        }
        return startingDialogueInt >= 2;
    }
}
