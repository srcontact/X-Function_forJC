using UnityEngine;

namespace clrev01.ClAction.ObjectSearch
{
    public class PerlinNoise3dPar
    {
        private Vector3? _offset;
        private readonly float _moveRate = 1;

        public Vector3 GetNoise3d(int frame, int searcherOffset)
        {
            _offset ??= Random.insideUnitSphere * 256;
            return new Vector3(
                Mathf.PerlinNoise1D(_offset.Value.x + searcherOffset + frame / 60f * _moveRate) - 0.5f,
                Mathf.PerlinNoise1D(_offset.Value.y + searcherOffset + frame / 60f * _moveRate) - 0.5f,
                Mathf.PerlinNoise1D(_offset.Value.z + searcherOffset + frame / 60f * _moveRate) - 0.5f
            );
        }
    }
}