namespace VRCAvatarColorChanger
{
    public enum LanguageMode { Auto, Japanese, English }

    public static class Localization
    {
        public static LanguageMode CurrentLanguage = LanguageMode.Auto;

        public static bool IsJapanese
        {
            get
            {
                switch (CurrentLanguage)
                {
                    case LanguageMode.Japanese: return true;
                    case LanguageMode.English:  return false;
                    default: // Auto
                        return System.Globalization.CultureInfo.CurrentUICulture
                            .TwoLetterISOLanguageName == "ja";
                }
            }
        }

        // ─── Window ───
        public static string WindowTitle => "VRC AvatarColorChanger";

        // ─── Header / Language ───
        public static string LangAuto     => IsJapanese ? "自動" : "Auto";
        public static string LangJapanese => "日本語";
        public static string LangEnglish  => "English";
        public static string Credit       => IsJapanese ? "クレジット" : "Credit";

        // ─── Credit Dialog ───
        public static string CreditTitle => IsJapanese ? "クレジット" : "Credits";
        public static string CreditBody  => IsJapanese
            ? "VRC AvatarColorChanger (VACC)\n\n制作: yukkuri__aoba\nAI補助: Claude (Anthropic)"
            : "VRC AvatarColorChanger (VACC)\n\nDeveloper: yukkuri__aoba\nAI Assistance: Claude (Anthropic)";

        // ─── Source Texture ───
        public static string SourceTexture => IsJapanese ? "元テクスチャ" : "Source Texture";
        public static string Texture => IsJapanese ? "テクスチャ" : "Texture";
        public static string ReadWriteError => IsJapanese
            ? "Read/Write Enabled がオフです。\nインポート設定で有効にしてください。"
            : "Read/Write Enabled is off.\nPlease enable it in the import settings.";
        public static string EnableReadWrite => IsJapanese
            ? "Read/Write を自動で有効にする"
            : "Enable Read/Write automatically";

        // ─── Color Zones ───
        public static string ColorZones => IsJapanese ? "カラーゾーン" : "Color Zones";
        public static string AddZone => IsJapanese ? "+ ゾーン追加" : "+ Add Zone";
        public static string SelectionMode => IsJapanese ? "選択モード" : "Selection Mode";
        public static string SampleColor => IsJapanese ? "サンプルカラー" : "Sample Color";
        public static string Tolerance => IsJapanese ? "許容範囲" : "Tolerance";
        public static string UVRect => IsJapanese ? "UV範囲 (0-1)" : "UV Rect (0-1)";
        public static string TargetColor => IsJapanese ? "変更先カラー" : "Target Color";
        public static string PatternPreserve => IsJapanese ? "模様保持" : "Pattern Preserve";
        public static string PatternPreserveTooltip => IsJapanese
            ? "0 = 変更先の明度に完全に合わせる（ベタ塗り風）\n1 = 元の明度を完全保持（模様が残りやすい）"
            : "0 = Match target brightness entirely (flat recolor)\n1 = Keep original brightness (preserves patterns)";
        public static string EdgeSoftness => IsJapanese ? "エッジ柔らかさ" : "Edge Softness";
        public static string EdgeSoftnessTooltip => IsJapanese
            ? "0 = 硬いエッジ（従来通り）\n1 = 柔らかいエッジ（アンチエイリアス境界を滑らかに）"
            : "0 = Hard edge (legacy)\n1 = Soft edge (smooth anti-aliased boundaries)";

        // ─── Processing ───
        public static string Processing => IsJapanese ? "加工設定" : "Processing";
        public static string CleanupRadius => IsJapanese ? "ドット除去" : "Dot Cleanup";
        public static string CleanupRadiusTooltip => IsJapanese
            ? "メディアンフィルタで孤立ドットを除去します。\n0 = オフ、1〜3 = フィルタ半径"
            : "Median filter removes isolated dot artifacts.\n0 = off, 1-3 = filter radius";
        public static string EdgeFeather => IsJapanese ? "エッジぼかし" : "Edge Feather";
        public static string EdgeFeatherTooltip => IsJapanese
            ? "選択境界をガウシアンブラーでぼかし、滑らかな遷移を実現します。\n0 = オフ"
            : "Gaussian blur on selection boundary for smooth transitions.\n0 = off";

        // ─── Exclusion Mask ───
        public static string ExclusionMask => IsJapanese ? "除外マスク" : "Exclusion Mask";
        public static string BrushSize => IsJapanese ? "ブラシサイズ" : "Brush Size";
        public static string BrushMode => IsJapanese ? "ブラシモード" : "Brush Mode";
        public static string Exclude => IsJapanese ? "除外" : "Exclude";
        public static string Include => IsJapanese ? "含める" : "Include";
        public static string ClearMask => IsJapanese ? "マスクをクリア" : "Clear Mask";
        public static string MaskHint => IsJapanese
            ? "プレビュー上でドラッグして塗りつぶし除外"
            : "Drag on preview to paint exclusion";

        // ─── Preview ───
        public static string Preview => IsJapanese ? "プレビュー" : "Preview";
        public static string Zoom => IsJapanese ? "ズーム" : "Zoom";
        public static string SetTexture => IsJapanese
            ? "テクスチャを設定してください。"
            : "Please set a texture.";

        // ─── Export ───
        public static string Export => IsJapanese ? "エクスポート" : "Export";
        public static string SaveAsNewFile => IsJapanese ? "新規ファイルとして保存" : "Save as new file";
        public static string FileName => IsJapanese ? "ファイル名" : "File Name";
        public static string ApplyAndSave => IsJapanese ? "適用して保存" : "Apply & Save";

        // ─── Dialogs ───
        public static string Error => IsJapanese ? "エラー" : "Error";
        public static string Confirm => IsJapanese ? "確認" : "Confirm";
        public static string Complete => IsJapanese ? "完了" : "Complete";
        public static string OK => "OK";
        public static string Overwrite => IsJapanese ? "上書き" : "Overwrite";
        public static string Cancel => IsJapanese ? "キャンセル" : "Cancel";
        public static string TextureReadError => IsJapanese
            ? "テクスチャが読み込めません。Read/Write Enabled を確認してください。"
            : "Cannot read texture. Please check Read/Write Enabled.";
        public static string TextureLoadError => IsJapanese
            ? "テクスチャファイルの読み込みに失敗しました。PNG または JPG 形式のファイルを使用してください。"
            : "Failed to load texture file. Please use a PNG or JPG file.";
        public static string PathNotFound => IsJapanese
            ? "テクスチャのパスが見つかりません。"
            : "Texture path not found.";
        public static string FileExistsConfirm(string path) => IsJapanese
            ? $"{path} は既に存在します。上書きしますか？"
            : $"{path} already exists. Overwrite?";
        public static string OverwriteConfirm => IsJapanese
            ? "元のテクスチャファイルを上書きします。よろしいですか？"
            : "This will overwrite the original texture file. Are you sure?";
        public static string Saved(string path) => IsJapanese
            ? $"保存しました:\n{path}"
            : $"Saved:\n{path}";
    }
}
