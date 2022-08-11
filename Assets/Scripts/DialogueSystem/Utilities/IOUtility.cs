using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class IOUtility
{
    private static string graphFilename;
    private static string containerFolderPath;
    
    public static void Initialize(string graphName)
    {
        graphFilename = graphName;
        containerFolderPath = $"Assets/DialogueSystem/Dialogues/{graphFilename}";
    }
    #region Save Methods

    public static void Save()
    {
        CreateStaticFolder();
    }



    #endregion

    #region Creation Methods

    private static void CreateStaticFolder()
    {
        CreateFolder("Assets/Scripts/DialogueSystem", "Graphs");
        CreateFolder("Assets", "DialogueSystem");
        CreateFolder("Assets/DialogueSystem", "Dialogues");

        CreateFolder("Assets/DialogueSystem/Dialogues", graphFilename);
        CreateFolder(containerFolderPath, "Global");
        CreateFolder(containerFolderPath, "Groups");

    }

    private static void CreateFolder(string path, string folderName)
    {
        if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
        {
            return;
        }

        AssetDatabase.CreateFolder(path, folderName);
    }

    #endregion
}
