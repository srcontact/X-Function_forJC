using clrev01.Bases;
using UnityEngine;

namespace clrev01.ClAction
{
    [CreateAssetMenu(menuName = "ActionEnvironment")]
    public class ActionEnvironmentPar : SOBaseOfCL
    {
        #region globalGPow
        /// <summary>
        /// 重力加速度。
        /// !注意!：単位はm/s^2
        /// </summary>
        [SerializeField]
        private float _globalGPowMSec = 1;
        public float globalGPowMSec => _globalGPowMSec;
        #endregion
        #region globalAirBreakPar
        [SerializeField]
        private float _globalAirBreakPar;
        public float globalAirBreakPar => _globalAirBreakPar;
        #endregion
    }
}