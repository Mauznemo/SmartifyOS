#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.UI.TranslucentImage
{
    [ExecuteAlways]
    [AddComponentMenu("UI/Blurred Image")]
    public partial class BlurredImage
    {
        protected override void Reset()
        {
            base.Reset();
            color = Color.white;

            material = FindDefaultMaterial();
            vibrancy = material.GetFloat(_vibrancyPropId);
            brightness = material.GetFloat(_brightnessPropId);
            flatten = material.GetFloat(_flattenPropId);
            spriteBlending = 0.085f;

            source = source ? source : Shims.FindObjectOfType<TranslucentImageSource>();
        }

        static Material FindDefaultMaterial()
        {
            var guid = AssetDatabase.FindAssets("Default-Translucent t:Material l:TranslucentImageResource");

            if (guid.Length == 0)
                Debug.LogError("Can't find Default-Translucent Material");

            var path = AssetDatabase.GUIDToAssetPath(guid[0]);

            return AssetDatabase.LoadAssetAtPath<Material>(path);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            SetVerticesDirty();

            Update();
        }
    }
}
#endif
