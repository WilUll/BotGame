using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GroupErrorData
{
    public ErrorData ErrorDataScript { get; set; }
    
    public List<DialogueGroup> Groups { get; set; }

    public GroupErrorData()
    {
        ErrorDataScript = new ErrorData();
        Groups = new List<DialogueGroup>();
    }
}
