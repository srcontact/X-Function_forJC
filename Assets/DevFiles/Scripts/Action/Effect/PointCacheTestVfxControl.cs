using clrev01.Bases;
using clrev01.Extensions;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace clrev01.ClAction.Effect
{
    public class PointCacheTestVfxControl : BaseOfCL
    {
        [SerializeField]
        private VisualEffect vfx;
        private MapToTextureObj _positionMapObj;
        [SerializeField]
        private PointCacheTestVfxUser origUser;
        [ShowInInspector]
        private List<PointCacheTestVfxUser> _users = new();
        [SerializeField]
        private int maxSpawnRadius = 100;


        public void Awake()
        {
            _positionMapObj = new MapToTextureObj(1024, TextureFormat.RGBAFloat, "PositionMap", "PositionCount", vfx);
            SpawnUser();
            SpawnUser();
            SpawnUser();
            SpawnUser();
            SpawnUser();
        }

        public void RegisterUser(PointCacheTestVfxUser user)
        {
            _users.Add(user);
        }

        public void UnregisterUser(PointCacheTestVfxUser user)
        {
            _users.Remove(user);
        }

        private void FixedUpdate()
        {
            foreach (var user in _users)
            {
                var up = user.pos;
                _positionMapObj.SetPixel(new Color(up.x, up.y, up.z));
            }
            _positionMapObj.ApplyMap(vfx);
        }

        [Button(ButtonSizes.Large)]
        public void SpawnUser()
        {
            var instance = origUser.SafeInstantiate(_users.Count.ToString());
            instance.transform.SetPositionAndRotation(Random.insideUnitSphere * maxSpawnRadius, quaternion.identity);
            instance.OnSpawn(this);
        }
    }
}