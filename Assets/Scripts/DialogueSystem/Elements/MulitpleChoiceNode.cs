using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class MulitpleChoiceNode : DialogueNode
{
    public override void Initialize(DialogueGraphView dialogueGraph, Vector2 position)
    {
        base.Initialize(dialogueGraph, position);

        DialogueType = DialogueType.MultipleChoice;

        ChoiceSaveData choiceSaveData = new ChoiceSaveData()
        {
            Text = "New Choice"
        };

        Choices.Add(choiceSaveData);
    }

    public override void Draw()
    {
        base.Draw();

        Button addChoicesButton = ElementUtility.CreateButton("Add Choice", () =>
        {
            ChoiceSaveData choiceSaveData = new ChoiceSaveData()
            {
                Text = "New Choice"
            };

            Choices.Add(choiceSaveData);
            Port choicePort = CreateChoicePort(choiceSaveData);

            
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
    private Port CreateChoicePort(object userData)
    {
        Port choicePort = this.CreatePort();

        choicePort.userData = userData;

        ChoiceSaveData choiceSaveData = (ChoiceSaveData) userData;
        
        Button deleteChoiceButton = ElementUtility.CreateButton("X", () =>
        {
            if (Choices.Count == 1)
            {
                return;
            }

            if (choicePort.connected)
            {
                dialogueGraphView.DeleteElements(choicePort.connections);
            }

            Choices.Remove(choiceSaveData);
            
            dialogueGraphView.RemoveElement(choicePort);
        });
            
        deleteChoiceButton.AddToClassList("ds-node__button");

        TextField choiceTextField = ElementUtility.CreateTextField(choiceSaveData.Text, null, callback =>
        {
            choiceSaveData.Text = callback.newValue;
        });
            
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
