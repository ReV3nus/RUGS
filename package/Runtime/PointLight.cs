using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;


namespace GaussianSplatting.Runtime
{
    [CustomEditor(typeof(PointLight))]
    public class PointLightEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PointLight captureScript = (PointLight)target;

            if (GUILayout.Button("Init"))
            {
                captureScript.Init();
            }
            if (GUILayout.Button("Capture"))
            {
                captureScript.CaptureShadowCubemap();
            }
        }
    }


    [ExecuteAlways]
    public class PointLight : MonoBehaviour
    {
        [Header("Components")]
        [HideInInspector]
        public PointLightData data;

        [HideInInspector]
        public Light lightSrc;
        [HideInInspector]
        public Camera shadowCamera;
        [HideInInspector]
        public RenderTexture shadowCubemap;
        [HideInInspector]
        public CubemapFace currentFace;

        [HideInInspector]
        public Material debugMat;

        public bool LightSwitch = true;

        public struct PointLightData
        {
            public Vector3 pos;
            public float intensity;
            public Vector3 color;
            public float range;
        }
        private Vector3 lastPos = new Vector3(-114f, 514f, 1919f);
        
        private void UpdataPLD()
        {
            data.pos = this.transform.position;
            data.intensity = lightSrc.intensity;
            data.color = new Vector3(lightSrc.color.r, lightSrc.color.g, lightSrc.color.b);
            data.range = lightSrc.range;
        }

        void InitCubemap()
        {
            if (shadowCubemap != null)
            {
                shadowCubemap.Release();
            }
            shadowCubemap = new RenderTexture(512, 512, 24);
            shadowCubemap.name = "Shadow Cubemap";
            shadowCubemap.dimension = UnityEngine.Rendering.TextureDimension.Cube;
            shadowCubemap.volumeDepth = 6;
            shadowCubemap.format = RenderTextureFormat.ARGBHalf;
            shadowCubemap.useMipMap = false;
            shadowCubemap.autoGenerateMips = false;
            shadowCubemap.Create();

            debugMat.SetTexture("_CubeMap", shadowCubemap);
        }
        public void Init()
        {
            if (!GetComponent<Light>())
                this.gameObject.AddComponent<Light>();
            lightSrc = GetComponent<Light>();
            UpdataPLD();


            if (!GetComponent<Camera>())
                this.gameObject.AddComponent<Camera>();
            shadowCamera = GetComponent<Camera>();
            shadowCamera.fieldOfView = 90f; 
            shadowCamera.aspect = 1f;       
            shadowCamera.nearClipPlane = 0.01f;
            shadowCamera.farClipPlane = 1000f;

            InitCubemap();
        }

        private void OnValidate()
        {
            UpdataPLD();
        }
        private void OnEnable()
        {
            Init();
            UpdataPLD();
        }
        private void Update()
        {
            if (!LightSwitch && this.shadowCamera.enabled)
            {
                this.shadowCamera.enabled = false;
                lastPos = new Vector3(-114f, 514f, 1919f);
                InitCubemap();
            }
            else if (LightSwitch && !this.shadowCamera.enabled)
            {
                this.shadowCamera.enabled = true;
                lastPos = new Vector3(-114f, 514f, 1919f);
            }

            if (!LightSwitch)
                return;

            if (transform.position != lastPos)
            {
                CaptureShadowCubemap();
                lastPos = transform.position;
            }
            UpdataPLD();
        }

        Quaternion GetCubemapFaceRotation(CubemapFace face)
        {
            switch (face)
            {
                case CubemapFace.PositiveX: return Quaternion.LookRotation(Vector3.right, Vector3.down);
                case CubemapFace.NegativeX: return Quaternion.LookRotation(Vector3.left, Vector3.down);
                case CubemapFace.PositiveY: return Quaternion.LookRotation(Vector3.up, Vector3.forward);
                case CubemapFace.NegativeY: return Quaternion.LookRotation(Vector3.down, Vector3.back);
                case CubemapFace.PositiveZ: return Quaternion.LookRotation(Vector3.forward, Vector3.down);
                case CubemapFace.NegativeZ: return Quaternion.LookRotation(Vector3.back, Vector3.down);
                default: return Quaternion.identity;
            }
        }
        public void CaptureShadowCubemap()
        {
            for (int face = 0; face < 6; face++)
            {
                currentFace = (CubemapFace)face;

                shadowCamera.transform.rotation = GetCubemapFaceRotation((CubemapFace)face);
                shadowCamera.Render();
            }
        }
        public PointLightData GetData()
        {
            return data;
        }
    }
}