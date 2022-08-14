using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SingleChoiceNode : DialogueNode
{
    public override void Initialize(string nodeName, DialogueGraphView dialogueGraph, Vector2 position)
    {
        base.Initialize(nodeName, dialogueGraph, position);

        DialogueType = DialogueType.SingleChoice;

        ChoiceSaveData choiceSaveData = new ChoiceSaveData()
        {
            Text = "Next Dialogue"
        };
        
        Choices.Add(choiceSaveData);
    }

    public override void Draw()
    {
        base.Draw();
        
        //Output Field
        
        foreach (var choice in Choices)
        {
            Port choicePort = this.CreatePort(choice.Text);

            choicePort.userData = choice;

            outputContainer.Add(choicePort);
            
            RefreshExpandedState();
        }
    }
}
