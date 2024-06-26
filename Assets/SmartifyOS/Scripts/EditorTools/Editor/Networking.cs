using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

public class Networking
{
    public static void POST(string url, WWWForm form, Action<bool, string> onComplete)
    {
        EditorApplication.delayCall += () =>
        {
            // Create a UnityWebRequest
            UnityWebRequest request = UnityWebRequest.Post(url, form);

            // Set headers if needed
            // request.SetRequestHeader("HeaderName", "HeaderValue");
            request.SetRequestHeader("User-Agent", "Unity Editor");
            request.SetRequestHeader("Origin", "http://localhost:5173");

            // Send the request asynchronously
            var operation = request.SendWebRequest();

            // Handle completion
            operation.completed += (op) =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    onComplete(true, request.downloadHandler.text);
                }
                else
                {
                    onComplete(false, request.error);
                }

                // Clean up the request
                request.Dispose();
            };
        };
    }

    public static void GET(string url, Action<bool, string> onComplete)
    {
        EditorApplication.delayCall += () =>
        {
            // Create a UnityWebRequest
            UnityWebRequest request = UnityWebRequest.Get(url);

            // Set headers if needed
            // request.SetRequestHeader("HeaderName", "HeaderValue");
            request.SetRequestHeader("User-Agent", "Unity Editor");
            request.SetRequestHeader("Origin", "http://localhost:5173");

            // Send the request asynchronously
            var operation = request.SendWebRequest();

            // Handle completion
            operation.completed += (op) =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    onComplete(true, request.downloadHandler.text);
                }
                else
                {
                    onComplete(false, request.error);
                }

                // Clean up the request
                request.Dispose();
            };
        };
    }

    public static void GET(string url, string queryParams, Action<bool, string> onComplete)
    {
        string fullUrl = $"{url}?{queryParams}";
        GET(fullUrl, onComplete);
    }
}
