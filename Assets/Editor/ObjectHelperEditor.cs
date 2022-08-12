using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectHelperEditor : EditorWindow
{
    float minVal = 0;
    float minLimit = 0;
    float maxVal = 10;
    float maxLimit = 10;
    bool usesNormal = false;
    bool hide = false;
    string hideText = "Hide everything";

    [MenuItem("Build Tools/ObjectPlacer")]
    static void UIElementsExampleWindowMenuItem()
    {
        var window = EditorWindow.GetWindow<ObjectHelperEditor>();
        window.minSize = new UnityEngine.Vector2(150, 150f);
        window.maxSize = new UnityEngine.Vector2(window.minSize.x + 200f, window.minSize.y);
        window.Show();
    }
    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Snap To Ground"))
        {
            SnapToGround();
        }
        usesNormal = EditorGUILayout.Toggle("Use Normal", usesNormal);


        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
        if (GUILayout.Button("Random Rotation"))
        {
            RandomizeRotation();
        }

        GuiLine();

        EditorGUILayout.LabelField("Min Size:", minVal.ToString());
        EditorGUILayout.LabelField("Max Size:", maxVal.ToString());
        EditorGUILayout.MinMaxSlider(ref minVal, ref maxVal, minLimit, maxLimit);

        if (GUILayout.Button("Random Size"))
        {
            RandomSize(minVal, maxVal);
        }

        GuiLine(1, false);


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Redo"))
        {
            Undo.PerformRedo();

        }
        if (GUILayout.Button("Undo"))
        {
            Undo.PerformUndo();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button(hideText))
        {
            hide = !hide;
            HideObjects(hide);
        }


        GUILayout.EndVertical();
    }



    void SnapToGround()
    {
        foreach (var transform in Selection.transforms)
        {
            var hits = Physics.RaycastAll(transform.position + Vector3.up, Vector3.down, 50f);
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == transform.gameObject)
                    continue;
                Undo.RecordObject(transform, "SnappedObjects");

                transform.position = hit.point;

                if (usesNormal)
                {
                    transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                }

                break;
            }
        }
    }

    void RandomSize(float min, float max)
    {
        foreach (var transform in Selection.transforms)
        {
            float ranodomFloat = Random.Range(min, max);
            Undo.RecordObject(transform, "Scale");
            transform.localScale = new Vector3(ranodomFloat, ranodomFloat, ranodomFloat);
        }
    }

    void RandomizeRotation()
    {
        foreach (var transform in Selection.transforms)
        {
            Undo.RecordObject(transform, "Rotation");
            transform.rotation = Quaternion.Euler(transform.rotation.x, (float)Random.Range(0, 360), transform.rotation.z);
        }
    }

    void HideObjects(bool test)
    {
        if (hide)
        {
            hideText = "Show everything";

            foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (obj.GetComponent<MeshRenderer>())
                {
                    if (!obj.name.ToLower().Contains("terrain"))
                    {
                        obj.GetComponent<MeshRenderer>().enabled = false;
                    }
                }

            }
        }
        if (!test)
        {
            hideText = "Hide everything";

            foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
            {

                if (obj.GetComponent<MeshRenderer>())
                {
                    obj.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }
    }



    void GuiLine(int i_height = 1, bool spaceAbove = true)
    {
        if (spaceAbove)
        {
            GUILayout.Space(10f);
        }
        Rect rect = EditorGUILayout.GetControlRect(false, i_height);

        rect.height = i_height;

        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        if (!spaceAbove)
        {
            GUILayout.Space(10f);
        }
    }
}
