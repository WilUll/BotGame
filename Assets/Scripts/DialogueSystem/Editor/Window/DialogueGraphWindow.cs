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

    private void AddStyles()
    {
        rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("DialogueSystem/GraphVariables"));
    }

    private void AddGraphview()
    {
        DialogueGraphView graphView = new DialogueGraphView();

        graphView.StretchToParentSize();

        rootVisualElement.Add(graphView);
    }
}
