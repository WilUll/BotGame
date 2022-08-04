using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView graphView;
    private string fileName = "New Narrative";
    
    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void ConstructGraphView()
    {
        graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };
        
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => fileName = evt.newValue);
        toolbar.Add(fileNameTextField);
        
        toolbar.Add(new Button(()=> SaveData()){text = "Save Data"});
        toolbar.Add(new Button(()=> LoadData()){text = "Load Data"});


        var nodeCreateButton = new Button(() =>
        {
            graphView.CreateNode("Dialogue Node");
        });
        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);
        
        rootVisualElement.Add(toolbar);
    }

    private void LoadData()
    {
        throw new NotImplementedException();
    }

    private void SaveData()
    {
        throw new NotImplementedException();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }
}
