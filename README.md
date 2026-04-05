# VRC AvatarColorChanger (VACC)

*[日本語](#日本語) | [English](#english)*

> **注：** 日本語版が公式版です。英語版は参考情報としてご利用ください。
> また、英語版はAI翻訳を使用しています。(Claude Haiku 4.5による翻訳)

---

## 日本語

### 概要

VRC AvatarColorChanger (VACC) は、Unity Editor 上でテクスチャの色を直感的に変更できるエディタ拡張ツールです。
主に VRChat アバターのテクスチャ編集を想定していますが、一般的な Unity プロジェクトでも使用できます。

### 推しポイント
- **オープンソース**
MIT License でソースコード公開。誰でも自由に利用・改変・再配布できます。
- **無料**
このツールは基本的に無料で利用できます。投げ銭は歓迎ですが、購入は任意です。
- **アトラス化されたテクスチャもOK** 
ブラシで除外マスクを簡単に描けるので、複数のパーツが1枚のテクスチャにまとめられていても利用できます。


### 機能

- **カラーゾーン**
複数のゾーンを定義してテクスチャの特定部分を一括リカラー
- **カラーピック モード**
  サンプルカラーと許容範囲 (Tolerance) で対象ピクセルを自動検出
- **UV矩形 モード**
  UV座標で矩形範囲を指定してリカラー
- **模様保持スライダー**
元テクスチャの明度をどの割合で残すか調整（0 = ベタ塗り、1 = 模様完全保持）
- **除外マスク**
プレビュー上でブラシをドラッグし、リカラーしたくない領域をペイントして除外
- **リアルタイムプレビュー**
ズーム対応。設定変更のたびに即座に反映
- **エクスポート**
新規 PNG として保存、または元テクスチャへの上書き保存に対応
- **日本語・英語対応**
日本語・英語の UI を自動検知。ツールバーで手動切り替えも可能

### 動作環境(検証済みの環境)

- Unity 2022.3.22f1（Editor スクリプト）
- VCC / VRCSDK との依存関係はありません（Unity Editor 単体で動作）

> これ以外でも動作する可能性はありますが、未検証です。

### 使い方



> **注意:** 元テクスチャは **Read/Write Enabled** が必要です。オフの場合はウィンドウ内のボタンで自動有効化できます。

### ライセンス

MIT License

Copyright (c) 2026 yukkuri__aoba

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

### 免責事項

- 本ツールは現状有姿で提供されます。使用によって生じた損害・不具合・データ損失について、制作者(yukkuri__aoba)は一切の責任を負いません。
- バグや不具合についてはできる限り対応しますが、修正を保証するものではありません。
- VRChat の利用規約・ガイドラインに従った使用は利用者の責任です。

---

## English

> **Note:** The Japanese version is the official version. Please use the English version as reference information.

### Overview

VRC AvatarColorChanger (VACC) is a Unity Editor extension for intuitively recoloring textures.
Primarily designed for VRChat avatar texture editing, but works in any Unity project.

### Features

- **Color Zones** — Define multiple zones to recolor specific areas of a texture in bulk
  - **Color Pick mode** — Auto-detect target pixels by sample color and a Tolerance value
  - **UV Rect mode** — Specify a rectangular region by UV coordinates for recoloring
- **Pattern Preserve slider** — Control how much of the original brightness is retained (0 = flat recolor, 1 = full pattern preservation)
- **Exclusion Mask** — Paint areas on the preview with a brush to exclude them from recoloring
- **Real-time Preview** — Zoom-capable; updates instantly on every change
- **Export** — Save as a new PNG file or overwrite the source texture
- **Multilingual UI** — Auto-detects Japanese/English. Manual toggle available in the toolbar

### Requirements

- Unity 2022.3.22f1 (Editor script)
- No VCC / VRCSDK required (works standalone in Unity Editor)

### Usage



> **Note:** The source texture must have **Read/Write Enabled**. If it is off, use the in-window button to enable it automatically.

### License

MIT License

Copyright (c) 2026 yukkuri__aoba

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

### Disclaimer

- This tool is provided "as is" without warranty of any kind. The author (yukkuri__aoba) is not liable for any damages, data loss, or issues arising from its use.
- Bugs will be addressed on a best-effort basis, but fixes are not guaranteed.
- It is the user's responsibility to comply with VRChat's Terms of Service and Community Guidelines.

---

## クレジット / Credits

| Role | Name |
|------|------|
| Developer / 開発 | **yukkuri__aoba** |
| AI Assistance / AI補助 | **Claude Sonnet 4.6** (Anthropic) |

## 連絡先 / Contact
- Misskey.io: [@yukkuri__aoba@misskey.io](https://misskey.io/@yukkuri__aoba)
