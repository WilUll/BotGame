using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public static class ElementUtility
{
    public static Button CreateButton(string text, Action onClick = null)
    {
        Button button = new Button(onClick)
        {
            text = text
        };

        return button;
    }
    
    public static Foldout CreateFoldout(string title, bool collapsed = false)
    {
        Foldout foldout = new Foldout()
        {
            text = title,
            value = !collapsed
        };

        return foldout;
    }

    public static Port CreatePort(this DialogueNode node, string portName = "", Orientation orientation = Orientation.Horizontal, 
        Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
    {
        Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));

        port.portName = portName;

        return port;
    }
    
    public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textField = new TextField()
        {
            value = value,
            label = label
        };

        if (onValueChanged != null)
        {
            textField.RegisterValueChangedCallback(onValueChanged);
        }

        return textField;
    }

    public static EnumField CreateEnumField(Enum value, string label, EventCallback<ChangeEvent<Enum>> onValueChanged = null)
    {
        EnumField enumField = new EnumField(label, value);
        
        if (onValueChanged != null)
        {
            enumField.RegisterValueChangedCallback(onValueChanged);
        }

        return enumField;
    }

    public static ObjectField CreateAudioField(string label = "Object Field Clip", EventCallback<ChangeEvent<Object>> onValueChanged = null)
    {
        ObjectField objectField = new ObjectField(label);
        objectField.objectType = typeof(AudioClip);
        if (onValueChanged != null)
        {
            objectField.RegisterValueChangedCallback(onValueChanged);
        }

        return objectField;
    }

    public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textArea = CreateTextField(value, label, onValueChanged);

        textArea.multiline = true;

        return textArea;
    }
}
