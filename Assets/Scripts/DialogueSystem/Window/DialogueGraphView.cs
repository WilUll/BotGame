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

    private MiniMap miniMap;
    
    private SerializableDictionary<string, NodeErrorData> ungroupedNodes;
    private SerializableDictionary<string, GroupErrorData> groups;
    private SerializableDictionary<Group, SerializableDictionary<string, NodeErrorData>> groupedNodes;

    private int repeatedNamesAmount;

    public int RepeatedNamesAmount
    {
        get
        {
            return repeatedNamesAmount;
        }

        set
        {
            repeatedNamesAmount = value;

            if (repeatedNamesAmount == 0)
            {
                editorWindow.EnableSaving();
            }

            if (repeatedNamesAmount == 1)
            {
                editorWindow.DisableSaving();
            }
        }
    }

    public DialogueGraphView(DialogueGraphWindow dialogueGraphWindow)
    {
        editorWindow = dialogueGraphWindow;

        ungroupedNodes = new SerializableDictionary<string, NodeErrorData>();
        groups = new SerializableDictionary<string, GroupErrorData>();
        groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, NodeErrorData>>();
        
        AddManipulators();
        AddSearchWindow();
        AddMiniMap();
        AddGridBackground();

        OnElementsDeleted();
        OnGroupElementsAdded();
        OnGroupElementsRemoved();
        OnGroupRenamed();
        OnGraphViewChanged();
            
        AddStyles();
        AddMiniMapStyles();
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
                actionEvent => CreateGroup("DialogueGroup", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
        );

        return contextualMenuManipulator;
    }


    private IManipulator CreateNodeContextualMenu(string actionTitle, DialogueType dialogueType)
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction(actionTitle,
                actionEvent => AddElement(CreateNode("DialogueName", dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
        );

        return contextualMenuManipulator;
    }

    #endregion

    #region Element Creation

    public DialogueGroup CreateGroup(string title, Vector2 localMousePosition)
    {
        DialogueGroup group = new DialogueGroup(title, localMousePosition);

        AddGroup(group);
        
        AddElement(group);

        foreach (GraphElement selectedElements in selection)
        {
            if (!(selectedElements is DialogueNode))
            {
                continue;
            }

            DialogueNode node = (DialogueNode) selectedElements;
            
            group.AddElement(node);
        }

        return group;
    }



    public DialogueNode CreateNode(string nodeName, DialogueType dialogueType, Vector2 position, bool shouldDraw = true)
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


        node.Initialize(nodeName, this, position);

        if (shouldDraw)
        {
            node.Draw();
        }

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
                Type edgeType = typeof(Edge);

                List<DialogueGroup> groupToDelete = new List<DialogueGroup>();
                List<Edge> edgesToDelete = new List<Edge>();
                List<DialogueNode> nodeToDelete = new List<DialogueNode>();
                
                foreach (GraphElement element in selection)
                {
                    if (element is DialogueNode node)
                    {
                        nodeToDelete.Add(node);
                        
                        continue;
                    }

                    if (element.GetType() == edgeType)
                    {
                        Edge edge = (Edge) element;
                        
                        edgesToDelete.Add(edge);
                        
                        continue;
                    }

                    if (element.GetType() != groupType)
                    {
                        continue;
                    }

                    DialogueGroup group = (DialogueGroup) element;

                    
                    groupToDelete.Add(group);
                }

                foreach (DialogueGroup group in groupToDelete)
                {
                    List<DialogueNode> groupNodes = new List<DialogueNode>();

                    foreach (GraphElement groupElements in group.containedElements)
                    {
                        if (!(groupElements is DialogueNode))
                        {
                            continue;
                        }

                        DialogueNode groupNode = (DialogueNode) groupElements;
                        
                        groupNodes.Add(groupNode);
                    }
                    
                    group.RemoveElements(groupNodes);
                    
                    RemoveGroup(group);
                    
                    RemoveElement(group);
                }
                
                DeleteElements(edgesToDelete);
                
                foreach (DialogueNode node in nodeToDelete)
                {
                    if (node.Group != null)
                    {
                        node.Group.RemoveElement(node);
                    }
                    
                    RemoveUngroupedNode(node);
                    
                    node.DisconnectAllPorts();
                    
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

                dialogueGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();
                
                if (string.IsNullOrEmpty(dialogueGroup.title))
                {
                    if (!string.IsNullOrEmpty(dialogueGroup.OldTitle))
                    {
                        ++RepeatedNamesAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(dialogueGroup.OldTitle))
                    {
                        --RepeatedNamesAmount;
                    }
                }
                
                RemoveGroup(dialogueGroup);

                dialogueGroup.OldTitle = dialogueGroup.title;
                
                AddGroup(dialogueGroup);
            };
        }

        private void OnGraphViewChanged()
        {
            graphViewChanged = (changes) =>
            {
                if (changes.edgesToCreate != null)
                {
                    foreach (Edge edge in changes.edgesToCreate)
                    {
                        DialogueNode nextNode = (DialogueNode) edge.input.node;

                        ChoiceSaveData choiceSaveData = (ChoiceSaveData) edge.output.userData;

                        choiceSaveData.NodeID = nextNode.ID;
                    }
                }

                if (changes.elementsToRemove != null)
                {
                    Type edgeType = typeof(Edge);

                    foreach (GraphElement element in changes.elementsToRemove)
                    {
                        if (element.GetType() != edgeType)
                        {
                            continue;
                        }

                        Edge edge = (Edge) element;

                        ChoiceSaveData choiceSaveData = (ChoiceSaveData) edge.output.userData;

                        choiceSaveData.NodeID = "";
                    }
                }

                return changes;
            };
        }

        #endregion
    
    #region Repeated Elements
    public void AddUngroupedNode(DialogueNode node)
    {
        string nodeName = node.DialogueName.ToLower();

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
            ++RepeatedNamesAmount;
            
            ungroupedNodesList[0].SetErrorColor(errorColor);
        }
    }

    public void RemoveUngroupedNode(DialogueNode node)
    {
        string nodeName = node.DialogueName.ToLower();

        var ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

        ungroupedNodesList.Remove(node);
        
        node.ResetStyle();

        if (ungroupedNodesList.Count == 1)
        {
            --RepeatedNamesAmount;
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
        string groupName = group.title.ToLower();

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
            ++RepeatedNamesAmount;
            groupsList[0].SetErrorStyle(errorColor);
        }
    }
    
    private void RemoveGroup(DialogueGroup group)
    {
        string oldGroupName = group.OldTitle.ToLower();

        List<DialogueGroup> groupsList = groups[oldGroupName].Groups;

        groupsList.Remove(group);
            
        group.ResetStyle();

        if (groupsList.Count == 1)
        {
            --RepeatedNamesAmount;
            
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
        string nodeName = node.DialogueName.ToLower();

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
            ++RepeatedNamesAmount;
            groupedNodes[group][nodeName].Nodes[0].SetErrorColor(errorColor);
        }
    }
    
    public void RemoveGroupedNode(DialogueNode node, Group group)
    {
        string nodeName = node.DialogueName.ToLower();

        node.Group = null;
        
        var groupedNodesList = groupedNodes[group][nodeName].Nodes;

        groupedNodesList.Remove(node);
        
        node.ResetStyle();

        if (groupedNodesList.Count == 1)
        {
            --RepeatedNamesAmount;
            
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
    
    private void AddMiniMap()
    {
        miniMap = new MiniMap()
        {
            anchored = true
        };
        
        miniMap.SetPosition(new Rect(15, 50, 200, 200));
        
        Add(miniMap);

        miniMap.visible = false;
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

    private void AddMiniMapStyles()
    {
        StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
        StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));

        miniMap.style.backgroundColor = backgroundColor;
        miniMap.style.borderBottomColor = borderColor;
        miniMap.style.borderLeftColor = borderColor;
        miniMap.style.borderRightColor = borderColor;
        miniMap.style.borderTopColor = borderColor;
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

    public void ClearGraph()
    {
        graphElements.ForEach(graphElement => RemoveElement(graphElement));
        
        groups.Clear();
        groupedNodes.Clear();
        ungroupedNodes.Clear();

        repeatedNamesAmount = 0;
    }

    public void ToggleMiniMap()
    {
        miniMap.visible = !miniMap.visible;
    }

    #endregion
}