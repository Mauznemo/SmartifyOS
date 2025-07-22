using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

namespace SmartifyOS.UI
{
    public abstract class BaseUIWindow : MonoBehaviour
    {
        [Tooltip("Should the window hide the bottom UI while its open?")]
        [SerializeField] public bool hidesBottomUI = false;
        /// <summary>Is the window currently shown/visible on screen</summary>
        protected bool isVisible;

        protected void Init()
        {
            UIManager.Instance.RegisterWindowInstance(this);

            transform.localScale = Vector3.zero;
        }

        /// <summary>
        /// Called when the window is shown
        /// </summary>
        protected virtual void OnShow() { }

        /// <summary>
        /// Called when the window is hidden
        /// </summary>
        protected virtual void OnHide() { }


        /// <summary>
        /// Shows the window on top
        /// </summary>
        public void Show(ShowAction showAction = ShowAction.OpenOnTop)
        {

            if (!Application.isPlaying)
            {
                transform.localScale = Vector3.one;
                return;
            }

            if (isVisible)
                return;

            UIManager.Instance.RegisterShownWindow(this);

            isVisible = true;

            LeanTween.scale(gameObject, Vector3.one, 0.2f).setEaseInOutCubic();

            if (showAction == ShowAction.OpenOnTop || showAction == ShowAction.OpenSingle)
                transform.SetSiblingIndex(UIManager.Instance.GetTopLevelSiblingIndex());
            else if (showAction == ShowAction.OpenInBackground)
                transform.SetSiblingIndex(0);

            OnShow();
        }

        /// <summary>
        /// Hides the window
        /// </summary>
        public void Hide()
        {
            if (!Application.isPlaying)
            {
                transform.localScale = Vector3.zero;
                return;
            }

            if (!isVisible)
                return;

            UIManager.Instance.RegisterHiddenWindow(this);

            isVisible = false;

            LeanTween.scale(gameObject, Vector3.zero, 0.2f).setEaseInOutCubic();

            OnHide();
        }

#if UNITY_EDITOR
        [ContextMenu("Remove Window and Delete Script File")]
        public void RemoveAndDeleteFile()
        {
            bool deleteConfirm = EditorUtility.DisplayDialog("Delete Window", $"Are you sure you want delete the window ({name}) and its script file permanently?", "Yes", "No");

            if (!deleteConfirm) return;

            // Get the MonoScript associated with this component
            MonoScript monoScript = MonoScript.FromMonoBehaviour(this);

            // Get the script's file path
            string scriptPath = AssetDatabase.GetAssetPath(monoScript);

            // Remove the component from the GameObject
            DestroyImmediate(this.gameObject);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            // Delete the script file from the project
            if (!string.IsNullOrEmpty(scriptPath) && File.Exists(scriptPath))
            {
                AssetDatabase.DeleteAsset(scriptPath);
                Debug.Log($"Script file deleted: {scriptPath}");
            }
            else
            {
                Debug.LogError("Script file could not be found or deleted.");
            }
        }
#endif
    }
}


