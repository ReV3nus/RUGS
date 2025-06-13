using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
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
        if (GUILayout.Button("Manual Sample Scene"))
        {
            captureScript.ManualSampleScene();
        }

    }
}



[ExecuteAlways]
public class RGBDSampler : MonoBehaviour
{
    private Camera sampleCamera;
    private RenderTexture rgbTexture, depthTexture;
    private bool depthOnly = false;

    [Header("Sample File Settings")]
    public string savePath = "SampleOutput";    //this should be the path under your project's folder
    public Vector2Int resolution = new(1296, 864);

    [Header("Camera Sample Params")]
    public int sampleCount = 100;
    public GameObject sampleCenter;
    public float sampleDistance;                //max sample distance from center

    public enum RPPL {BiRP, URP, HDRP};
    [Header("Rendering Pipeline Settings")]
    public RPPL rppl = RPPL.BiRP;
    public Material BiRPDepthMaterial;
    public GameObject HDRPDepthVolume;

    private int SampleCnt = 0;

    // This function only works in Built-in Render Pipeline
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (depthOnly)
        {
            Graphics.Blit(src, dest, BiRPDepthMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    public void InitCameraAndTexture()
    {
        if (!GetComponent<Camera>())
            this.gameObject.AddComponent<Camera>();
        sampleCamera = GetComponent<Camera>();

        //sample camera must be physical camera to get correct params for training
        sampleCamera.usePhysicalProperties = true;
        sampleCamera.sensorSize = new Vector2(24f, 18f);
        sampleCamera.focalLength = 20f;
        sampleCamera.aperture = 22f;
        sampleCamera.focusDistance = 10f;

        sampleCamera.depthTextureMode = DepthTextureMode.Depth;
        rgbTexture = new RenderTexture(resolution.x, resolution.y, 24);
        depthTexture = new RenderTexture(resolution.x, resolution.y, 24);
    }

    private void OnEnable()
    {
        InitCameraAndTexture();
    }
    public void CaptureRGB(int numid)
    {
        //Create 24 bit Texture(8 for each RGB)
        rgbTexture = new RenderTexture(resolution.x, resolution.y, 24);

        if (rppl == RPPL.BiRP) depthOnly = false;
        else if (rppl == RPPL.HDRP) HDRPDepthVolume.SetActive(false);
        else
        {
            Debug.LogError("This script doesn't support for URP yet!");
            return;
        }

        sampleCamera.targetTexture = rgbTexture;
        sampleCamera.Render();

        RenderTexture.active = rgbTexture;
        Texture2D rgbTex = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false);
        rgbTex.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);
        rgbTex.Apply();

        byte[] rgbBytes = rgbTex.EncodeToPNG();
        string filename = string.Format("{0}/input/{1}.png", savePath, numid);
        File.WriteAllBytes(filename, rgbBytes);

        sampleCamera.targetTexture = null;
        RenderTexture.active = null;
    }
    public void CaptureDepth(int numid)
    {
        depthTexture = new RenderTexture(resolution.x, resolution.y, 24);

        if (rppl == RPPL.BiRP) depthOnly = true;
        else if (rppl == RPPL.HDRP) HDRPDepthVolume.SetActive(true);


        sampleCamera.targetTexture = depthTexture;
        sampleCamera.Render();

        RenderTexture.active = depthTexture;

        Texture2D depthImage = new Texture2D(resolution.x, resolution.y, TextureFormat.R8, false);
        depthImage.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);
        depthImage.Apply();
        byte[] bytes = depthImage.EncodeToPNG();
        string depthFilename = string.Format("{0}/depth/{1}.png", savePath, numid);
        File.WriteAllBytes(depthFilename, bytes);

        sampleCamera.targetTexture = null;
        RenderTexture.active = null;

    }


    public void ManualSampleScene()
    {
        InitCameraAndTexture();
        string path = string.Format("{0}/input/", savePath);
        Directory.CreateDirectory(path);
        path = string.Format("{0}/depth/", savePath);
        Directory.CreateDirectory(path);

        SampleCnt = 0;

        for (int total = 0; total < sampleCount; total++)
        {
            float degree = UnityEngine.Random.Range(0f, 360f);
            float tmp = UnityEngine.Random.Range(0f, 1f);
            if (tmp <= 0.5f)
                tmp = Mathf.Sqrt(2f * tmp) / 2f;
            else
                tmp = 1f - Mathf.Sqrt(2f * (1f - tmp)) / 2f;
            float pitch = -90f + 180f * tmp;
            float dist = UnityEngine.Random.Range(0.5f, sampleDistance);
            SampleAt(sampleCenter.transform.position, pitch, degree, dist);
        }

    }
    

    private void SampleAt(Vector3 center, float pitch, float degree, float dist)
    {
        Vector3 dir = new Vector3(0, 0, dist);
        Quaternion rotation = Quaternion.Euler(pitch, degree, 0);
        transform.position = center + rotation * dir;
        transform.LookAt(center);

        CaptureRGB(SampleCnt);
        CaptureDepth(SampleCnt);
        //CaptureMask(SampleCnt);
        SampleCnt++;

        if (rppl == RPPL.BiRP) depthOnly = false;
        else if (rppl == RPPL.HDRP) HDRPDepthVolume.SetActive(false);
    }

    public void CaptureCurrent()
    {
        string path = string.Format("{0}/input/", "SampleOutput");
        Directory.CreateDirectory(path);
        path = string.Format("{0}/depth/", "SampleOutput");
        Directory.CreateDirectory(path);

        CaptureRGB(0);
        CaptureDepth(0);

        if (rppl == RPPL.BiRP) depthOnly = false;
        else if (rppl == RPPL.HDRP) HDRPDepthVolume.SetActive(false);
    }



    public void Test()
    {
        Vector3 dir = new Vector3(0, 0, sampleDistance);
        HDRPDepthVolume.GetComponent<CustomPassVolume>().customPasses[0].targetColorBuffer = CustomPass.TargetBuffer.Custom;
    }
}
