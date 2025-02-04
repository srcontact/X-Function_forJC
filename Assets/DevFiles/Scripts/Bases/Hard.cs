using Sirenix.OdinInspector;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Bases
{
    public abstract class Hard<C, L, H> : HardBase
        where C : CommonData<C, L, H>
        where L : LocalData<C, L, H>, new()
        where H : Hard<C, L, H>
    {
        #region ld

        [SerializeReference, ReadOnly]
        private L _ld;
        public L ld
        {
            get { return _ld; }
            set
            {
                _ld = value;
                if (_ld == null) return;
                _ld.hd = (H)this;
            }
        }

        #endregion

        //#if UNITY_EDITOR
        public bool notDisActiveOnNoCD;

        private void Start()
        {
            if (ld == null &&
                !notDisActiveOnNoCD)
            {
                gameObject.SetActive(false);
            }
        }
        //#endif

        public override void ResetEveryFrame()
        {
            base.ResetEveryFrame();
            if (ld == null || ld.cd == null) return;
            ld.ResetEveryFrame();
        }
        public override void RunBeforePhysics()
        {
            if (ld == null || ld.cd == null) return;
            ld.RunBeforePhysics();
        }
        public override void RunAfterPhysics()
        {
            ld.RunAfterPhysics();
            DotonCheck();
            if (objectSearchTgt != null) objectSearchTgt.UpdateMovementVector();
            //TransformRoundDown();
        }

        private void TransformRoundDown()
        {
            Vector3 tempPos = pos;
            tempPos.x = RoundDown(tempPos.x, Vector3.kEpsilon);
            tempPos.y = RoundDown(tempPos.y, Vector3.kEpsilon);
            tempPos.z = RoundDown(tempPos.z, Vector3.kEpsilon);
            pos = tempPos;

            Quaternion tempRot = rot;
            tempRot.w = RoundDown(tempRot.w, Quaternion.kEpsilon * 10);
            tempRot.x = RoundDown(tempRot.x, Quaternion.kEpsilon * 10);
            tempRot.y = RoundDown(tempRot.y, Quaternion.kEpsilon * 10);
            tempRot.z = RoundDown(tempRot.z, Quaternion.kEpsilon * 10);
            //Debug.Log(GetInstanceID() + ":" + ld.exeCount + "_" + rot.w + "_" + tempRot.w);


            rot = tempRot;
        }

        private float RoundDown(float value, float dCoef)
        {
            dCoef *= 100;
            return value > 0 ? Mathf.Floor(value / dCoef) * dCoef : Mathf.Ceil(value / dCoef) * dCoef;
        }

        public virtual void Disable()
        {
            gameObject.SetActive(false);
        }

        public void DotonCheck()
        {
            if (!Physics.Raycast(pos, Vector3.down, 1000, layerOfGround))
            {
                OnDotonExe();
            }
        }
    }
}