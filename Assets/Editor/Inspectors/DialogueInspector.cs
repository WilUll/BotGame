using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Dialogue))]
public class DialogueInspector : Editor
{
    /* DIALOGUE SCRIPTABLE OBJECTS */
    private SerializedProperty dialogueContainerProperty;
    private SerializedProperty dialogueGroupProperty;
    private SerializedProperty dialogueProperty;

    /* FILTERS */
    private SerializedProperty groupedDialoguesProperty;
    private SerializedProperty startingDialoguesOnlyProperty;

    /* INDEXES */
    private SerializedProperty selectedDialogueGroupIndexProperty;
    private SerializedProperty selectedDialogueIndexProperty;

    private void OnEnable()
    {
        dialogueContainerProperty = serializedObject.FindProperty("dialogueContainer");
        dialogueGroupProperty = serializedObject.FindProperty("dialogueGroup");
        dialogueProperty = serializedObject.FindProperty("dialogue");

        groupedDialoguesProperty = serializedObject.FindProperty("groupedDialogues");
        startingDialoguesOnlyProperty = serializedObject.FindProperty("startingDialoguesOnly");

        selectedDialogueGroupIndexProperty = serializedObject.FindProperty("selectedDialogueGroupIndex");
        selectedDialogueIndexProperty = serializedObject.FindProperty("selectedDialogueIndex");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDialogueContainerArea();

        DialogueContainerSO dialogueContainer = (DialogueContainerSO) dialogueContainerProperty.objectReferenceValue;

        if (dialogueContainer == null)
        {
            StopDrawing("Select a Dialogue Container to see the rest of the inspector");

            return;
        }

        DrawFiltersArea();

        bool currentStartingDialoguesOnlyFilter = startingDialoguesOnlyProperty.boolValue;

        List<string> dialogueNames;

        string dialogueFolderPath = $"Assets/DialogueSystem/Dialogues/{dialogueContainer.FileName}";

        string dialogueInfoMessage;

        if (groupedDialoguesProperty.boolValue)
        {
            List<string> dialogueGroupNames = dialogueContainer.GetDialogueGroupNames();

            if (dialogueGroupNames.Count == 0)
            {
                StopDrawing("There are no dialogue groups in this dialogue container");

                return;
            }
            
            DrawDialogueGroupArea(dialogueContainer, dialogueGroupNames);

            DialogueGroupSO dialogueGroup = (DialogueGroupSO)dialogueGroupProperty.objectReferenceValue;
            
            dialogueNames = dialogueContainer.GetGroupedDialogueNames(dialogueGroup, currentStartingDialoguesOnlyFilter);

            dialogueFolderPath += $"/Groups/{dialogueGroup.GroupName}/Dialogues";

            dialogueInfoMessage = "There are no"+ (currentStartingDialoguesOnlyFilter ? " Starting": "") +" Dialogues in this Dialogue Group";
        }
        else
        {
            dialogueNames = dialogueContainer.GetUngroupedDialogueNames(currentStartingDialoguesOnlyFilter);
            dialogueFolderPath += "/Global/Dialogues";
            dialogueInfoMessage = "There are no"+ (currentStartingDialoguesOnlyFilter ? " Starting": "") +" Ungrouped Dialogues in this Dialogue Container";
        }

        if (dialogueNames.Count == 0)
        {
            StopDrawing(dialogueInfoMessage);
            
            return;
        }

        DrawDialogueArea(dialogueNames, dialogueFolderPath);


        serializedObject.ApplyModifiedProperties();
    }

    private void StopDrawing(string reason, MessageType messageType = MessageType.Info)
    {
        InspectorUtility.DrawHelpBox(reason, messageType);
        
        InspectorUtility.DrawSpace();
        
        InspectorUtility.DrawHelpBox("You need to select a Dialogue for this component to work at Runtime!", MessageType.Warning);

        serializedObject.ApplyModifiedProperties();
    }

    #region DrawMethod

    private void DrawDialogueContainerArea()
    {
        InspectorUtility.DrawHeader("Dialogue Container");

        dialogueContainerProperty.DrawPropertyField();

        InspectorUtility.DrawSpace();
    }

