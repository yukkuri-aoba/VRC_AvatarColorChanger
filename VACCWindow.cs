using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VRCAvatarColorChanger
{
    public class VACCWindow : EditorWindow
    {
        private Texture2D sourceTexture;
        private List<ColorZone> zones = new List<ColorZone>();
        private Texture2D previewTexture;
        private Vector2 scrollPos;
        private bool saveAsNewFile = true;
        private string newFileName = "";
        private float previewZoom = 1f;
        private bool previewDirty = true;

        // Exclusion mask (full resolution, true = excluded)
        private bool[] exclusionMask;
        private int maskWidth, maskHeight;
        private int brushSize = 8;
        private bool brushEraseMode; // false = paint exclusion, true = erase exclusion
        private bool isPainting;

        // Mask overlay
        private Texture2D maskOverlayTexture;
        private bool maskDirty = true;

        // Foldouts
        private bool zonesFoldout = true;
        private bool maskFoldout = true;
        private bool exportFoldout = true;

        // Preview generation
        private const int PreviewMaxSize = 512;

        [MenuItem("Tools/VRC AvatarColorChanger")]
        public static void ShowWindow()
        {
            var window = GetWindow<VACCWindow>(Localization.WindowTitle);
            window.minSize = new Vector2(340, 500);
        }

        private void OnGUI()
        {
            DrawHeader();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUI.BeginChangeCheck();

            DrawTextureField();
            DrawZoneList();
            DrawMaskSection();

            if (EditorGUI.EndChangeCheck())
            {
                previewDirty = true;
            }

            DrawPreview();
            DrawExportSection();

            EditorGUILayout.EndScrollView();
        }

        // ───────────────────────── Header ─────────────────────────

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            // Language selector
            var labels = new[] { Localization.LangAuto, Localization.LangJapanese, Localization.LangEnglish };
            int current = (int)Localization.CurrentLanguage;
            int next = GUILayout.Toolbar(current, labels, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
            if (next != current)
            {
                Localization.CurrentLanguage = (LanguageMode)next;
                Repaint();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(Localization.Credit, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                EditorUtility.DisplayDialog(Localization.CreditTitle, Localization.CreditBody, Localization.OK);
            }

            EditorGUILayout.EndHorizontal();
        }

        // ───────────────────────── Texture Field ─────────────────────────

        private void DrawTextureField()
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField(Localization.SourceTexture, EditorStyles.boldLabel);

            var newTex = (Texture2D)EditorGUILayout.ObjectField(
                Localization.Texture, sourceTexture, typeof(Texture2D), false);
            if (newTex != sourceTexture)
            {
                sourceTexture = newTex;
                previewDirty = true;
                exclusionMask = null;
                if (sourceTexture != null)
                {
                    var path = AssetDatabase.GetAssetPath(sourceTexture);
                    newFileName = Path.GetFileNameWithoutExtension(path) + "_recolored";
                }
            }

            if (sourceTexture != null && !IsReadable(sourceTexture))
            {
                EditorGUILayout.HelpBox(Localization.ReadWriteError, MessageType.Error);

                if (GUILayout.Button(Localization.EnableReadWrite))
                {
                    EnableReadWrite(sourceTexture);
                }
            }

            EditorGUILayout.Space(4);
        }

        // ───────────────────────── Zone List ─────────────────────────

        private void DrawZoneList()
        {
            zonesFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(zonesFoldout, Localization.ColorZones);
            if (!zonesFoldout)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            int removeIndex = -1;
            for (int i = 0; i < zones.Count; i++)
            {
                var zone = zones[i];
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // Header row
                EditorGUILayout.BeginHorizontal();
                zone.enabled = EditorGUILayout.ToggleLeft("", zone.enabled, GUILayout.Width(16));
                zone.name = EditorGUILayout.TextField(zone.name);
                if (GUILayout.Button("×", GUILayout.Width(22)))
                {
                    removeIndex = i;
                }
                EditorGUILayout.EndHorizontal();

                if (zone.enabled)
                {
                    zone.mode = (SelectionMode)EditorGUILayout.EnumPopup(
                        Localization.SelectionMode, zone.mode);

                    if (zone.mode == SelectionMode.ColorPick)
                    {
                        zone.sampleColor = EditorGUILayout.ColorField(
                            Localization.SampleColor, zone.sampleColor);
                        zone.tolerance = EditorGUILayout.Slider(
                            Localization.Tolerance, zone.tolerance, 0f, 1f);
                    }
                    else
                    {
                        EditorGUILayout.LabelField(Localization.UVRect);
                        EditorGUI.indentLevel++;
                        float x = EditorGUILayout.Slider("X", zone.uvRect.x, 0f, 1f);
                        float y = EditorGUILayout.Slider("Y", zone.uvRect.y, 0f, 1f);
                        float w = EditorGUILayout.Slider("W", zone.uvRect.width, 0f, 1f);
                        float h = EditorGUILayout.Slider("H", zone.uvRect.height, 0f, 1f);
                        zone.uvRect = new Rect(x, y, w, h);
                        EditorGUI.indentLevel--;
                    }

                    zone.targetColor = EditorGUILayout.ColorField(
                        Localization.TargetColor, zone.targetColor);
                    zone.valueBlend = EditorGUILayout.Slider(
                        new GUIContent(Localization.PatternPreserve, Localization.PatternPreserveTooltip),
                        zone.valueBlend, 0f, 1f);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }

            if (removeIndex >= 0)
            {
                zones.RemoveAt(removeIndex);
                previewDirty = true;
            }

            if (GUILayout.Button(Localization.AddZone))
            {
                zones.Add(new ColorZone());
                previewDirty = true;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(4);
        }

        // ───────────────────────── Exclusion Mask ─────────────────────────

        private void DrawMaskSection()
        {
            maskFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(maskFoldout, Localization.ExclusionMask);
            if (!maskFoldout)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            brushSize = EditorGUILayout.IntSlider(Localization.BrushSize, brushSize, 1, 64);

            EditorGUILayout.BeginHorizontal();
            bool newEraseMode = brushEraseMode;
            if (GUILayout.Toggle(!brushEraseMode, Localization.Exclude, EditorStyles.miniButton))
                newEraseMode = false;
            if (GUILayout.Toggle(brushEraseMode, Localization.Include, EditorStyles.miniButton))
                newEraseMode = true;
            brushEraseMode = newEraseMode;
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button(Localization.ClearMask))
            {
                exclusionMask = null;
                previewDirty = true;
            }

            EditorGUILayout.HelpBox(Localization.MaskHint, MessageType.Info);

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(4);
        }

        private void EnsureMask()
        {
            if (sourceTexture == null) return;
            if (exclusionMask == null || exclusionMask.Length != sourceTexture.width * sourceTexture.height)
            {
                maskWidth = sourceTexture.width;
                maskHeight = sourceTexture.height;
                exclusionMask = new bool[maskWidth * maskHeight];
            }
        }

        private void PaintMask(Vector2 uvPos)
        {
            EnsureMask();
            int cx = Mathf.RoundToInt(uvPos.x * maskWidth);
            int cy = Mathf.RoundToInt(uvPos.y * maskHeight);
            // Scale brush size from preview pixels to mask pixels
            float maskScale = maskWidth / (float)Mathf.Min(maskWidth, PreviewMaxSize);
            int r = Mathf.Max(1, Mathf.RoundToInt(brushSize * maskScale));

            bool value = !brushEraseMode; // true = excluded

            for (int dy = -r; dy <= r; dy++)
            {
                for (int dx = -r; dx <= r; dx++)
                {
                    if (dx * dx + dy * dy > r * r) continue;
                    int px = cx + dx;
                    int py = cy + dy;
                    if (px < 0 || px >= maskWidth || py < 0 || py >= maskHeight) continue;
                    exclusionMask[py * maskWidth + px] = value;
                }
            }

            maskDirty = true;
            previewDirty = true;
        }

        private bool IsExcluded(int x, int y, int texWidth, int texHeight)
        {
            if (exclusionMask == null) return false;
            int mx = Mathf.Clamp(x * maskWidth / texWidth, 0, maskWidth - 1);
            int my = Mathf.Clamp(y * maskHeight / texHeight, 0, maskHeight - 1);
            return exclusionMask[my * maskWidth + mx];
        }

        // ───────────────────────── Preview ─────────────────────────

        private void DrawPreview()
        {
            EditorGUILayout.LabelField(Localization.Preview, EditorStyles.boldLabel);

            if (sourceTexture == null)
            {
                EditorGUILayout.HelpBox(Localization.SetTexture, MessageType.Info);
                return;
            }

            if (!IsReadable(sourceTexture))
                return;

            if (previewDirty)
            {
                GeneratePreview();
                previewDirty = false;
            }

            if (previewTexture == null) return;

            previewZoom = EditorGUILayout.Slider(Localization.Zoom, previewZoom, 0.25f, 4f);

            float displayW = previewTexture.width * previewZoom;
            float displayH = previewTexture.height * previewZoom;

            // Clamp to available window width so the layout is never stretched
            float available = EditorGUIUtility.currentViewWidth - 20f;
            if (displayW > available)
            {
                float ratio = available / displayW;
                displayW = available;
                displayH *= ratio;
            }

            var previewRect = GUILayoutUtility.GetRect(
                displayW, displayH,
                GUILayout.Width(displayW), GUILayout.Height(displayH));
            EditorGUI.DrawPreviewTexture(previewRect, previewTexture);

            // Draw mask overlay
            if (maskOverlayTexture != null)
            {
                GUI.DrawTexture(previewRect, maskOverlayTexture, ScaleMode.StretchToFill, true);
            }

            // Handle brush painting on preview
            HandlePreviewInput(previewRect);

            EditorGUILayout.Space(4);
        }

        private void HandlePreviewInput(Rect previewRect)
        {
            Event e = Event.current;
            if (e == null) return;

            bool isInRect = previewRect.Contains(e.mousePosition);
            int controlId = GUIUtility.GetControlID(FocusType.Passive);

            switch (e.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                    if (e.button == 0 && isInRect)
                    {
                        GUIUtility.hotControl = controlId;
                        isPainting = true;
                        PaintAtScreenPos(e.mousePosition, previewRect);
                        e.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (isPainting && GUIUtility.hotControl == controlId)
                    {
                        PaintAtScreenPos(e.mousePosition, previewRect);
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (e.button == 0 && GUIUtility.hotControl == controlId)
                    {
                        GUIUtility.hotControl = 0;
                        isPainting = false;
                        e.Use();
                    }
                    break;

                case EventType.Repaint:
                    if (isInRect)
                    {
                        float brushPixels = brushSize * previewZoom;
                        Handles.color = brushEraseMode
                            ? new Color(0, 1, 0, 0.5f)
                            : new Color(1, 0, 0, 0.5f);
                        Handles.DrawSolidDisc(e.mousePosition, Vector3.forward, brushPixels * 0.5f);
                    }
                    break;
            }

            // Only repaint for cursor tracking, not full regeneration
            if (isInRect && e.type == EventType.MouseMove)
                Repaint();
        }

        private void PaintAtScreenPos(Vector2 screenPos, Rect previewRect)
        {
            float u = (screenPos.x - previewRect.x) / previewRect.width;
            float v = 1f - (screenPos.y - previewRect.y) / previewRect.height; // flip Y
            if (u < 0 || u > 1 || v < 0 || v > 1) return;
            PaintMask(new Vector2(u, v));
        }

        private void GeneratePreview()
        {
            if (sourceTexture == null || !IsReadable(sourceTexture)) return;

            int srcW = sourceTexture.width;
            int srcH = sourceTexture.height;

            // Compute preview size
            float scale = 1f;
            if (srcW > PreviewMaxSize || srcH > PreviewMaxSize)
            {
                scale = PreviewMaxSize / (float)Mathf.Max(srcW, srcH);
            }
            int prevW = Mathf.Max(1, Mathf.RoundToInt(srcW * scale));
            int prevH = Mathf.Max(1, Mathf.RoundToInt(srcH * scale));

            // Create scaled copy
            RenderTexture rt = RenderTexture.GetTemporary(prevW, prevH, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(sourceTexture, rt);
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;

            if (previewTexture == null || previewTexture.width != prevW || previewTexture.height != prevH)
            {
                if (previewTexture != null)
                    DestroyImmediate(previewTexture);
                previewTexture = new Texture2D(prevW, prevH, TextureFormat.RGBA32, false);
            }

            previewTexture.ReadPixels(new UnityEngine.Rect(0, 0, prevW, prevH), 0, 0);
            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);

            // Process pixels
            ProcessPixels(previewTexture);
            previewTexture.Apply();

            // Rebuild mask overlay if needed
            if (maskDirty || maskOverlayTexture == null ||
                maskOverlayTexture.width != prevW || maskOverlayTexture.height != prevH)
            {
                RebuildMaskOverlay(prevW, prevH);
                maskDirty = false;
            }
        }

        // ───────────────────────── Pixel Processing ─────────────────────────

        private void ProcessPixels(Texture2D tex)
        {
            if (zones.Count == 0) return;

            Color32[] pixels = tex.GetPixels32();
            int w = tex.width;
            int h = tex.height;

            for (int i = 0; i < pixels.Length; i++)
            {
                int x = i % w;
                int y = i / w;

                // Check exclusion mask
                if (IsExcluded(x, y, w, h)) continue;

                Color pixel = pixels[i];

                for (int z = zones.Count - 1; z >= 0; z--)
                {
                    var zone = zones[z];
                    if (zone.ContainsPixel(pixel, x, y, w, h))
                    {
                        pixels[i] = RecolorPixel(pixel, zone.targetColor, zone.valueBlend);
                        break;
                    }
                }
            }

            tex.SetPixels32(pixels);
        }

        private static Color32 RecolorPixel(Color original, Color target, float valueBlend)
        {
            float oH, oS, oV;
            Color.RGBToHSV(original, out oH, out oS, out oV);

            float tH, tS, tV;
            Color.RGBToHSV(target, out tH, out tS, out tV);

            float newV = Mathf.Lerp(tV, oV, valueBlend);

            Color result = Color.HSVToRGB(tH, tS, newV);
            result.a = original.a;
            return result;
        }

        // ───────────────────────── Export ─────────────────────────

        private void DrawExportSection()
        {
            exportFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(exportFoldout, Localization.Export);
            if (!exportFoldout)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            if (sourceTexture == null)
            {
                GUI.enabled = false;
            }

            saveAsNewFile = EditorGUILayout.Toggle(Localization.SaveAsNewFile, saveAsNewFile);
            if (saveAsNewFile)
            {
                newFileName = EditorGUILayout.TextField(Localization.FileName, newFileName);
            }

            if (GUILayout.Button(Localization.ApplyAndSave, GUILayout.Height(32)))
            {
                ApplyRecolor();
            }

            GUI.enabled = true;
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void ApplyRecolor()
        {
            if (sourceTexture == null || !IsReadable(sourceTexture))
            {
                EditorUtility.DisplayDialog(Localization.Error, Localization.TextureReadError, Localization.OK);
                return;
            }

            string srcPath = AssetDatabase.GetAssetPath(sourceTexture);
            if (string.IsNullOrEmpty(srcPath))
            {
                EditorUtility.DisplayDialog(Localization.Error, Localization.PathNotFound, Localization.OK);
                return;
            }

            // Full-resolution copy
            var fullTex = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, false);
            fullTex.SetPixels32(sourceTexture.GetPixels32());

            ProcessPixels(fullTex);
            fullTex.Apply();

            byte[] pngData = fullTex.EncodeToPNG();
            DestroyImmediate(fullTex);

            string outputPath;
            if (saveAsNewFile)
            {
                string dir = Path.GetDirectoryName(srcPath);
                string safeName = string.IsNullOrWhiteSpace(newFileName) ? "recolored" : newFileName;
                // Security: prevent path traversal by taking only the filename portion
                safeName = Path.GetFileName(safeName);
                // Strip characters that are invalid in file names
                foreach (char c in Path.GetInvalidFileNameChars())
                    safeName = safeName.Replace(c.ToString(), "_");
                if (string.IsNullOrWhiteSpace(safeName)) safeName = "recolored";
                outputPath = Path.Combine(dir, safeName + ".png");

                if (File.Exists(outputPath))
                {
                    if (!EditorUtility.DisplayDialog(Localization.Confirm,
                        Localization.FileExistsConfirm(outputPath), Localization.Overwrite, Localization.Cancel))
                    {
                        return;
                    }
                }
            }
            else
            {
                outputPath = Path.ChangeExtension(srcPath, ".png");
                if (!EditorUtility.DisplayDialog(Localization.Confirm,
                    Localization.OverwriteConfirm, Localization.Overwrite, Localization.Cancel))
                {
                    return;
                }
            }

            File.WriteAllBytes(outputPath, pngData);
            AssetDatabase.Refresh();

            Debug.Log($"[VACC] Saved: {outputPath}");
            EditorUtility.DisplayDialog(Localization.Complete, Localization.Saved(outputPath), Localization.OK);
        }

        // ───────────────────────── Utility ─────────────────────────

        private static bool IsReadable(Texture2D tex)
        {
            try
            {
                tex.GetPixel(0, 0);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void EnableReadWrite(Texture2D tex)
        {
            string path = AssetDatabase.GetAssetPath(tex);
            if (string.IsNullOrEmpty(path)) return;

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;

            importer.isReadable = true;
            importer.SaveAndReimport();
        }

        private void RebuildMaskOverlay(int width, int height)
        {
            if (exclusionMask == null)
            {
                if (maskOverlayTexture != null)
                {
                    DestroyImmediate(maskOverlayTexture);
                    maskOverlayTexture = null;
                }
                return;
            }

            if (maskOverlayTexture == null || maskOverlayTexture.width != width || maskOverlayTexture.height != height)
            {
                if (maskOverlayTexture != null)
                    DestroyImmediate(maskOverlayTexture);
                maskOverlayTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                maskOverlayTexture.filterMode = FilterMode.Point;
            }

            var overlayPixels = new Color32[width * height];
            var excluded = new Color32(255, 60, 60, 80);  // semi-transparent red
            var clear = new Color32(0, 0, 0, 0);

            for (int i = 0; i < overlayPixels.Length; i++)
            {
                int x = i % width;
                int y = i / width;
                overlayPixels[i] = IsExcluded(x, y, width, height) ? excluded : clear;
            }

            maskOverlayTexture.SetPixels32(overlayPixels);
            maskOverlayTexture.Apply();
        }

        private void OnDestroy()
        {
            if (previewTexture != null)
                DestroyImmediate(previewTexture);
            if (maskOverlayTexture != null)
                DestroyImmediate(maskOverlayTexture);
        }
    }
}
