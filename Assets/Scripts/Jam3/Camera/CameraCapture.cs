using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jam3.Render.Capture
{
    public enum TextureResolutions
    {
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096
    }

    public class CameraCapture : MonoBehaviour
    {
        [Header("Texture Capture")]
        public Camera CameraObject = null;
        public TextureResolutions ImageSize = TextureResolutions._2048;

        private string saveFolder = "";
        private string currentName = "capture";

        void Start()
        {
            CreateNewFolderForScreenshots();
        }

        void CreateNewFolderForScreenshots()
        {
            string folderName = Application.dataPath + "/../Captures";

            if (!System.IO.Directory.Exists(folderName))
                System.IO.Directory.CreateDirectory(folderName);

            saveFolder = folderName;
        }

        public void SaveTexture()
        {
            int imagSize = (int)ImageSize;
            Texture2D tex = new Texture2D(imagSize, imagSize, TextureFormat.ARGB32, false);
            Texture2D texAlpha = new Texture2D(imagSize, imagSize, TextureFormat.ARGB32, false);
            Texture2D texFinal = new Texture2D(imagSize, imagSize, TextureFormat.ARGB32, false);

            if (CameraObject != null)
            {
                RenderTexture oldRT = CameraObject.targetTexture;

                RenderTexture renderTextureAlpha = new RenderTexture(imagSize, imagSize, 24, RenderTextureFormat.ARGBFloat);
                renderTextureAlpha.antiAliasing = 8;

                CameraObject.targetTexture = renderTextureAlpha;
                CameraObject.Render();
                RenderTexture.active = renderTextureAlpha;
                texAlpha.ReadPixels(new Rect(0, 0, renderTextureAlpha.width, renderTextureAlpha.height), 0, 0);
                texAlpha.Apply();

                RenderTexture renderTexture = new RenderTexture(imagSize, imagSize, 24);
                renderTexture.antiAliasing = 8;

                CameraObject.targetTexture = renderTexture;
                CameraObject.Render();

                RenderTexture.active = renderTexture;
                tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                tex.Apply();

                CameraObject.targetTexture = oldRT;
                RenderTexture.active = oldRT;

                DestroyImmediate(renderTexture);
                DestroyImmediate(renderTextureAlpha);

                for (int y = 0; y < texAlpha.height; ++y)
                {
                    for (int x = 0; x < texAlpha.width; ++x)
                    {
                        float alpha = texAlpha.GetPixel(x, y).a;

                        Color color;
                        if (alpha == 0)
                            color = Color.clear;
                        else
                            color = tex.GetPixel(x, y) * alpha;

                        color.a = alpha;

                        texFinal.SetPixel(x, y, color);
                    }
                }
            }

            byte[] bytes = texFinal.EncodeToPNG();
            System.IO.File.WriteAllBytes(saveFolder + "/" + currentName + ".png", bytes);

            DestroyImmediate(texFinal);
            DestroyImmediate(texAlpha);
            DestroyImmediate(tex);
        }
    }

#if UNITY_EDITOR
        [CustomEditor(typeof(CameraCapture))]
        public class CameraCaptureEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (!Application.isPlaying) return;

                CameraCapture t = target as CameraCapture;

                if (GUILayout.Button("Capture"))
                {
                    t.SaveTexture();
                };
            }
        }
#endif
}

