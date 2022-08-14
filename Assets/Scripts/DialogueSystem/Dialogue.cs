using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
