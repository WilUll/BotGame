using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class StyleUtility
{
    public static VisualElement AddClasses(this VisualElement element, params string[] classNames)
    {
        foreach (var className in classNames)
        {
            element.AddToClassList(className);
        }

        return element;
    }
    
    public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheetNames)
    {
        foreach (var styleSheetName in styleSheetNames)
        {
            element.styleSheets.Add(Resources.Load<StyleSheet>(styleSheetName));
        }

        return element;
    }
}
