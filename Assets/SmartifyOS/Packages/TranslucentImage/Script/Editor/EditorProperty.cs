using System;
using System.Globalization;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.UI.TranslucentImage.Editor
{
public class EditorProperty
{
    public readonly SerializedProperty serializedProperty;

    readonly SerializedObject   serializedObject;
    readonly MethodInfo         propertySetter;
    readonly SerializedProperty dirtyFlag;

    public EditorProperty(SerializedObject obj, string name)
    {
        var propertyName = char.ToLowerInvariant(name[0]) + name.Substring(1);
        serializedObject   = obj;
        serializedProperty = serializedObject.FindProperty(propertyName);
        propertySetter     = serializedObject.targetObject.GetType().GetProperty(name).SetMethod;
        dirtyFlag          = serializedObject.FindProperty("modifiedFromInspector");
    }

    public void Draw(params GUILayoutOption[] options)
    {
        using (var scope = new EditorGUI.ChangeCheckScope())
        {
            EditorGUILayout.PropertyField(serializedProperty, options);

            if (!scope.changed)
                return;

            if (dirtyFlag != null)
                dirtyFlag.boolValue = true;

            serializedObject.ApplyModifiedProperties();

            foreach (var target in serializedObject.targetObjects)
            {
                switch (serializedProperty.propertyType)
                {
                case SerializedPropertyType.ObjectReference:
                    propertySetter.Invoke(target, new object[] { serializedProperty.objectReferenceValue });
                    break;
                case SerializedPropertyType.Float:
                    propertySetter.Invoke(target, new object[] { serializedProperty.floatValue });
                    break;
                case SerializedPropertyType.Integer:
                    propertySetter.Invoke(target, new object[] { serializedProperty.intValue });
                    break;
                case SerializedPropertyType.Rect:
                    propertySetter.Invoke(target, new object[] { serializedProperty.rectValue });
                    break;
                case SerializedPropertyType.Enum:
                    propertySetter.Invoke(target, new object[] { serializedProperty.enumValueIndex });
                    break;
                case SerializedPropertyType.Boolean:
                    propertySetter.Invoke(target, new object[] { serializedProperty.boolValue });
                    break;
                case SerializedPropertyType.Color:
                    propertySetter.Invoke(target, new object[] { serializedProperty.colorValue });
                    break;
                case SerializedPropertyType.Generic:
                    // Not needed for now
                    break;
                default: throw new NotImplementedException($"Type {serializedProperty.propertyType} is not implemented");
                }
            }

            serializedObject.Update();
        }
    }
}
}
