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
        /// <summary>Was the window open before it was closed by another window</summary>
        protected bool wasOpen;
        /// <summary>Is the window currently open</summary>
        protected bool isOpen;

        protected void Init()
        {
            UIManager.Instance.RegisterUIWindow(this);

            BaseUIManager.OnWindowOpened += UIManager_OnWindowOpened;
            BaseUIManager.OnWindowClosed += UIManager_OnWindowClosed;

            transform.localScale = Vector3.zero;
        }

        private void UIManager_OnWindowClosed(BaseUIWindow obj)
        {
            if (obj == this)
                return;

            HandleWindowClosed(obj);
        }

        private void UIManager_OnWindowOpened(BaseUIWindow obj)
        {
            if (obj == this)
                return;

            HandleWindowOpened(obj);
        }

        /// <summary>
        /// Called when any other window is opened
        /// </summary>
        /// <param name="window">The window that was opened</param>
        protected virtual void HandleWindowOpened(BaseUIWindow window) { }


        /// <summary>
        /// Called when any other window is closed
        /// </summary>
        /// <param name="window">The window that was closed</param>
        protected virtual void HandleWindowClosed(BaseUIWindow window) { }

        /// <summary>
        /// Called when the window is shown (opened)
        /// </summary>
        protected virtual void OnShow() { }

        /// <summary>
        /// Called when the window is hidden (closed)
        /// </summary>
        protected virtual void OnHide() { }


        /// <summary>
        /// Shows the window
        /// </summary>
        public void Show()
        {
            if (!Application.isPlaying)
            {
                transform.localScale = Vector3.one;
                return;
            }

            UIManager.Instance.AddOpenWindow(this);

            wasOpen = true;
            isOpen = true;

            //transform.localScale = Vector3.one;
            LeanTween.scale(gameObject, Vector3.one, 0.2f).setEaseInOutCubic();

            OnShow();
        }

        /// <summary>
        /// Hides the window
        /// </summary>
        /// <param name="internalUpdate"><see cref="wasOpen"/> will not be reset if <see langword="true"/></param>
        public void Hide(bool internalUpdate = false)
        {
            if (!Application.isPlaying)
            {
                transform.localScale = Vector3.zero;
                return;
            }

            UIManager.Instance.RemoveOpenWindow(this);

            if (!internalUpdate)
                wasOpen = false;

            isOpen = false;

            //transform.localScale = Vector3.zero;
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


