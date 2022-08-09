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
}
