using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class IOUtility
{
    private static DialogueGraphView graphView;
    private static string graphFilename;
    private static string containerFolderPath;

    private static List<DialogueGroup> groups;
    private static List<DialogueNode> nodes;

    private static Dictionary<string, DialogueGroupSO> createdDialogueGroups;
    private static Dictionary<string, DialogueSO> createdDialogue;

    private static Dictionary<string, DialogueGroup> loadedGroups;
    private static Dictionary<string, DialogueNode> loadedNodes;

    public static void Initialize(DialogueGraphView dsGraphView, string graphName)
    {
        graphView = dsGraphView;
        graphFilename = graphName;
        containerFolderPath = $"Assets/DialogueSystem/Dialogues/{graphFilename}";

        groups = new List<DialogueGroup>();
        nodes = new List<DialogueNode>();

        createdDialogueGroups = new Dictionary<string, DialogueGroupSO>();
        createdDialogue = new Dictionary<string, DialogueSO>();

        loadedGroups = new Dictionary<string, DialogueGroup>();
        loadedNodes = new Dictionary<string, DialogueNode>();
    }
    #region Save Methods

    public static void Save()
    {
        CreateStaticFolder();

        GetElementsFromGraphView();

        GraphSaveDataSO graphData = CreateAsset<GraphSaveDataSO>("Assets/Scripts/DialogueSystem/Graphs", 
            $"{graphFilename}Graph");
        
        graphData.Initialize(graphFilename);

        DialogueContainerSO dialogueContainer = CreateAsset<DialogueContainerSO>(containerFolderPath, graphFilename);
        
        dialogueContainer.Initialize(graphFilename);

        SaveGroups(graphData, dialogueContainer);
        SaveNodes(graphData, dialogueContainer);
        
        SaveAsset(graphData);
        SaveAsset(dialogueContainer);
    }

    #endregion

    #region Load Methods
    public static void Load()
    {
        GraphSaveDataSO graphData = LoadAsset<GraphSaveDataSO>("Assets/Scripts/DialogueSystem/Graphs", graphFilename);

        if (graphData == null)
        {
            EditorUtility.DisplayDialog(
                "Couldn't load the file!",
                "The file at the following path could not be found:\n\n" +
                $"Assets/Scripts/DialogueSystem/Graphs{graphFilename}\n\n" +
                "Make sure you chose the right file and its placed at the correct path",
                "Thanks!"
            );
            
            return;
        }
        
        DialogueGraphWindow.UpdateFileName(graphData.FileName);

        LoadGroups(graphData.Groups);
        LoadNodes(graphData.Nodes);
        LoadNodeConnections();
    }

    private static void LoadNodeConnections()
    {
        foreach (KeyValuePair<string,DialogueNode> loadedNode in loadedNodes)
        {
            foreach (Port choicePort in loadedNode.Value.outputContainer.Children())
            {
                ChoiceSaveData choiceData = (ChoiceSaveData) choicePort.userData;

                if (string.IsNullOrEmpty(choiceData.NodeID))
                {
                    continue;
                }

                DialogueNode nextNode = loadedNodes[choiceData.NodeID];

                Port nextNodeInputPort = (Port) nextNode.inputContainer.Children().First();

                Edge edge = choicePort.ConnectTo(nextNodeInputPort);
                
                graphView.AddElement(edge);

                loadedNode.Value.RefreshPorts();
            }
        }
    }


    private static void LoadGroups(List<GroupSaveData> graphDataGroups)
    {
        foreach (GroupSaveData groupData in graphDataGroups)
        {
            DialogueGroup group = graphView.CreateGroup(groupData.Name, groupData.Position);

            group.ID = groupData.ID;
            
            loadedGroups.Add(group.ID, group);
        }
    }
    
    private static void LoadNodes(List<NodeSaveData> graphDataNodes)
    {
        foreach (NodeSaveData nodeData in graphDataNodes)
        {
            List<ChoiceSaveData> choice = CloneNodeChoices(nodeData.Choices);
            
            DialogueNode node = graphView.CreateNode(nodeData.Name, nodeData.DialogueType, nodeData.Position, false);

            node.ID = nodeData.ID;
            node.Choices = choice;
            node.Text = nodeData.Text;
            
            node.Draw();
            
            graphView.AddElement(node);
            
            loadedNodes.Add(node.ID, node);
            
            if (string.IsNullOrEmpty(nodeData.GroupID))
            {
                continue;
            }

            DialogueGroup group = loadedGroups[nodeData.GroupID];

            node.Group = group;
            
            group.AddElement(node);
        }
    }


    #endregion

    #region Groups

    private static void SaveGroups(GraphSaveDataSO graphData, DialogueContainerSO dialogueContainer)
    {
        List<string> groupNames = new List<string>();
        foreach (DialogueGroup group in groups)
        {
            SaveGroupToGraph(group, graphData);
            SaveGroupToScriptableObject(group, dialogueContainer);
            
            groupNames.Add(group.title);
        }

        UpdateOldGroups(groupNames, graphData);
    }

    private static void UpdateOldGroups(List<string> currentGroupNames,GraphSaveDataSO graphData)
    {
        if (graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count != 0)
        {
            List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();

            foreach (string groupToRemove in groupsToRemove)
            {
                RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");
            }
        }

        graphData.OldGroupNames = new List<string>(currentGroupNames);
    }

    private static void SaveGroupToGraph(DialogueGroup group, GraphSaveDataSO graphData)
    {
        GroupSaveData groupSaveData = new GroupSaveData()
        {
            ID = group.ID,
            Name = group.title,
            Position = group.GetPosition().position
        };
        
        graphData.Groups.Add(groupSaveData);
    }
    private static void SaveGroupToScriptableObject(DialogueGroup group, DialogueContainerSO dialogueContainer)
    {
        string groupName = group.title;
        
        CreateFolder($"{containerFolderPath}/Groups", groupName);
        CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Dialogues");

        DialogueGroupSO dialogueGroup =
            CreateAsset<DialogueGroupSO>($"{containerFolderPath}/Groups/{groupName}", groupName);
        
        dialogueGroup.Initialize(groupName);
        dialogueContainer.DialogueGroup.Add(dialogueGroup, new List<DialogueSO>());
        
        createdDialogueGroups.Add(group.ID, dialogueGroup);

        SaveAsset(dialogueGroup);
    }
    


    #endregion

    #region Nodes

    private static void SaveNodes(GraphSaveDataSO graphData, DialogueContainerSO dialogueContainer)
    {
        SerializableDictionary<string, List<string>> groupedNodeNames =
            new SerializableDictionary<string, List<string>>();
        List<string> ungroupedNodeNames = new List<string>();
        foreach (DialogueNode node in nodes)
        {
            SaveNodeToGraph(node, graphData);
            SaveNodeToScriptableObject(node, dialogueContainer);

            if (node.Group != null)
            {
                groupedNodeNames.AddItem(node.Group.title, node.DialogueName);
                
                continue;
            }
            
            ungroupedNodeNames.Add(node.DialogueName);
        }

        UpdateDialogueChoicesConnections();

        UpdateOldGroupedNodes(groupedNodeNames, graphData);
        UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
    }



    private static void SaveNodeToScriptableObject(DialogueNode node, DialogueContainerSO dialogueContainer)
    {
        DialogueSO dialogue;

        if (node.Group != null)
        {
            dialogue = CreateAsset<DialogueSO>($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues",
                node.DialogueName);
            
            dialogueContainer.DialogueGroup.AddItem(createdDialogueGroups[node.Group.ID], dialogue);
        }
        else
        {
            dialogue = CreateAsset<DialogueSO>($"{containerFolderPath}/Global/Dialogue", node.DialogueName);
            
            dialogueContainer.UngroupedDialogues.Add(dialogue);
        }
        
        dialogue.Initialize(
            node.DialogueName,
            node.Text,
            ConvertNodeChoicesToDialogueChoices(node.Choices),
            node.DialogueType,
            node.IsStartingNode()
            );
        
        createdDialogue.Add(node.ID, dialogue);
        
        SaveAsset(dialogue);
    }

    private static List<DialogueChoiceData> ConvertNodeChoicesToDialogueChoices(List<ChoiceSaveData> nodeChoices)
    {
        List<DialogueChoiceData> dialogueChoice = new List<DialogueChoiceData>();

        foreach (ChoiceSaveData nodeChoice in nodeChoices)
        {
            DialogueChoiceData choiceData = new DialogueChoiceData()
            {
                Text = nodeChoice.Text
            };
            
            dialogueChoice.Add(choiceData);
        }

        return dialogueChoice;
    }

    private static void SaveNodeToGraph(DialogueNode node, GraphSaveDataSO graphData)
    {
        List<ChoiceSaveData> choices = CloneNodeChoices(node.Choices);
        NodeSaveData nodeData = new NodeSaveData()
        {
            ID = node.ID,
            Name = node.DialogueName,
            Choices = choices,
            Text = node.Text,
            GroupID = node.Group?.ID,
            DialogueType = node.DialogueType,
            Position = node.GetPosition().position
        };
        
        graphData.Nodes.Add(nodeData);
    }

    private static void UpdateDialogueChoicesConnections()
    {
        foreach (DialogueNode node in nodes)
        {
            DialogueSO dialogue = createdDialogue[node.ID];

            for (int choiceIndex = 0; choiceIndex < node.Choices.Count; choiceIndex++)
            {
                ChoiceSaveData nodeChoice = node.Choices[choiceIndex];

                if (string.IsNullOrEmpty(nodeChoice.NodeID))
                {
                    continue;
                }

                dialogue.Choices[choiceIndex].NextDialogue = createdDialogue[nodeChoice.NodeID];
                
                SaveAsset(dialogue);
            }
        }
    }
    
    
    private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, 
        GraphSaveDataSO graphData)
    {
        if (graphData.OldUngroupedNodeNames != null && graphData.OldUngroupedNodeNames.Count != 0)
        {
            foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeNames)
            {
                List<string> nodesToRemove = new List<string>();

                if (currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                {
                    nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key]).ToList();
                }

                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{containerFolderPath}/Groups/{oldGroupedNode.Key}/Dialogues", nodeToRemove);
                }
            }
        }

        graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
    }
    
    private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, GraphSaveDataSO graphData)
    {
        if (graphData.OldUngroupedNodeNames != null && graphData.OldUngroupedNodeNames.Count != 0)
        {
            List<string> nodesToRemove = graphData.OldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();

            foreach (string nodeToRemove in nodesToRemove)
            {
                RemoveAsset($"{containerFolderPath}/Global/Dialogues", nodeToRemove);
            }
        }

        graphData.OldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);
    }

    #endregion
    
    #region Fetch Methods

    private static void GetElementsFromGraphView()
    {
        Type groupType = typeof(DialogueGroup);
        graphView.graphElements.ForEach(graphElement =>
        {
            if (graphElement is DialogueNode node)
            {
                nodes.Add(node);
                
                return;
            }

            if (graphElement.GetType() == groupType)
            {
                DialogueGroup group = (DialogueGroup) graphElement;
                
                groups.Add(group);
                
                return;
            }
        });
    }

    #endregion
    
    #region Creation Methods

    public static void CreateFolder(string path, string folderName)
    {
        if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
        {
            return;
        }

        AssetDatabase.CreateFolder(path, folderName);
    }

    #endregion

    #region Utility Methods

    public static void CreateStaticFolder()
    {
        CreateFolder("Assets/Scripts/DialogueSystem", "Graphs");
        CreateFolder("Assets", "DialogueSystem");
        CreateFolder("Assets/DialogueSystem", "Dialogues");
        CreateFolder("Assets/DialogueSystem/Dialogues", graphFilename);
        CreateFolder(containerFolderPath, "Global");
        CreateFolder(containerFolderPath, "Groups");
        CreateFolder($"{containerFolderPath}/Global", "Dialogue");
    }
    
    public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
    {
        string fullPath = $"{path}/{assetName}.asset";

        T asset = (T) LoadAsset<T>(path, assetName);

        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();
        
            AssetDatabase.CreateAsset(asset, fullPath);
        }

        return asset;
    }
    
    public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
    {
        string fullPath = $"{path}/{assetName}.asset";

        return AssetDatabase.LoadAssetAtPath<T>(fullPath);
    }
    
    public static void RemoveFolder(string fullPath)
    {
        FileUtil.DeleteFileOrDirectory($"{fullPath}.meta");
        FileUtil.DeleteFileOrDirectory($"{fullPath}/");
    }
    
    public static void RemoveAsset(string path, string nodeToRemove)
    {
        AssetDatabase.DeleteAsset($"{path}/{nodeToRemove}.asset");
    }
    
    public static void SaveAsset(UnityEngine.Object asset)
    {
        EditorUtility.SetDirty(asset);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private static List<ChoiceSaveData> CloneNodeChoices(List<ChoiceSaveData> nodeChoices)
    {
        List<ChoiceSaveData> choices = new List<ChoiceSaveData>();
        foreach (ChoiceSaveData choice in nodeChoices)
        {
            ChoiceSaveData choiceData = new ChoiceSaveData()
            {
                Text = choice.Text,
                NodeID = choice.NodeID
            };

            choices.Add(choiceData);
        }

        return choices;
    }
    #endregion
}
