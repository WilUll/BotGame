using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphWindow : EditorWindow
{
    private readonly string defaultFileName = "DialogueName";
    private TextField fieldNameTextField;
    private Button saveButton;
    [MenuItem("Window/Graph")]
    public static void OpenGraph()
    {
        GetWindow<DialogueGraphWindow>("Dialogue Graph");
    }

    private void OnEnable()
    {
        AddGraphview();
        AddToolbar();

        AddStyles();
    }
    
    #region Elements Addition
    private void AddGraphview()
    {
        DialogueGraphView graphView = new DialogueGraphView(this);

        graphView.StretchToParentSize();

        rootVisualElement.Add(graphView);
    }
    
    private void AddToolbar()
    {
        Toolbar toolbar = new Toolbar();

        fieldNameTextField = ElementUtility.CreateTextField(defaultFileName, "File Name:", callback =>
        {
            fieldNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
        });

        saveButton = ElementUtility.CreateButton("Save");
        
        toolbar.Add(fieldNameTextField);
        toolbar.Add(saveButton);

        toolbar.AddStyleSheets("DialogueSystem/ToolbarStyles");
        
        rootVisualElement.Add(toolbar);
    }
    
    private void AddStyles()
    {
        rootVisualElement.AddStyleSheets("DialogueSystem/GraphVariables");
    }
    #endregion

    #region Utility Methods

    public void EnableSaving()
    {
        saveButton.SetEnabled(true);
    }
    public void DisableSaving()
    {
        saveButton.SetEnabled(false);
    }
    #endregion
}
