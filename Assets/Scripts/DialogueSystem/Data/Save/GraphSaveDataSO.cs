using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphSaveDataSO : ScriptableObject
{
    [field: SerializeField] public string FileName { get; set; }
    [field: SerializeField] public List<GroupSaveData> Groups { get; set; }
    [field: SerializeField] public List<NodeSaveData> Nodes { get; set; }
    [field: SerializeField] public List<string> OldGroupNames { get; set; }
    [field: SerializeField] public List<string> OldUngroupedNodeNames { get; set; }
    [field: SerializeField] public SerializableDictionary<string, List<string>> OldGroupedNames { get; set; }

    public void Initialize(string fileName)
    {
        FileName = fileName;

        Groups = new List<GroupSaveData>();
        Nodes = new List<NodeSaveData>();
    }
}
