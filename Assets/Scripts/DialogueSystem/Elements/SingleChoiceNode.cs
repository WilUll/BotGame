using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SingleChoiceNode : DialogueNode
{
    public override void Initialize(DialogueGraphView dialogueGraph, Vector2 position)
    {
        base.Initialize(dialogueGraph, position);

        DialogueType = DialogueType.SingleChoice;
        
        Choices.Add("Next Dialogue");
    }

    public override void Draw()
    {
        base.Draw();
        
        //Output Field
        
        foreach (var choice in Choices)
        {
            Port choicePort = this.CreatePort(choice);

            outputContainer.Add(choicePort);
            
            RefreshExpandedState();
        }
    }
}
