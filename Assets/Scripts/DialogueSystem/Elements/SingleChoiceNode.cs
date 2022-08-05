using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SingleChoiceNode : DialogueNode
{
    public override void Initialize(Vector2 position)
    {
        base.Initialize(position);

        DialogueType = DialogueType.SingleChoice;
        
        Choices.Add("Next Dialogue");
    }

    public override void Draw()
    {
        base.Draw();
        
        //Output Field
        
        foreach (var choice in Choices)
        {
            Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));

            choicePort.portName = choice;
            
            outputContainer.Add(choicePort);
            
            RefreshExpandedState();
        }
    }
}
