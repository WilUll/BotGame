using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class MulitpleChoiceNode : DialogueNode
{
    public override void Initialize(Vector2 position)
    {
        base.Initialize(position);

        DialogueType = DialogueType.MultipleChoice;
        
        Choices.Add("New Choice");
    }

    public override void Draw()
    {
        base.Draw();

        Button addChoicesButton = ElementUtility.CreateButton("Add Choice", () =>
        {
            Port choicePort = CreateChoicePort("New Choice");

            Choices.Add("New Choice");
            
            outputContainer.Add(choicePort);
        });
        
        addChoicesButton.AddToClassList("ds-node__button");
        
        mainContainer.Insert(1,addChoicesButton);
        
        //Output Field
        
        foreach (var choice in Choices)
        {
            Port choicePort = CreateChoicePort(choice);
            outputContainer.Add(choicePort);
            
        }
        
        RefreshExpandedState();
    }

    #region Element Creation
    private Port CreateChoicePort(string choice)
    {
        Port choicePort = this.CreatePort();

        Button deleteChoiceButton = ElementUtility.CreateButton("X");
            
        deleteChoiceButton.AddToClassList("ds-node__button");

        TextField choiceTextField = ElementUtility.CreateTextField(choice);
            
        choiceTextField.AddClasses(
            "ds-node__textfield",
            "ds-node__choice-textfield",
            "ds-node__textfield__hidden"
        );
        
            
        choicePort.Add(choiceTextField);
        choicePort.Add(deleteChoiceButton);
        return choicePort;
    }
    #endregion
}
