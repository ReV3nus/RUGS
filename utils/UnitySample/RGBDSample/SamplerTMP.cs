//using System.IO;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.Rendering.HighDefinition;
//using UnityEngine.Rendering;
//using static UnityEditor.PlayerSettings;
//using static UnityEngine.GraphicsBuffer;
//using System.Collections.Generic;

//[CustomEditor(typeof(RGBDSampler))]
//public class RGBDCameraCaptureEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        RGBDSampler captureScript = (RGBDSampler)target;

//        //if (GUILayout.Button("Init"))
//        //{
//        //    captureScript.InitCameraAndTexture();
//        //}
//        //if (GUILayout.Button("Test"))
//        //{
//        //    captureScript.Test();
//        //}
//        if (GUILayout.Button("Capture Current View"))
//        {
//            captureScript.CaptureCurrent();
//        }
//        if (GUILayout.Button("Manual Sample Scene"))
//        {
//            captureScript.ManualSampleScene();
//        }

//    }
//}



//[ExecuteAlways]
//public class RGBDSampler : MonoBehaviour
//{
//    private Camera sampleCamera;
//    private RenderTexture rgbTexture, depthTexture;
//    private bool depthOnly = false;

//    [Header("Sample File Settings")]
//    public string savePath = "SampleOutput";    //this should be the path under your project's folder
//    public Vector2Int resolution = new(1296, 864);

//    [Header("Camera Sample Params")]
//    public int sampleCount = 100;
//    public GameObject sampleCenter;
//    public float sampleDistance;                //max sample distance from center

//    public enum RPPL { BiRP, URP, HDRP };
//    [Header("Rendering Pipeline Settings")]
//    public RPPL rppl = RPPL.BiRP;
//    public Material BiRPDepthMaterial;
//    public GameObject HDRPDepthVolume;

//    private int SampleCnt = 0;

//    // This function only works in Built-in Render Pipeline
//    private void OnRenderImage(RenderTexture src, RenderTexture dest)
//    {
//        if (depthOnly)
//        {
//            Graphics.Blit(src, dest, BiRPDepthMaterial);
//        }
//        else
//        {
//            Graphics.Blit(src, dest);
//        }
//    }

//    public void InitCameraAndTexture()
//    {
//        if (!GetComponent<Camera>())
//            this.gameObject.AddComponent<Camera>();
//        sampleCamera = GetComponent<Camera>();

//        //sample camera must be physical camera to get correct params for training
//        sampleCamera.usePhysicalProperties = true;
//        sampleCamera.sensorSize = new Vector2(24f, 18f);
//        sampleCamera.focalLength = 20f;
//        sampleCamera.aperture = 22f;
//        sampleCamera.focusDistance = 10f;

//        sampleCamera.depthTextureMode = DepthTextureMode.Depth;
//        rgbTexture = new RenderTexture(resolution.x, resolution.y, 24);
//        depthTexture = new RenderTexture(resolution.x, resolution.y, 24);
//    }

//    private void OnEnable()
//    {
//        InitCameraAndTexture();
//    }
//    public void CaptureRGB(int numid)
//    {
//        //Create 24 bit Texture(8 for each RGB)
//        rgbTexture = new RenderTexture(resolution.x, resolution.y, 24);

//        if (rppl == RPPL.BiRP) depthOnly = false;
//        else if (rppl == RPPL.HDRP) HDRPDepthVolume.SetActive(false);
//        else
//        {
//            Debug.LogError("This script doesn't support for URP yet!");
//            return;
//        }

//        sampleCamera.targetTexture = rgbTexture;
//        sampleCamera.Render();

//        RenderTexture.active = rgbTexture;
//        Texture2D rgbTex = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false);
//        rgbTex.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);
//        rgbTex.Apply();

//        byte[] rgbBytes = rgbTex.EncodeToPNG();
//        string filename = string.Format("{0}/input/{1}.png", savePath, numid);
//        File.WriteAllBytes(filename, rgbBytes);

//        sampleCamera.targetTexture = null;
//        RenderTexture.active = null;
//    }
//    public void CaptureDepth(int numid)
//    {
//        depthTexture = new RenderTexture(resolution.x, resolution.y, 24);

//        if (rppl == RPPL.BiRP) depthOnly = true;
//        else if (rppl == RPPL.HDRP) HDRPDepthVolume.SetActive(true);


//        sampleCamera.targetTexture = depthTexture;
//        sampleCamera.Render();

//        RenderTexture.active = depthTexture;

//        Texture2D depthImage = new Texture2D(resolution.x, resolution.y, TextureFormat.R8, false);
//        depthImage.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);
//        depthImage.Apply();
//        byte[] bytes = depthImage.EncodeToPNG();
//        string depthFilename = string.Format("{0}/depth/{1}.png", savePath, numid);
//        File.WriteAllBytes(depthFilename, bytes);

//        sampleCamera.targetTexture = null;
//        RenderTexture.active = null;

//    }


