using System.Linq;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace SmartifyOS.UI.TranslucentImage.Editor
{
[CustomEditor(typeof(BlurredImage))]
[CanEditMultipleObjects]
public class TranslucentImageEditor : ImageEditor
{
    SerializedProperty spriteBlending;
    SerializedProperty source;
    SerializedProperty vibrancy;
    SerializedProperty brightness;
    SerializedProperty flatten;

    bool materialUsedInDifferentSource;
    bool usingIncorrectShader;

    Shader correctShader;

    protected override void OnEnable()
    {
        base.OnEnable();

        source         = serializedObject.FindProperty("source");
        spriteBlending = serializedObject.FindProperty("m_spriteBlending");
        vibrancy       = serializedObject.FindProperty("vibrancy");
        brightness     = serializedObject.FindProperty("brightness");
        flatten        = serializedObject.FindProperty("flatten");

        correctShader = Shader.Find("UI/TranslucentImage");

        var self = serializedObject.targetObject as BlurredImage;
        if (self)
        {
            CheckMaterialUsedInDifferentSource(self);
            CheckCorrectShader(self);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var ti = serializedObject.targetObject as BlurredImage;
        Debug.Assert(ti != null, "Translucent Image Editor serializedObject target is null");
        var oldSource   = ti.source;
        var oldMaterial = ti.material;

        base.OnInspectorGUI();
        if (usingIncorrectShader)
        {
            EditorGUILayout.HelpBox("Material should use shader UI/Translucent Image",
                                    MessageType.Error);
        }

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(source);
        if (source.objectReferenceValue == null)
        {
            var existingSources = Shims.FindObjectsOfType<TranslucentImageSource>();
            if (existingSources.Length > 0)
            {
                using (new EditorGUI.IndentLevelScope())
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel("From current Scene");
                    using (new EditorGUILayout.VerticalScope())
                        foreach (var s in existingSources)
                        {
                            if (GUILayout.Button(s.gameObject.name))
                                source.objectReferenceValue = s;
                        }
                }

                EditorGUILayout.Space();
            }
            else
            {
                EditorGUILayout.HelpBox("No Translucent Image Source(s) found in current scene", MessageType.Warning);
            }
        }

        if (materialUsedInDifferentSource)
        {
            EditorGUILayout.HelpBox("Translucent Images with different Sources" +
                                    " should also use different Materials",
                                    MessageType.Error);
        }

        EditorGUILayout.PropertyField(spriteBlending);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Shared settings", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.PropertyField(vibrancy);
        EditorGUILayout.PropertyField(brightness);
        EditorGUILayout.PropertyField(flatten);
        serializedObject.ApplyModifiedProperties();

        if (ti.source != oldSource)
            OnSourceChanged(ti);
        if (ti.material != oldMaterial)
            OnMaterialChanged(ti);
    }

    void OnSourceChanged(BlurredImage self)
    {
        CheckMaterialUsedInDifferentSource(self);
    }

    void OnMaterialChanged(BlurredImage self)
    {
        CheckMaterialUsedInDifferentSource(self);
        CheckCorrectShader(self);
    }

    private void CheckCorrectShader(BlurredImage self)
    {
        usingIncorrectShader = self.material.shader != correctShader;
    }

    private void CheckMaterialUsedInDifferentSource(BlurredImage self)
    {
        if (!self.source)
        {
            materialUsedInDifferentSource = false;
            return;
        }

        var diffSource = Shims.FindObjectsOfType<BlurredImage>()
                              .Where(ti => ti.source != self.source)
                              .ToList();

        if (!diffSource.Any())
        {
            materialUsedInDifferentSource = false;
            return;
        }

        var sameMat = diffSource.GroupBy(ti => ti.material).ToList();

        materialUsedInDifferentSource = sameMat.All(group => group.Key == self.material);
    }
}
}
