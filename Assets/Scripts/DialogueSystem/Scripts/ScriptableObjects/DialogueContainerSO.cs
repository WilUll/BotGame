using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueContainerSO : ScriptableObject
{
    [field: SerializeField] public string FileName { get; set; }
    [field: SerializeField] public SerializableDictionary<DialogueGroupSO, List<DialogueSO>> DialogueGroup { get; set; }
    [field: SerializeField] public List<DialogueSO> UngroupedDialogues { get; set; }

    public void Initialize(string fileName)
    {
        FileName = fileName;

        DialogueGroup = new SerializableDictionary<DialogueGroupSO, List<DialogueSO>>();
        UngroupedDialogues = new List<DialogueSO>();
    }

    public List<string> GetDialogueGroupNames()
    {
        List<string> dialogueGroupNames = new List<string>();

        foreach (DialogueGroupSO dialogueGroup in DialogueGroup.Keys)
        {
            dialogueGroupNames.Add(dialogueGroup.GroupName);
        }

        return dialogueGroupNames;
    }

    public List<string> GetGroupedDialogueNames(DialogueGroupSO dialogueGroup, bool startingDialoguesOnly)
    {
        List<DialogueSO> groupedDialogues = DialogueGroup[dialogueGroup];

        List<string> groupedDialogueNames = new List<string>();

        foreach (DialogueSO groupedDialogue in groupedDialogues)
        {
            if (startingDialoguesOnly && !groupedDialogue.IsStartingDialogue)
            {
                continue;
            }
            groupedDialogueNames.Add(groupedDialogue.DialogueName);
        }

        return groupedDialogueNames;
    }

    public List<string> GetUngroupedDialogueNames(bool startingDialoguesOnly)
    {
        List<string> ungroupedDialougeNames = new List<string>();

        foreach (DialogueSO ungroupedDialogue in UngroupedDialogues)
        {
            if (startingDialoguesOnly && !ungroupedDialogue.IsStartingDialogue)
            {
                continue;
            }
            ungroupedDialougeNames.Add(ungroupedDialogue.DialogueName);
        }

        return ungroupedDialougeNames;
    }
}
