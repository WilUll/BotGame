using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphWindow : EditorWindow
{
    [MenuItem("Window/Graph")]
    public static void OpenGraph()
    {
        GetWindow<DialogueGraphWindow>("Dialogue Graph");
    }

    private void OnEnable()
    {
        AddGraphview();

        AddStyles();
    }

    #region Elements Addition
    private void AddGraphview()
    {
        DialogueGraphView graphView = new DialogueGraphView(this);

        graphView.StretchToParentSize();

        rootVisualElement.Add(graphView);
    }
    
    private void AddStyles()
    {
        rootVisualElement.AddStyleSheets("DialogueSystem/GraphVariables");
    }
    #endregion
}
