using UnityEngine;

namespace clrev01.Extensions
{
    public static class ExPrediction
    {
        /// <summary>
        /// 線形予測照準(旧)
        /// 参考:https://qiita.com/A_rosuko/items/4a0612e4ed91f71813d6
        /// </summary>
        /// <param name="shootPoint">射撃位置</param>
        /// <param name="tgtPosition">ターゲット位置</param>
        /// <param name="tgtSpeed">ターゲット速度</param>
        /// <param name="bulletSpeed">弾速</param>
        /// <param name="bulletAdditionalSpeed">弾に追加で乗る速度。主に射撃者の移動速度</param>
        /// <param name="gravityAccele">重力加速度</param>
        /// <param name="impactFrameCorrection">弾着時間補正。標的に弾が当たるまでの時間の計算結果を補正する（デフォルトでは明らかに遅れていたので）</param>
        /// <returns></returns>
        public static Vector3 _old_LinePrediction(
            Vector3 shootPoint, Vector3 tgtPosition, Vector3 tgtSpeed,
            Vector3 bulletSpeed, Vector3 bulletAdditionalSpeed, float gravityAccele, int impactFrameCorrection = 0)
        {
            Vector3 relativePosition = tgtPosition - shootPoint;

            float a = tgtSpeed.sqrMagnitude - bulletSpeed.sqrMagnitude;
            float b = Vector3.Dot(tgtPosition, tgtSpeed) - Vector3.Dot(shootPoint, bulletSpeed + bulletAdditionalSpeed);
            // float b = Vector3.Dot(relativePosition, tgtSpeed );
            float c = tgtPosition.sqrMagnitude - shootPoint.sqrMagnitude;
            // float c = relativePosition.sqrMagnitude;

            //0割禁止
            if (a == 0 && b == 0) return tgtPosition;
            float impactTimeCorrection = impactFrameCorrection / 60f;
            float impactTime;
            if (a == 0)
            {
                impactTime = (-c / b / 2) + impactTimeCorrection;
            }
            else
            {
                //虚数解はどうせ当たらないので絶対値で無視した
                float d = Mathf.Sqrt(Mathf.Abs(b * b - a * c));
                impactTime = PlusMin((-b - d) / a, (-b + d) / a) + impactTimeCorrection;
            }
            Vector3 gravityFall = new Vector3(0, gravityAccele / 2 * impactTime * impactTime, 0);
            return tgtPosition + (tgtSpeed - bulletAdditionalSpeed) * impactTime + gravityFall;
        }

        static float PlusMin(float a, float b)
        {
            if (a < 0 && b < 0) return 0;
            if (a < 0) return b;
            if (b < 0) return a;
            return a < b ? a : b;
        }

        /// <summary>
        /// 線形予測射撃
        /// </summary>
        /// <param name="tgtPosition">ターゲット位置</param>
        /// <param name="tgtVector">ターゲットの移動ベクトル</param>
        /// <param name="bulletSpeed">弾速</param>
        /// <param name="bulletNvreak">弾の空気抵抗値</param>
        /// <param name="shooterPosition">射撃者の位置</param>
        /// <param name="shooterVector">射撃者の移動ベクトル(弾の移動ベクトルに合算される)</param>
        /// <param name="gravityAccele">重力加速度</param>
        /// <param name="frameRate">予測射撃のフレームレート</param>
        /// <returns></returns>
        public static Vector3 LinePrediction(Vector3 tgtPosition, Vector3 tgtVector,
            float bulletSpeed, float bulletNvreak, Vector3 shooterPosition, Vector3 shooterVector, float gravityAccele, int frameRate = 60)
        {
            //射撃者→Tgtへの位置ベクトル
            var nowVectorToTgt = tgtPosition - shooterPosition;
            //射撃者-Tgtの相対速度ベクトル
            var relativeVector = tgtVector - shooterVector;
            //射撃者→Tgtへの距離（初期値）
            var relativeLength = nowVectorToTgt.magnitude;
            //射撃者-Tgtの相対速度
            var relativeSpeed = CalcSpeedSign(relativeVector, nowVectorToTgt) * Vector3.Project(relativeVector, nowVectorToTgt).magnitude;
            //弾の初速
            var bulletRealStartSpeed = bulletSpeed + CalcSpeedSign(shooterVector, nowVectorToTgt) * Vector3.Project(shooterVector, nowVectorToTgt).magnitude;

            //Tgtの距離に到達するまで、1フレームずつ飛翔距離を合算する
            float calcBulletSpeed = bulletRealStartSpeed;
            float calcFlightLength = 0;
            float predictionLengthToTgt = relativeLength;
            float maxFrame = gravityAccele != 0 ? Mathf.Sqrt(2 * relativeLength / gravityAccele) * frameRate : frameRate * 20;
            int i;
            for (i = 1; i <= maxFrame; i++)
            {
                calcBulletSpeed -= calcBulletSpeed * bulletNvreak / frameRate;
                calcFlightLength += calcBulletSpeed / frameRate;
                predictionLengthToTgt += relativeSpeed / frameRate;
                var gravityFall = gravityAccele * Mathf.Pow((float)i / frameRate, 2) / 2;
                var length = Mathf.Pow(predictionLengthToTgt, 2) + Mathf.Pow(gravityFall, 2);
                if (length <= Mathf.Pow(calcFlightLength, 2)) break;
            }
            var gravityFallCorrection = gravityAccele * Mathf.Pow((float)i / frameRate, 2) / 2 * Vector3.up;
            var result = tgtPosition + relativeVector * i / frameRate + gravityFallCorrection;
            return result;
        }

        /// <summary>
        /// 直線速度のプラスマイナス符号を計算する。
        /// </summary>
        /// <param name="calcVector">計算する速度ベクトル</param>
        /// <param name="nowVectorToTgt">射撃者→tgtへの位置ベクトル</param>
        /// <returns></returns>
        private static int CalcSpeedSign(Vector3 calcVector, Vector3 nowVectorToTgt)
        {
            //接近時はマイナス、離れていくときはプラス
            return Vector3.Dot(nowVectorToTgt, calcVector) > 0 ? 1 : -1;
        }
    }
}