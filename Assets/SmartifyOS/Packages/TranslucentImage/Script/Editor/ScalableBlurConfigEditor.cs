using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace SmartifyOS.UI.TranslucentImage.Editor
{
[CustomEditor(typeof(ScalableBlurConfig))]
[CanEditMultipleObjects]
public class ScalableBlurConfigEditor : UnityEditor.Editor
{
    readonly AnimBool useAdvancedControl = new AnimBool(false);

    int tab, previousTab;

    EditorProperty radius;
    EditorProperty iteration;
    EditorProperty strength;

    public void Awake()
    {
        LoadTabSelection();
        useAdvancedControl.value = tab > 0;
    }

    public void OnEnable()
    {
        radius    = new EditorProperty(serializedObject, nameof(ScalableBlurConfig.Radius));
        iteration = new EditorProperty(serializedObject, nameof(ScalableBlurConfig.Iteration));
        strength  = new EditorProperty(serializedObject, nameof(ScalableBlurConfig.Strength));

        // Without this editor will not Repaint automatically when animating
        useAdvancedControl.valueChanged.AddListener(Repaint);
    }

    public override void OnInspectorGUI()
    {
        Draw();
    }

    public void Draw()
    {
        using (new EditorGUILayout.VerticalScope())
        {
            DrawTabBar();

            using (var changes = new EditorGUI.ChangeCheckScope())
            {
                serializedObject.Update();
                DrawTabsContent();
                if (changes.changed) serializedObject.ApplyModifiedProperties();
            }
        }
    }

    void DrawTabBar()
    {
        using (var h = new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();

            tab = GUILayout.Toolbar(
                tab,
                new[] { "Simple", "Advanced" },
                GUILayout.MinWidth(0),
                GUILayout.MaxWidth(EditorGUIUtility.pixelsPerPoint * 192)
            );

            GUILayout.FlexibleSpace();
        }

        if (tab != previousTab)
        {
            GUI.FocusControl(""); // Defocus
            SaveTabSelection();
            previousTab = tab;
        }

        useAdvancedControl.target = tab == 1;
    }

    void DrawTabsContent()
    {
        if (EditorGUILayout.BeginFadeGroup(1 - useAdvancedControl.faded))
        {
            // EditorProperty dooesn't invoke getter. Not needed anywhere else.
            _ = ((ScalableBlurConfig)target).Strength;
            strength.Draw();
        }
        EditorGUILayout.EndFadeGroup();

        if (EditorGUILayout.BeginFadeGroup(useAdvancedControl.faded))
        {
            radius.Draw();
            iteration.Draw();
        }
        EditorGUILayout.EndFadeGroup();
    }

    //Persist selected tab between sessions and instances
    void SaveTabSelection()
    {
        EditorPrefs.SetInt("LETAI_TRANSLUCENTIMAGE_TIS_TAB", tab);
    }

    void LoadTabSelection()
    {
        if (EditorPrefs.HasKey("LETAI_TRANSLUCENTIMAGE_TIS_TAB"))
            tab = EditorPrefs.GetInt("LETAI_TRANSLUCENTIMAGE_TIS_TAB");
    }
}
}
