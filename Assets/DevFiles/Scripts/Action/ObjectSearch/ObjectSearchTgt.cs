using clrev01.Bases;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.ObjectSearch
{
    public class ObjectSearchTgt : BaseOfCL
    {
        [ReadOnly]
        public HardBase hardBase;
        /// <summary>
        /// オブジェクトの位置を取得するTransformを指定する。
        /// Animationでポーズ変更した場合に見た目通りにオブジェクトの位置を取得できるようにする。
        /// 設定しない場合はposの値を位置として使用する。
        /// </summary>
        [SerializeField]
        private Transform targetObj;
        [SerializeField, HideInInspector]
        private long objectSearchType;
        [ShowInInspector]
        public SearchTgtType ObjectSearchType
        {
            get => (SearchTgtType)objectSearchType;
            private set => objectSearchType = (long)value;
        }
        [ReadOnly]
        public int chunkNum = -1;
        [ReadOnly]
        public int indexNum = -1;
        [ReadOnly]
        public Dictionary<int, ObjectSearchTgt> aimingAtMeDict;
        [ReadOnly]
        public Vector3 movementVector;
        private Vector3 _previousPos;
        [ReadOnly]
        public bool ignoreRegisterFlag;
        /// <summary>
        /// 自分をサーチする相手に対して与えるジャミングサイズ
        /// </summary>
        [ReadOnly]
        public float jammingSize;
        /// <summary>
        /// 自分がサーチするときに被るジャミングサイズ
        /// </summary>
        [ReadOnly]
        public float jammedSize;
        private readonly PerlinNoise3dPar _perlinNoise3dPar = new();

        private void Awake()
        {
            hardBase = GetComponent<HardBase>();
        }
        public void InitMovementVector(Vector3 movementVector, Vector3 previousPos)
        {
            this.movementVector = movementVector;
            _previousPos = previousPos;
        }
        public void UpdateMovementVector()
        {
            movementVector = (pos - _previousPos).normalized;
            _previousPos = pos;
        }
        public void RegisterSearchTgt()
        {
            if (ObjectSearch.Inst != null) ObjectSearch.Inst.RegisterTgt(this);
        }
        public void UnregisterSearchTgt()
        {
            if (ObjectSearch.Inst != null) ObjectSearch.Inst.UnregisterTgt(this);
        }

        /// <summary>
        /// 位置精度の値を取得する
        /// </summary>
        /// <param name="searcherPos">サーチ実行者の位置</param>
        /// <param name="spd">サーチ実行者のサーチパラメータ</param>
        /// <param name="searcherJammedSize">サーチ実行者の被ジャミングサイズ</param>
        /// <returns></returns>
        public float GetPosAccuracy(Vector3 searcherPos, SearchParameterData spd, float searcherJammedSize)
        {
            var direction = pos - searcherPos;
            var accuracy = Physics.Raycast(
                searcherPos,
                direction.normalized,
                direction.magnitude,
                layerOfGround + layerOfSmokeScreen,
                QueryTriggerInteraction.Collide
            )
                ? spd.concealedAccuracy
                : spd.visibleAccuracy;
            var ratio = direction.magnitude / 1000;
            return ratio * accuracy + (jammingSize + searcherJammedSize) * spd.antiJammingRate;
        }
        /// <summary>
        /// 位置の誤差を取得する
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="posAccuracy"></param>
        /// <returns></returns>
        private Vector3 GetPosError(int uniqueId, float posAccuracy)
        {
            return _perlinNoise3dPar.GetNoise3d(ACM.actionFrame, uniqueId + hardBase.uniqueID) * posAccuracy;
        }
        /// <summary>
        /// ObjectSearchTgtの現在位置を諸々の影響値込みで取得する
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="uniqueId"></param>
        /// <param name="spd"></param>
        /// <param name="posAccuracy"></param>
        /// <returns></returns>
        public Vector3 GetTargetPosition(int teamId, int uniqueId, SearchParameterData spd, float posAccuracy)
        {
            var position = targetObj ? targetObj.position : pos;
            if (teamId == hardBase.teamID || spd == null) return position;
            var posError = GetPosError(uniqueId, posAccuracy);
            return position + posError;
        }
        /// <summary>
        /// ObjectSearchTgtの現在速度ベクトルを諸々の影響値込みで取得する
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="uniqueId"></param>
        /// <param name="spd"></param>
        /// <param name="posAccuracy"></param>
        /// <returns></returns>
        public Vector3 GetTargetVelocity(int teamId, int uniqueId, SearchParameterData spd, float posAccuracy)
        {
            if (teamId == hardBase.teamID || spd == null) return hardBase.rigidBody.linearVelocity;
            return hardBase.rigidBody.linearVelocity + GetPosError(uniqueId, posAccuracy);
        }
    }
}