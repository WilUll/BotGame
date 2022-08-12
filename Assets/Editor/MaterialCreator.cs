using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering.HighDefinition;

public class MaterialCreator : EditorWindow
{
    public Shader shader;

    private string path;
    
    //Values
    private float minMetallic = 0, maxMetallic = 1;
    private float minSmoothness = 0, maxSmoothness = 1;
    private float minAO = 0, maxAO = 1;

    float minLimit = 0, maxLimit = 1;
    [MenuItem("Tools/CreateMaterialsForTextures")]
    static void CreateWindow()
    {
        GetWindow<MaterialCreator>("CreateMaterialsForTextures");
    }

    private void OnGUI()
    {
        GUILayout.Label("Metallic");
        EditorGUILayout.MinMaxSlider(ref minMetallic, ref maxMetallic, minLimit, maxLimit);
        GUILayout.Label("Smoothness");
        EditorGUILayout.MinMaxSlider(ref minSmoothness, ref maxSmoothness, minLimit, maxLimit);
        GUILayout.Label("AO");
        EditorGUILayout.MinMaxSlider(ref minAO, ref maxAO, minLimit, maxLimit);
        if (GUILayout.Button("CreateMaterial"))
        {
            CreateMaterial();
        }
    }

    void OnEnable()
    {
        shader = Shader.Find("HDRP/Lit");
    }
 
    void CreateMaterial()
    {
        try
        {
            var mat = new Material(shader);
            AssetDatabase.StartAssetEditing();
            var textures = Selection.GetFiltered(typeof(Texture), SelectionMode.Assets).Cast<Texture>();
            foreach(var tex in textures)
            {
                path = AssetDatabase.GetAssetPath(tex);
                string folderName = path.Remove(path.LastIndexOf('/'));
                folderName = folderName.Substring(folderName.LastIndexOf('/') + 1);
                path = path.Remove(path.LastIndexOf('/')) + "/M_" + folderName + ".mat";
                if (AssetDatabase.LoadAssetAtPath(path,typeof(Material)) != null)
                {
                    Debug.LogWarning("Can't create material, it already exists: " + path);
                    continue;
                }

                if (tex.name.ToLower().Contains("basemap"))
                {
                    mat.SetTexture("_BaseColorMap", tex);
                }
                if (tex.name.ToLower().Contains("maskmap"))
                {
                    mat.SetTexture("_MaskMap", tex);
                }
                if (tex.name.ToLower().Contains("normal"))
                {
                    mat.SetTexture("_NormalMapOS", tex);
                }
                if (tex.name.ToLower().Contains("specular"))
                {
                    mat.SetTexture("_DetailMap", tex);
                }
                


                AssetDatabase.GetAssetPath(tex);
            }
            
            //Last settings
            mat.SetFloat("_NormalMapSpace", 1);
            
            mat.SetFloat("_MetallicRemapMin", minMetallic);
            mat.SetFloat("_MetallicRemapMax", maxMetallic);
            
            mat.SetFloat("_SmoothnessRemapMin", minSmoothness);
            mat.SetFloat("_SmoothnessRemapMax", maxSmoothness);
            
            mat.SetFloat("_AORemapMin", minAO);
            mat.SetFloat("_AORemapMax", maxAO);


            AssetDatabase.CreateAsset(mat,path);

        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
        }
    }
}