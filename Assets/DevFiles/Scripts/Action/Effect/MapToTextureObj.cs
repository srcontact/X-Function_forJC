using UnityEngine;
using UnityEngine.VFX;

namespace clrev01.ClAction.Effect
{
    public class MapToTextureObj
    {
        private readonly Texture2D _map;
        private readonly int _mapWidth;
        private readonly string _setCountKey;

        private int _mappedCount;

        public MapToTextureObj(int width, TextureFormat textureFormat, string setTextureKey, string setCountKey, VisualEffect vfx)
        {
            _map = new Texture2D(width, 1, textureFormat, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            _mapWidth = width;
            _setCountKey = setCountKey;
            vfx.SetTexture(setTextureKey, _map);
        }

        public void SetPixel(Color mapInfo)
        {
            if (_mappedCount >= _mapWidth) return;
            _map.SetPixel(_mappedCount++, 0, mapInfo);
        }

        public void ApplyMap(VisualEffect vfx)
        {
            _map.Apply();
            vfx.SetInt(_setCountKey, _mappedCount);
            _mappedCount = 0;
        }
    }
}