using System;
using System.Collections.Generic;

namespace VRCAvatarColorChanger
{
    [Serializable]
    public class VACCPresetData
    {
        public string name = "";
        public List<ColorZone> zones = new List<ColorZone>();
        public float edgeFeather;
    }
}
