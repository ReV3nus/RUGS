using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace GaussianSplatting.Runtime
{
	[CustomEditor(typeof(GSEffects))]
	public class GSEffectsEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			GSEffects captureScript = (GSEffects)target;

			if (GUILayout.Button("Update"))
			{
				captureScript.ManualUpdate();
			}
		}
	}

	public class GSEffects : MonoBehaviour
	{
		public AnimationCurve colorCurve = AnimationCurve.Linear(0, 0, 1, 1);
		[HideInInspector] public Texture colorCurveTex;

		public AnimationCurve roughnessCurve = AnimationCurve.Linear(0, 0, 1, 1);
		[HideInInspector] public Texture roughnessCurveTex;

		public AnimationCurve metallicCurve = AnimationCurve.Linear(0, 0, 1, 1);
		[HideInInspector] public Texture metallicCurveTex;

		Texture2D GenerateCurveTexture(AnimationCurve curve)
		{
			Texture2D tex = new Texture2D(256, 1, TextureFormat.RFloat, false, true);
			tex.wrapMode = TextureWrapMode.Clamp;
			for (int i = 0; i < 256; i++)
			{
				float input = i / 255f;
				float value = Mathf.Clamp01(curve.Evaluate(input));
				tex.SetPixel(i, 0, new Color(value, 0, 0, 1));
			}
			tex.Apply();
			return tex;
		}

		public void ManualUpdate()
		{
			colorCurveTex = GenerateCurveTexture(colorCurve);
			roughnessCurveTex = GenerateCurveTexture(roughnessCurve);
			metallicCurveTex = GenerateCurveTexture(metallicCurve);
		}

        private void OnValidate()
        {
            ManualUpdate();
        }

		void OnEnable()
		{
			ManualUpdate();
		}
	}
}
