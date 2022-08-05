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

        Button addChoicesButton = new Button()
        {
            text = "Add Choice"
        };
        
        mainContainer.Insert(1,addChoicesButton);
        
        //Output Field
        
        foreach (var choice in Choices)
        {
            Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));

            choicePort.portName = "";

            Button deleteChoiceButton = new Button()
            {
                text = "X"
            };
            TextField choiceTextField = new TextField()
            {
                value = choice
            };
            
            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);
            outputContainer.Add(choicePort);
            
            RefreshExpandedState();
        }
    }
}
