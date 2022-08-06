using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueNode : Node
{
    public string DialogueName { get; set; }
    public List<string> Choices { get; set; }
    public string Text { get; set; }
    public DialogueType DialogueType { get; set; }

    public virtual void Initialize(Vector2 position)
    {
        DialogueName = "DialogueName";
        Choices = new List<string>();
        Text = "Dialogue Text.";
        
        SetPosition(new Rect(position,Vector2.zero));
        
        mainContainer.AddToClassList("ds-node__main-container");
        extensionContainer.AddToClassList("ds-node__extension-container");
    }

    public virtual void Draw()
    {
        //Title Field
        TextField dialogueNameTextField = new TextField()
        {
            value = DialogueName,
        };
        
        dialogueNameTextField.AddToClassList("ds-node__textfield");
        dialogueNameTextField.AddToClassList("ds-node__filename-textfield");
        dialogueNameTextField.AddToClassList("ds-node__textfield__hidden");

        
        titleContainer.Insert(0, dialogueNameTextField);

        //InputPort Field
        Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        inputPort.portName = "Input";
        inputContainer.Add(inputPort);
        
        //DialogueText Field
        VisualElement customDataContainer = new VisualElement();
        
        customDataContainer.AddToClassList("ds-node__custom-data-container");
        Foldout textFoldout = new Foldout()
        {
            text = "Dialogue Text"
        };

        TextField textTextField = new TextField()
        {
            value = Text
        };
        
        textTextField.AddToClassList("ds-node__textfield");
        textTextField.AddToClassList("ds-node__quote-textfield");

        textFoldout.Add(textTextField);
        
        customDataContainer.Add(textFoldout);
        
        extensionContainer.Add(customDataContainer);
    }
}
