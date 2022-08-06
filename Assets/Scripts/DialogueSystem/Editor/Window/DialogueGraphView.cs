using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    private DialogueGraphWindow editorWindow;
    private SearchWindow searchWindow;
    public DialogueGraphView(DialogueGraphWindow dialogueGraphWindow)
    {
        editorWindow = dialogueGraphWindow;
        AddManipulators();
        AddSearchWindow();
        AddGridBackground();

        AddStyles();
    }



    #region Overrided Methods

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();

        ports.ForEach(port =>
        {
            if (startPort == port)
            {
                return;
            }

            if (startPort.node == port.node)
            {
                return;
            }

            if (startPort.direction == port.direction)
            {
                return;
            }

            compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    #endregion

    #region Manipulators

    private void AddManipulators()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);


        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        this.AddManipulator(CreateNodeContextualMenu("Add Node (Single Choice)", DialogueType.SingleChoice));
        this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", DialogueType.MultipleChoice));

        this.AddManipulator(CreateGroupContextualMenu());
    }

    private IManipulator CreateGroupContextualMenu()
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction("Add Group",
                actionEvent => AddElement(CreateGroup("DialogueGroup", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
        );

        return contextualMenuManipulator;
    }


    private IManipulator CreateNodeContextualMenu(string actionTitle, DialogueType dialogueType)
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction(actionTitle,
                actionEvent => AddElement(CreateNode(dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
        );

        return contextualMenuManipulator;
    }

    #endregion

    #region Element Creation

    public Group CreateGroup(string title, Vector2 position)
    {
        Group group = new Group()
        {
            title = title
        };

        group.SetPosition(new Rect(position, Vector2.zero));

        return group;
    }

    public DialogueNode CreateNode(DialogueType dialogueType, Vector2 position)
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

    #endregion

    #region Element Addition
    private void AddSearchWindow()
    {
        if (searchWindow == null)
        {
            searchWindow = ScriptableObject.CreateInstance<SearchWindow>();
            
            searchWindow.Initialize(this);
        }

        nodeCreationRequest = context =>
            UnityEditor.Experimental.GraphView.SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
    }
    private void AddGridBackground()
    {
        GridBackground gridBackground = new GridBackground();

        gridBackground.StretchToParentSize();

        Insert(0, gridBackground);
    }

    private void AddStyles()
    {
        this.AddStyleSheets(
            "DialogueSystem/GraphViewStyles",
            "DialogueSystem/NodeStyles"
        );
    }

    #endregion

    #region Utilites

    public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
    {
        Vector2 worldMousePosition = mousePosition;

        if (!searchWindow)
        {
            worldMousePosition -= editorWindow.position.position;
        }

        Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

        return localMousePosition;
    }

    #endregion
}