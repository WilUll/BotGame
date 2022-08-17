using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = System.Object;

public class DialogueNode : Node
{
    public string ID { get; set; }
    public string DialogueName { get; set; }
    public List<ChoiceSaveData> Choices { get; set; }
    public string Text { get; set; }
    public DialogueType DialogueType { get; set; }
    public DialogueGroup Group { get; set; }
    
    public AudioClip AudioClipToPlay { get; set; }

    public BotEmotions BotEmotion { get; set; }

    public SpeakingColor Speaker { get; set; }

    private Color defaultBackgroundColor;

    protected DialogueGraphView dialogueGraphView;
    public virtual void Initialize(string nodeName, DialogueGraphView dialogueGraph, Vector2 position)
    {
        ID = Guid.NewGuid().ToString();
        DialogueName = nodeName;
        Choices = new List<ChoiceSaveData>();
        Text = "Dialogue Text.";

        dialogueGraphView = dialogueGraph;
        defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);
        
        SetPosition(new Rect(position,Vector2.zero));
        
        mainContainer.AddToClassList("ds-node__main-container");
        extensionContainer.AddToClassList("ds-node__extension-container");
    }

    public virtual void Draw()
    {
        //Title Field
        TextField dialogueNameTextField = ElementUtility.CreateTextField(DialogueName, null, callback =>
        {
            TextField target = (TextField) callback.target;

            target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

            if (string.IsNullOrEmpty(target.value))
            {
                if (!string.IsNullOrEmpty(DialogueName))
                {
                    ++dialogueGraphView.RepeatedNamesAmount;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(DialogueName))
                {
                    --dialogueGraphView.RepeatedNamesAmount;
                }
            }
            
            if (Group == null)
            {
                dialogueGraphView.RemoveUngroupedNode(this);

                DialogueName = target.value;
                
                dialogueGraphView.AddUngroupedNode(this);
                
                return;
            }

            DialogueGroup currentGroup = Group;
            
            dialogueGraphView.RemoveGroupedNode(this, Group);
            
            DialogueName = callback.newValue;

            dialogueGraphView.AddGroupedNode(this, currentGroup);
        });
        
        
        dialogueNameTextField.AddClasses(
            "ds-node__textfield",
            "ds-node__filename-textfield",
            "ds-node__textfield__hidden"
            );

        titleContainer.Insert(0, dialogueNameTextField);

        //InputPort Field
        Port inputPort = this.CreatePort("Input", Orientation.Horizontal, Direction.Input,
            Port.Capacity.Multi);
        inputContainer.Add(inputPort);
        
        //DialogueText Field
        VisualElement customDataContainer = new VisualElement();
        
        customDataContainer.AddToClassList("ds-node__custom-data-container");
        Foldout textFoldout = ElementUtility.CreateFoldout("Dialogue Text");

        TextField textTextField = ElementUtility.CreateTextArea(Text, null, callback =>
        {
            Text = callback.newValue;
        });




        textTextField.AddClasses(
            "ds-node__textfield",
            "ds-node__quote-textfield"
            );

        textFoldout.Add(textTextField);
        
        customDataContainer.Add(textFoldout);
        
        Foldout textFoldoutOptions = ElementUtility.CreateFoldout("Dialogue Options");
        
        //AudioClip Options
        ObjectField audioClipField = ElementUtility.CreateAudioField("Audio Clip",  callback =>
        {
            AudioClipToPlay = (AudioClip) callback.newValue;
            Debug.Log(AudioClipToPlay);
        });

        textFoldoutOptions.Add(audioClipField);
        
        //Emotion Options
        EnumField emotionEnumField = ElementUtility.CreateEnumField(BotEmotions.Neutral, "Emotion", callback =>
        {
            BotEmotion = (BotEmotions)callback.newValue;
            Debug.Log(BotEmotion);

        });
        textFoldoutOptions.Add(emotionEnumField);
        
        //Color Options
        EnumField colorEnumField = ElementUtility.CreateEnumField(SpeakingColor.Bot, "Speaker", callback =>
        {
            Speaker = (SpeakingColor)callback.newValue;
            Debug.Log(Speaker);

        });
        textFoldoutOptions.Add(colorEnumField);
        
        customDataContainer.Add(textFoldoutOptions);

        extensionContainer.Add(customDataContainer);
    }
    
    #region Overrided Methods
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Disconnect Input Ports", action=>DisconnectInputPorts());
        evt.menu.AppendAction("Disconnect Output Ports", action=>DisconnectOutputPorts());

        base.BuildContextualMenu(evt);
        
    }
    #endregion
    
    #region Utility Methods

    public void DisconnectAllPorts()
    {
        DisconnectPorts(inputContainer);
        DisconnectPorts(outputContainer);
    }

    private void DisconnectInputPorts()
    {
        DisconnectPorts(inputContainer);
    }
    private void DisconnectOutputPorts()
    {
        DisconnectPorts(outputContainer);
    }
    private void DisconnectPorts(VisualElement container)
    {
        foreach (Port port in container.Children())
        {
            if (!port.connected)
            {
                continue;
            }
            
            dialogueGraphView.DeleteElements(port.connections);
        }
    }

    public bool IsStartingNode()
    {
        Port inputPort = (Port) inputContainer.Children().First();

        return !inputPort.connected;
    }

    public void SetErrorColor(Color color)
    {
        mainContainer.style.backgroundColor = color;
    }

    public void ResetStyle()
    {
        mainContainer.style.backgroundColor = defaultBackgroundColor;
    }
    #endregion
}
