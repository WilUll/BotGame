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

    private Color defaultBackgroundColor;

    public virtual void Initialize(Vector2 position)
    {
        DialogueName = "DialogueName";
        Choices = new List<string>();
        Text = "Dialogue Text.";

        defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);
        
        SetPosition(new Rect(position,Vector2.zero));
        
        mainContainer.AddToClassList("ds-node__main-container");
        extensionContainer.AddToClassList("ds-node__extension-container");
    }

    public virtual void Draw()
    {
        //Title Field
        TextField dialogueNameTextField = ElementUtility.CreateTextField(DialogueName);
        
        
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

        TextField textTextField = ElementUtility.CreateTextArea(Text);


        textTextField.AddClasses(
            "ds-node__textfield",
            "ds-node__quote-textfield"
            );

        textFoldout.Add(textTextField);
        
        customDataContainer.Add(textFoldout);
        
        extensionContainer.Add(customDataContainer);
    }

    public void SetErrorColor(Color color)
    {
        mainContainer.style.backgroundColor = color;
    }

    public void ResetStyle()
    {
        mainContainer.style.backgroundColor = defaultBackgroundColor;
    }
}
