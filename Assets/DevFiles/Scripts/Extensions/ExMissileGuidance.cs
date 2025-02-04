using UnityEngine;

namespace clrev01.Extensions
{
    public static class ExMissileGuidance
    {
        /// <summary>
        /// 比例航法の目標ベクトルを導出
        /// </summary>
        /// <param name="targetPos">ターゲットの位置</param>
        /// <param name="missileRigidbody">自分（ミサイル）のRigidBody</param>
        /// <param name="navigationalConstant">比例定数</param>
        /// <param name="deltaTime">単位時間</param>
        /// <param name="previousLos">前回のLOS</param>
        /// <param name="previousDirection">前回の目標ベクトル</param>
        /// <param name="advanceMode">加速度を考慮するオプション</param>
        /// <returns></returns>
        public static Vector3 GetProNavDirection(Vector3 targetPos, Rigidbody missileRigidbody, float navigationalConstant, float deltaTime, ref Vector3 previousLos, Vector3 previousDirection, bool advanceMode = true)
        {
            var los = targetPos - missileRigidbody.position;
            var losDelta = los - previousLos;
            losDelta = losDelta - Vector3.Project(losDelta, los);
            Vector3 desiredRotation;
            if (advanceMode)
            {
                desiredRotation = (deltaTime * los) + (losDelta * navigationalConstant) + (previousDirection * (deltaTime * navigationalConstant) / 2);
            }
            else
            {
                desiredRotation = (deltaTime * los) + (losDelta * navigationalConstant);
            }
            previousLos = los;
            return desiredRotation.normalized;
        }

        public static Vector3 GetTailChaseDirection(Vector3 targetPos, Vector3 missilePos, float accele, float gravity)
        {
            var antiGravityAngle = Mathf.Atan(gravity / accele) * Mathf.Rad2Deg;
            var toTgt = targetPos - missilePos;
            return Quaternion.AngleAxis(antiGravityAngle, Vector3.Cross(toTgt, Vector3.up)) * toTgt.normalized;
        }
    }
}