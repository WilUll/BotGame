using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSO : ScriptableObject
{
    [field: SerializeField]public string DialogueName { get; set; }
    [field: SerializeField] [field: TextArea()]public string Text { get; set; }
    [field: SerializeField]public List<DialogueChoiceData> Choices { get; set; }
    [field: SerializeField]public DialogueType DialogueType { get; set; }
    [field: SerializeField]public bool IsStartingDialogue { get; set; }
    
    [field: SerializeField]public AudioClip AudioClipToPlay { get; set; }
    [field: SerializeField]public BotEmotions BotEmotion { get; set; }
    [field: SerializeField]public SpeakingColor Speaker { get; set; }
    public void Initialize(string dialogueName, string text, List<DialogueChoiceData> choices, DialogueType dialogueType,
        bool isStartingDialogue, AudioClip audioClip, BotEmotions botEmotion, SpeakingColor speakerEnum)
    {
        DialogueName = dialogueName;
        Text = text;
        Choices = choices;
        DialogueType = dialogueType;
        IsStartingDialogue = isStartingDialogue;
        AudioClipToPlay = audioClip;
        BotEmotion = botEmotion;
        Speaker = speakerEnum;
    }
}
