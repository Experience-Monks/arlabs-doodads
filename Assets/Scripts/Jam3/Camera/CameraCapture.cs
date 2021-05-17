//-----------------------------------------------------------------------
// <copyright file="CameraCapture.cs" company="Jam3 Inc">
//
// Copyright 2021 Jam3 Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jam3.Render.Capture
{
    /// <summary>
    /// Texture resolutions.
    /// </summary>
    public enum TextureResolutions
    {
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096
    }

    /// <summary>
    /// Camera capture.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    public class CameraCapture : MonoBehaviour
    {
        [Header("Texture Capture")]
        public Camera CameraObject = null;
        public TextureResolutions ImageSize = TextureResolutions._2048;

        // Runtime varilables
        private string saveFolder = "";
        private string currentName = "capture";

        /// <summary>
        /// Start.
        /// </summary>
        private void Start()
        {
            CreateNewFolderForScreenshots();
        }

        /// <summary>
        /// Creates new folder for screenshots.
        /// </summary>
        private void CreateNewFolderForScreenshots()
        {
            string folderName = Application.dataPath + "/../Captures";

            if (!System.IO.Directory.Exists(folderName))
                System.IO.Directory.CreateDirectory(folderName);

            saveFolder = folderName;
        }

        /// <summary>
        /// Saves texture.
        /// </summary>
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

