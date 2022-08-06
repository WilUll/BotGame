using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public DialogueGraphView()
    {
        AddManipulators();
        AddGridBackground();

        AddStyles();
    }
    
    private void AddManipulators()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(CreateNodeContextualMenu("Add Node (Single Choice)", DialogueType.SingleChoice));
        this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", DialogueType.MultipleChoice));

        
    }

    private IManipulator CreateNodeContextualMenu(string actionTitle, DialogueType dialogueType)
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction(actionTitle, 
                actionEvent => AddElement(CreateNode(dialogueType,actionEvent.eventInfo.localMousePosition)))
            );

        return contextualMenuManipulator;
    }

    private DialogueNode CreateNode(DialogueType dialogueType,Vector2 position)
    {
        //Type nodeType = Type.GetType($"{dialogueType}Node");

        //DialogueNode node = (DialogueNode)Activator.CreateInstance(nodeType);
        DialogueNode node;
        if (dialogueType == DialogueType.SingleChoice)
        {
            node = new SingleChoiceNode();
        }
        else
        {
            node = new MulitpleChoiceNode();
        }
        


        node.Initialize(position);
        node.Draw();

        return node;
    }
    private void AddGridBackground()
    {
        GridBackground gridBackground = new GridBackground();
        
        gridBackground.StretchToParentSize();
        
        Insert(0, gridBackground);
    }
    
    private void AddStyles()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueSystem/GraphViewStyles"));
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueSystem/NodeStyles"));

    }
}
