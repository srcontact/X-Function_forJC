using clrev01.Bases;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.ClAction.Radar
{
    public class RadarSymbol : PoolableBehaviour
    {
        [SerializeField]
        private List<Renderer> _renderers = new();

        public void SetMaterial(Material material)
        {
            _renderers.ForEach(x => x.material = material);
        }
    }
}