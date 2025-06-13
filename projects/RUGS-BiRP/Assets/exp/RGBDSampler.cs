using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;

[CustomEditor(typeof(RGBDSampler))]
public class RGBDCameraCaptureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RGBDSampler captureScript = (RGBDSampler)target;

        //if (GUILayout.Button("Init"))
        //{
        //    captureScript.InitCameraAndTexture();
        //}
        //if (GUILayout.Button("Test"))
        //{
        //    captureScript.Test();
        //}
        if (GUILayout.Button("Capture Current View"))
        {
            captureScript.CaptureCurrent();
        }

    }
}



[ExecuteAlways]
public class RGBDSampler : MonoBehaviour
{
    private Camera sampleCamera;
    private RenderTexture rgbTexture;

    [Header("Sample File Settings")]
    public string savePath = "SampleOutput";    //this should be the path under your project's folder
    public Vector2Int resolution = new(1500, 1000);

    public int currentId = 0;

    public void InitCameraAndTexture()
    {
        if (!GetComponent<Camera>())
            this.gameObject.AddComponent<Camera>();
        sampleCamera = GetComponent<Camera>();
        rgbTexture = new RenderTexture(resolution.x, resolution.y, 24);
    }

    private void OnEnable()
    {
        InitCameraAndTexture();
    }
    public void CaptureRGB()
    {
        //Create 24 bit Texture(8 for each RGB)
        rgbTexture = new RenderTexture(resolution.x, resolution.y, 24);

        sampleCamera.targetTexture = rgbTexture;
        sampleCamera.Render();

        RenderTexture.active = rgbTexture;
        Texture2D rgbTex = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false);
        rgbTex.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);
        rgbTex.Apply();

        byte[] rgbBytes = rgbTex.EncodeToPNG();
        string filename = string.Format("ExpRes/{0}.png", currentId);
        File.WriteAllBytes(filename, rgbBytes);

        currentId++;

        sampleCamera.targetTexture = null;
        RenderTexture.active = null;
    }

    public void CaptureCurrent()
    {
        CaptureRGB();
    }
}