    private void DrawFiltersArea()
    {
        InspectorUtility.DrawHeader("Filters");

        groupedDialoguesProperty.DrawPropertyField();
        startingDialoguesOnlyProperty.DrawPropertyField();

        InspectorUtility.DrawSpace();
    }

    private void DrawDialogueGroupArea(DialogueContainerSO dialogueContainer, List<string> dialogueGroupNames)
    {
        InspectorUtility.DrawHeader("Dialogue Group");

        int oldSelectedDialogueGroupIndex = selectedDialogueGroupIndexProperty.intValue;

        DialogueGroupSO oldDialogueGroup = (DialogueGroupSO) dialogueGroupProperty.objectReferenceValue;

        bool isOldDialogueGroup = oldDialogueGroup == null;

        string oldDialogueGroupName = isOldDialogueGroup ? "" : oldDialogueGroup.GroupName;
        
        UpdateIndexOnNamesListUpdate(dialogueGroupNames, selectedDialogueGroupIndexProperty, 
            oldSelectedDialogueGroupIndex, oldDialogueGroupName, isOldDialogueGroup);

        selectedDialogueGroupIndexProperty.intValue = InspectorUtility.DrawPopup("Dialogue Group",
            selectedDialogueGroupIndexProperty, dialogueGroupNames.ToArray());

        string selectedDialogueGroupName = dialogueGroupNames[selectedDialogueGroupIndexProperty.intValue];

        DialogueGroupSO selectedDialogueGroup = IOUtility.LoadAsset<DialogueGroupSO>(
            $"Assets/DialogueSystem/Dialogues/{dialogueContainer.FileName}/Groups/{selectedDialogueGroupName}",
            selectedDialogueGroupName);

        dialogueGroupProperty.objectReferenceValue = selectedDialogueGroup;

        InspectorUtility.DrawDisabledFields(() => dialogueGroupProperty.DrawPropertyField());

        InspectorUtility.DrawSpace();
    }

    private void DrawDialogueArea(List<string> dialogueNames, string dialogueFolderPath)
    {
        InspectorUtility.DrawHeader("Dialogue");

        int oldSelectedDialogueIndex = selectedDialogueIndexProperty.intValue;

        DialogueSO oldDialogue = (DialogueSO) dialogueProperty.objectReferenceValue;
        
        bool isOldDialogue = oldDialogue == null;

        string oldDialogueName = isOldDialogue ? "" : oldDialogue.DialogueName;
        
        UpdateIndexOnNamesListUpdate(dialogueNames, selectedDialogueIndexProperty, oldSelectedDialogueIndex, oldDialogueName, isOldDialogue);


        selectedDialogueIndexProperty.intValue =
            InspectorUtility.DrawPopup("Dialogue", selectedDialogueIndexProperty, dialogueNames.ToArray());

        string selectedDialogueName = dialogueNames[selectedDialogueIndexProperty.intValue];

        DialogueSO selectedDialogue = IOUtility.LoadAsset<DialogueSO>(dialogueFolderPath, selectedDialogueName);

        dialogueProperty.objectReferenceValue = selectedDialogue;
        
        InspectorUtility.DrawDisabledFields(() => dialogueProperty.DrawPropertyField());

        InspectorUtility.DrawSpace();
    }

    #endregion

    #region Index Methods

    private void UpdateIndexOnNamesListUpdate(List<string> optionNames, SerializedProperty indexProperty, int oldSelectedPropertyIndex, 
        string oldPropertyName, bool isOldPropertyNull)
    {
        if (isOldPropertyNull)
        {
            indexProperty.intValue = 0;

            return;
        }

        bool oldIndexIsOutOfBounds = oldSelectedPropertyIndex > optionNames.Count - 1;
        bool oldNameIsDifferent = oldIndexIsOutOfBounds || oldPropertyName != optionNames[oldSelectedPropertyIndex];

        if (oldNameIsDifferent)
        {
            if (optionNames.Contains(oldPropertyName))
            {
                indexProperty.intValue =
                    optionNames.IndexOf(oldPropertyName);
            }
            else
            {
                indexProperty.intValue = 0;
            }
        }
    }

    #endregion
}