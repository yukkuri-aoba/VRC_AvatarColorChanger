"""PSD レイヤー構造を出力するユーティリティ"""
from psd_tools import PSDImage
import os

BASE = os.path.join(os.path.dirname(__file__), "..")

def inspect(rel_path):
    path = os.path.join(BASE, rel_path)
    psd = PSDImage.open(path)
    print(f"=== {os.path.basename(rel_path)} ({psd.width}x{psd.height}) ===")
    def walk(layers, indent=1):
        for layer in layers:
            prefix = "  " * indent
            print(f"{prefix}[{layer.kind}] \"{layer.name}\" visible={layer.visible} blend={layer.blend_mode} bbox={layer.bbox}")
            if hasattr(layer, "__iter__"):
                walk(layer, indent + 1)
    walk(psd)
    print()

for name in ["HAOLAN_Hair.psd", "HAOLAN_Costume.psd"]:
    inspect(os.path.join("texture_sample", "HAOLAN", "PSD", name))

for name in ["Clothes.psd", "Hair.psd"]:
    inspect(os.path.join("texture_sample", "Feina", "Feina_PSD_CLIP", name))
