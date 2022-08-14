using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphWindow : EditorWindow
{
    private DialogueGraphView graphView;
    private readonly string defaultFileName = "DialogueName";
    private static TextField fieldNameTextField;
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
        graphView = new DialogueGraphView(this);

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

        saveButton = ElementUtility.CreateButton("Save", () => Save());

        Button loadButton = ElementUtility.CreateButton("Load", () => Load());
        Button clearButton = ElementUtility.CreateButton("Clear", () => Clear());
        Button resetButton = ElementUtility.CreateButton("Reset", () => ResetGraph());
        Button miniMapButton = ElementUtility.CreateButton("Minimap", () => ToggleMiniMap());

        toolbar.Add(fieldNameTextField);
        toolbar.Add(saveButton);
        toolbar.Add(loadButton);
        toolbar.Add(clearButton);
        toolbar.Add(resetButton);
        toolbar.Add(miniMapButton);


        toolbar.AddStyleSheets("DialogueSystem/ToolbarStyles");
        
        rootVisualElement.Add(toolbar);
    }

    private void ToggleMiniMap()
    {
        graphView.ToggleMiniMap();
    }


    private void ResetGraph()
    {
        if (EditorUtility.DisplayDialog(
                "Reset the graph?",
                "Are you sure?",
                "Yes",
                "No"))
        {
            graphView.ClearGraph();
            
            UpdateFileName(defaultFileName);
        }
    }

    private void Load()
    {
        string filePath = EditorUtility.OpenFilePanel("Dialogue Graphs", "Assets/Scripts/DialogueSystem/Graphs", "asset");

        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }
        
        graphView.ClearGraph();
        
        IOUtility.Initialize(graphView, Path.GetFileNameWithoutExtension(filePath));
        IOUtility.Load();
    }
    
    private void Clear()
    {
        if (EditorUtility.DisplayDialog(
                "Clear the graph?",
                "Are you sure?",
                "Yes",
                "No"))
        {
            graphView.ClearGraph();
        }
    }

    private void Save()
    {
        if (string.IsNullOrEmpty(fieldNameTextField.value))
        {
            EditorUtility.DisplayDialog(
                "Invalid file name",
                "Ehhh du måste ha ett namn på filen Sandra..",
                "OK"
            );
            
            return;
        }
        IOUtility.Initialize(graphView, fieldNameTextField.value);
        IOUtility.Save();
    }

    private void AddStyles()
    {
        rootVisualElement.AddStyleSheets("DialogueSystem/GraphVariables");
    }
    #endregion

    #region Utility Methods

    public static void UpdateFileName(string newFileName)
    {
        fieldNameTextField.value = newFileName;
    }

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
