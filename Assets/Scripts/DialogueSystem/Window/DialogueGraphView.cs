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
    
    private SerializableDictionary<string, NodeErrorData> ungroupedNodes;
    private SerializableDictionary<string, GroupErrorData> groups;
    private SerializableDictionary<Group, SerializableDictionary<string, NodeErrorData>> groupedNodes;
    public DialogueGraphView(DialogueGraphWindow dialogueGraphWindow)
    {
        editorWindow = dialogueGraphWindow;

        ungroupedNodes = new SerializableDictionary<string, NodeErrorData>();
        groups = new SerializableDictionary<string, GroupErrorData>();
        groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, NodeErrorData>>();
        
        AddManipulators();
        AddSearchWindow();
        AddGridBackground();

        OnElementsDeleted();
        OnGroupElementsAdded();
        OnGroupElementsRemoved();
        OnGroupRenamed();
            
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

    public DialogueGroup CreateGroup(string title, Vector2 localMousePosition)
    {
        DialogueGroup group = new DialogueGroup(title, localMousePosition);

        AddGroup(group);

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


        node.Initialize(this, position);
        node.Draw();

        AddUngroupedNode(node);

        return node;
    }
    #endregion

    #region Callbacks

        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, AskUser) =>
            {
                Type groupType = typeof(DialogueGroup);

                List<DialogueGroup> groupToDelete = new List<DialogueGroup>();
                List<DialogueNode> nodeToDelete = new List<DialogueNode>();
                
                foreach (GraphElement element in selection)
                {
                    if (element is DialogueNode node)
                    {
                        nodeToDelete.Add(node);
                        
                        continue;
                    }

                    if (element.GetType() != groupType)
                    {
                        continue;
                    }

                    DialogueGroup group = (DialogueGroup) element;

                    RemoveGroup(group);
                    
                    groupToDelete.Add(group);
                }

                foreach (DialogueGroup group in groupToDelete)
                {
                    RemoveElement(group);
                }
                
                foreach (DialogueNode node in nodeToDelete)
                {
                    if (node.Group != null)
                    {
                        node.Group.RemoveElement(node);
                    }
                    
                    RemoveUngroupedNode(node);
                    
                    RemoveElement(node);
                }
            };

        }
    
        private void OnGroupElementsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DialogueNode))
                    {
                        continue;
                    }

                    DialogueGroup nodeGroup = (DialogueGroup) group;
                    DialogueNode node = (DialogueNode) element;

                    RemoveUngroupedNode(node);
                    AddGroupedNode(node, nodeGroup);
                }
            };
        }

        private void OnGroupElementsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DialogueNode))
                    {
                        continue;
                    }

                    DialogueNode node = (DialogueNode) element;
                    
                    RemoveGroupedNode(node, group);
                    AddUngroupedNode(node);
                }
            };
        }

        private void OnGroupRenamed()
        {
            groupTitleChanged = (group, newTitle) =>
            {
                DialogueGroup dialogueGroup = (DialogueGroup) group;
                
                RemoveGroup(dialogueGroup);

                dialogueGroup.oldTitle = newTitle;
                
                AddGroup(dialogueGroup);
            };
        }

        #endregion
    
    #region Repeated Elements
    public void AddUngroupedNode(DialogueNode node)
    {
        string nodeName = node.DialogueName;

        if (!ungroupedNodes.ContainsKey(nodeName))
        {
            NodeErrorData nodeErrorData = new NodeErrorData();
            
            nodeErrorData.Nodes.Add(node);
            
            ungroupedNodes.Add(nodeName, nodeErrorData);
            
            return;
        }

        List<DialogueNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

        ungroupedNodesList.Add(node);

        Color errorColor = ungroupedNodes[nodeName].ErrorData.Color;
        
        node.SetErrorColor(errorColor);

        if (ungroupedNodesList.Count == 2)
        {
            ungroupedNodesList[0].SetErrorColor(errorColor);
        }
    }

    public void RemoveUngroupedNode(DialogueNode node)
    {
        string nodeName = node.DialogueName;

        var ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

        ungroupedNodesList.Remove(node);
        
        node.ResetStyle();

        if (ungroupedNodesList.Count == 1)
        {
            ungroupedNodesList[0].ResetStyle();
            
            return;
        }
        
        if (ungroupedNodesList.Count == 0)
        {
            ungroupedNodes.Remove(nodeName);
        }
    }
    
    private void AddGroup(DialogueGroup group)
    {
        string groupName = group.title;

        if (!(groups.ContainsKey(groupName)))
        {
            GroupErrorData groupErrorData = new GroupErrorData();
            
            groupErrorData.Groups.Add(group);
            
            groups.Add(groupName, groupErrorData);
            
            return;
        }

        List<DialogueGroup> groupsList = groups[groupName].Groups;

        groupsList.Add(group);

        Color errorColor = groups[groupName].ErrorDataScript.Color;
        
        group.SetErrorStyle(errorColor);

        if (groupsList.Count == 2)
        {
            groupsList[0].SetErrorStyle(errorColor);
        }
    }
    
    private void RemoveGroup(DialogueGroup group)
    {
        string oldGroupName = group.oldTitle;

        List<DialogueGroup> groupsList = groups[oldGroupName].Groups;

        groupsList.Remove(group);
            
        group.ResetStyle();

        if (groupsList.Count == 1)
        {
            groupsList[0].ResetStyle();
                
            return;
        }

        if (groupsList.Count == 0)
        {
            groups.Remove(oldGroupName);
        }
    }

    public void AddGroupedNode(DialogueNode node, DialogueGroup group)
    {
        string nodeName = node.DialogueName;

        node.Group = group;
        
        if (!groupedNodes.ContainsKey(group))
        {
            groupedNodes.Add(group, new SerializableDictionary<string, NodeErrorData>());
        }

        if (!groupedNodes[group].ContainsKey(nodeName))
        {
            NodeErrorData nodeErrorData = new NodeErrorData();
            
            nodeErrorData.Nodes.Add(node);
            
            groupedNodes[group].Add(nodeName, nodeErrorData);
            
            return;
        }

        groupedNodes[group][nodeName].Nodes.Add(node);

        Color errorColor = groupedNodes[group][nodeName].ErrorData.Color;
        
        node.SetErrorColor(errorColor);

        if (groupedNodes[group][nodeName].Nodes.Count == 2)
        {
            groupedNodes[group][nodeName].Nodes[0].SetErrorColor(errorColor);
        }
    }
    
    public void RemoveGroupedNode(DialogueNode node, Group group)
    {
        string nodeName = node.DialogueName;

        node.Group = null;
        
        var groupedNodesList = groupedNodes[group][nodeName].Nodes;

        groupedNodesList.Remove(node);
        
        node.ResetStyle();

        if (groupedNodesList.Count == 1)
        {
            groupedNodesList[0].ResetStyle();
            
            return;
        }
        
        if (groupedNodesList.Count == 0)
        {
            groupedNodes[group].Remove(nodeName);

            if (groupedNodes[group].Count == 0)
            {
                groupedNodes.Remove(group);
            }
        }
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