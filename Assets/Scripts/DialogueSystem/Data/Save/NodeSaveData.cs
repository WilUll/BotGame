using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeSaveData
{
    [field: SerializeField] public string ID { get; set; }
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public string Text { get; set; }
    [field: SerializeField] public List<ChoiceSaveData> Choices { get; set; }
    [field: SerializeField] public string GroupID { get; set; }
    [field: SerializeField] public DialogueType DialogueType { get; set; }
    [field: SerializeField] public Vector2 Position { get; set; }
    [field: SerializeField] public AudioClip AudioClipToPlay { get; set; }
    [field: SerializeField] public BotEmotions BotEmotion { get; set; }
    [field: SerializeField] public SpeakingColor Speaker { get; set; }
}