//    private int[,,] coverageGrid;
//    private List<Vector3> sampledPositions;
//    private Vector3 gridMin;
//    private Vector3 gridMax;
//    public void ManualSampleScene()
//    {
//        InitCameraAndTexture();
//        string path = string.Format("{0}/input/", savePath);
//        Directory.CreateDirectory(path);
//        path = string.Format("{0}/depth/", savePath);
//        Directory.CreateDirectory(path);

//        //SampleCnt = 0;

//        //for (int total = 0; total < sampleCount; total++)
//        //{
//        //    float degree = UnityEngine.Random.Range(0f, 360f);
//        //    float pitch = UnityEngine.Random.Range(-60f, 60f);
//        //    float dist = UnityEngine.Random.Range(0.5f, sampleDistance);
//        //    SampleAt(sampleCenter.transform.position, pitch, degree, dist);
//        //}

//        coverageGrid = new int[20, 10, 20];
//        gridMax = sampleCenter.transform.position + new Vector3(sampleDistance, sampleDistance / 2f, sampleDistance);
//        gridMin = sampleCenter.transform.position - new Vector3(sampleDistance, sampleDistance / 2f, sampleDistance);
//        sampledPositions = new List<Vector3>();

//        SampleCnt = 0;
//        for (int total = 0; total < sampleCount; total++)
//        {
//            Vector3 samplePos = GetNewSamplePosition();
//            Quaternion sampleRot = Random.rotation;
//        }
//    }
//    Vector3 GetNewSamplePosition()
//    {
//        for (int tries = 0; tries < 100; tries++)
//        {
//            Vector3 randPos = new Vector3(
//                Random.Range(gridMin.x, gridMax.x),
//                Random.Range(gridMin.y, gridMax.y),
//                Random.Range(gridMin.z, gridMax.z)
//            );

//            if (!IsTooCloseToPrevious(randPos))
//            {
//                int3 cell = WorldToGridCell(randPos);
//                if (coverageGrid[cell.x, cell.y, cell.z] == GetMinCoverage())
//                    return randPos;
//            }
//        }
//        // fallback
//        return gridMin + Vector3.Scale(Random.insideUnitSphere, (gridMax - gridMin));
//    }
//    void UpdateCoverageGrid(Vector3 camPos)
//    {
//        int3 cell = WorldToGridCell(camPos);
//        coverageGrid[cell.x, cell.y, cell.z]++;
//        sampledPositions.Add(camPos);
//    }

//    bool IsTooCloseToPrevious(Vector3 pos)
//    {
//        foreach (var prev in sampledPositions)
//        {
//            if (Vector3.Distance(prev, pos) < minViewDistance)
//                return true;
//        }
//        return false;
//    }

//    int GetMinCoverage()
//    {
//        int min = int.MaxValue;
//        foreach (var val in coverageGrid)
//            min = Mathf.Min(min, val);
//        return min;
//    }

//    struct int3 { public int x, y, z; public int3(int a, int b, int c) { x = a; y = b; z = c; } }

//    int3 WorldToGridCell(Vector3 pos)
//    {
//        Vector3 rel = pos - gridMin;
//        Vector3 norm = new Vector3(
//            rel.x / (gridMax.x - gridMin.x),
//            rel.y / (gridMax.y - gridMin.y),
//            rel.z / (gridMax.z - gridMin.z)
//        );

//        return new int3(
//            Mathf.Clamp((int)(norm.x * gridResolution.x), 0, gridResolution.x - 1),
//            Mathf.Clamp((int)(norm.y * gridResolution.y), 0, gridResolution.y - 1),
//            Mathf.Clamp((int)(norm.z * gridResolution.z), 0, gridResolution.z - 1)
//        );
//    }



//    private void SampleAt(Vector3 center, float pitch, float degree, float dist)
//    {
//        Vector3 dir = new Vector3(0, 0, dist);
//        Quaternion rotation = Quaternion.Euler(pitch, degree, 0);
//        transform.position = center + rotation * dir;
//        transform.LookAt(center);

//        CaptureRGB(SampleCnt);
//        CaptureDepth(SampleCnt);
//        //CaptureMask(SampleCnt);
//        SampleCnt++;

//        if (rppl == RPPL.BiRP) depthOnly = false;
//        else if (rppl == RPPL.HDRP) HDRPDepthVolume.SetActive(false);
//    }

//    public void CaptureCurrent()
//    {
//        string path = string.Format("{0}/input/", "SampleOutput");
//        Directory.CreateDirectory(path);
//        path = string.Format("{0}/depth/", "SampleOutput");
//        Directory.CreateDirectory(path);

//        CaptureRGB(0);
//        CaptureDepth(0);

//        if (rppl == RPPL.BiRP) depthOnly = false;
//        else if (rppl == RPPL.HDRP) HDRPDepthVolume.SetActive(false);
//    }



//    public void Test()
//    {
//        Vector3 dir = new Vector3(0, 0, sampleDistance);
//        HDRPDepthVolume.GetComponent<CustomPassVolume>().customPasses[0].targetColorBuffer = CustomPass.TargetBuffer.Custom;
//    }
//}
