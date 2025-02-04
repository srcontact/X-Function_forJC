using clrev01.ClAction;
using clrev01.ClAction.OptionParts;
using clrev01.Extensions;
using clrev01.HUB;
using clrev01.Menu;
using clrev01.Menu.BattleMenu.Arena;
using clrev01.PGE.NodeFace;
using clrev01.Save;
using clrev01.Settings;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace clrev01.Bases
{
    public static class UtlOfCL
    {
        #region ACM

        [SerializeField]
        private static ActionManager _ACM;
        /// <summary>
        /// ActionManager
        /// アクション全体を管理するオブジェクト
        /// </summary>
        public static ActionManager ACM => _ACM ? _ACM : _ACM = Object.FindObjectOfType<ActionManager>();

        #endregion

        #region MHUB

        [SerializeField]
        private static MachineHub _MHUB;
        /// <summary>
        /// MachineHub
        /// 機体オブジェクトを登録し呼び出す。
        /// </summary>
        public static MachineHub MHUB => _MHUB ? _MHUB : _MHUB = Addressables.LoadAssetAsync<MachineHub>("MachineHub").WaitForCompletion();

        #endregion

        #region WHUB

        [SerializeField]
        private static WeaponHub _WHUB;
        /// <summary>
        /// WeaponHub
        /// 発射物を登録し、呼び出す。
        /// </summary>
        public static WeaponHub WHUB => _WHUB ? _WHUB : _WHUB = Addressables.LoadAssetAsync<WeaponHub>("WeaponHub").WaitForCompletion();

        #endregion

        #region PowerPlantHub

        [SerializeField]
        private static PowerPlantHub _PpHub;
        public static PowerPlantHub PpHub => _PpHub ? _PpHub : _PpHub = Addressables.LoadAssetAsync<PowerPlantHub>("PowerPlantHub").WaitForCompletion();

        #endregion

        #region FcsHub

        [SerializeField]
        private static FcsHub _FcsHub;
        public static FcsHub FHub => _FcsHub ? _FcsHub : _FcsHub = Addressables.LoadAssetAsync<FcsHub>("FcsHub").WaitForCompletion();

        #endregion

        #region CpuHub

        [SerializeField]
        private static CpuHub _CHub;
        public static CpuHub CHub => _CHub ? _CHub : _CHub = Addressables.LoadAssetAsync<CpuHub>("CpuHub").WaitForCompletion();

        #endregion

        #region ArmorTypeHub

        [SerializeField]
        private static ArmorTypeHub _ATHub;
        public static ArmorTypeHub ATHub => _ATHub ? _ATHub : _ATHub = Addressables.LoadAssetAsync<ArmorTypeHub>("ArmorTypeHub").WaitForCompletion();

        #endregion

        #region ThrustHub

        private static ThrusterHub _THub;
        public static ThrusterHub THub => _THub ? _THub : _THub = Addressables.LoadAssetAsync<ThrusterHub>("ThrusterHub").WaitForCompletion();

        #endregion

        #region ShieldHub

        [SerializeField]
        private static ShieldHub _ShldHub;
        public static ShieldHub ShldHub => _ShldHub ? _ShldHub : _ShldHub = Addressables.LoadAssetAsync<ShieldHub>("ShieldHub").WaitForCompletion();

        #endregion

        #region OptionHub

        private static OptionPartsHub _OpHub;
        public static OptionPartsHub OpHub => _OpHub ? _OpHub : _OpHub = Addressables.LoadAssetAsync<OptionPartsHub>("OptionPartsHub").WaitForCompletion();

        #endregion

        #region NodeFaceHub

        [SerializeField]
        private static NodeFaceHub _NfHub;
        public static NodeFaceHub NfHub => _NfHub ? _NfHub : _NfHub = Addressables.LoadAssetAsync<NodeFaceHub>("NodeFaceHub").WaitForCompletion();

        #endregion

        #region MPPM

        /// <summary>
        /// MenuManager
        /// メニュー全体を管理するオブジェクト
        /// </summary>
        public static MenuPagePanelManager MPPM
        {
            get { return MenuPagePanelManager.Inst; }
        }

        #endregion

        #region UBS

        [SerializeField]
        private static UIBaseSettings _UBS;
        /// <summary>
        /// UIBaseSettings
        /// UIの基礎挙動パラメータなどをまとめる。
        /// </summary>
        public static UIBaseSettings UBS => _UBS ? _UBS : _UBS = Addressables.LoadAssetAsync<UIBaseSettings>("UIBaseSettings").WaitForCompletion();

        #endregion

        #region ArenaHub

        private static ArenaHub _ArenaHub;
        public static ArenaHub ArnHub => _ArenaHub ? _ArenaHub : _ArenaHub = Addressables.LoadAssetAsync<ArenaHub>("ArenaHub").WaitForCompletion();

        #endregion

        #region Layers

        public static int layerOfTouchCollision { get; } = 1 << LayerMask.NameToLayer("TouchCollision");
        public static int layerOfMachine { get; } = 1 << LayerMask.NameToLayer("Machine");
        public static int layerOfBullet { get; } = 1 << LayerMask.NameToLayer("Bullet");
        public static int layerOfMissile { get; } = 1 << LayerMask.NameToLayer("Missile");
        public static int layerOfMine { get; } = 1 << LayerMask.NameToLayer("Mine");
        public static int layerOfAerialSmallObject { get; } = 1 << LayerMask.NameToLayer("AerialSmallObject");
        public static int layerOfGround { get; } = 1 << LayerMask.NameToLayer("Ground");
        public static int layerOfShield { get; } = 1 << LayerMask.NameToLayer("Shield");
        public static int layerOfSmokeScreen { get; } = 1 << LayerMask.NameToLayer("SmokeScreen");

        #endregion


        [System.Serializable]
        public struct DirectionPar
        {
            public float forward, back, left, right, up, down;
        }

        [System.Serializable]
        public struct DirectionHPar
        {
            public float forward, back, left, right;
        }

        public static Vector3 DPforV3(this DirectionPar dp, Vector3 v)
        {
            Vector3 nv = Vector3.zero;
            if (v.y != 0)
            {
                if (v.y.isBiggerV3(0)) nv.y = dp.up;
                else nv.y = dp.down;
            }
            if (v.z != 0)
            {
                if (v.z.isBiggerV3(0)) nv.z = dp.forward;
                else nv.z = dp.back;
            }
            if (v.x != 0)
            {
                if (v.x.isBiggerV3(0)) nv.x = dp.right;
                else nv.x = dp.left;
            }
            return nv;
        }
        public static Vector3 DHPforV3(this DirectionHPar dp, Vector3 v)
        {
            Vector3 nv = Vector3.zero;

            if (v.z != 0)
            {
                if (v.z.isBiggerV3(0)) nv.z = dp.forward;
                else nv.z = dp.back;
            }
            if (v.x != 0)
            {
                if (v.x.isBiggerV3(0)) nv.x = dp.right;
                else nv.x = dp.left;
            }
            return nv;
        }
        public static Vector2 DHPforV2(this DirectionHPar dp, Vector3 v)
        {
            Vector2 nv = Vector2.zero;

            if (v.z != 0)
            {
                if (v.z.isBiggerV3(0)) nv.y = dp.forward;
                else nv.y = dp.back;
            }
            if (v.x != 0)
            {
                if (v.x.isBiggerV3(0)) nv.x = dp.right;
                else nv.x = dp.left;
            }
            return nv;
        }

        public const float epsilonLookRotation = 0.0001f;

        public static Quaternion LookRotationCheckViewingVectorIsZero(Vector3 forward, Vector3 upwards)
        {
            forward = forward.normalized;
            upwards = upwards.normalized;
            if (forward.sqrMagnitude <= epsilonLookRotation || upwards.sqrMagnitude <= epsilonLookRotation) return Quaternion.identity;
            return Quaternion.LookRotation(forward, upwards);
        }

        /// <summary>
        /// 古い正面方向のベクトルと新しい上方ベクトルから地面に平行になる回転を返す
        /// </summary>
        /// <param name="oldForward">古い正面方向のベクトル</param>
        /// <param name="newUp">新しい上方ベクトル</param>
        /// <returns>地面に平行になる回転</returns>
        public static Quaternion VectorFollowTheGround(Vector3 oldForward, Vector3 newUp)
        {
            Vector3 newRight = Vector3.Cross(newUp, oldForward);
            Vector3 newForward = Vector3.Cross(newRight, newUp);
            return LookRotationCheckViewingVectorIsZero(newForward, newUp);
        }

        public static int CountFromNull(this ICollection list)
        {
            if (list == null) return -1;
            return list.Count;
        }

        /// <summary>
        /// 0~360度の角度を-180~180度に正規化する。
        /// </summary>
        /// <param name="eulerAngles">0~360度の角度</param>
        /// <returns></returns>
        public static Vector3 EulerAnglesNormalize180(this Vector3 eulerAngles)
        {
            return new Vector3(
                eulerAngles.x.EulerAngleNormalize180(),
                eulerAngles.y.EulerAngleNormalize180(),
                eulerAngles.z.EulerAngleNormalize180()
            );
        }

        /// <summary>
        /// 0~360度の角度を-180~180度に正規化する。
        /// </summary>
        /// <param name="eulerAngle">0~360度の角度</param>
        /// <returns></returns>
        public static float EulerAngleNormalize180(this float eulerAngle)
        {
            return Mathf.Repeat(eulerAngle + 180, 360) - 180;
        }

        public static int GetDetectObjectLayer(DetectObjectFlags detectObjectFlags)
        {
            var layer = 0;
            if ((detectObjectFlags & DetectObjectFlags.Ground) == DetectObjectFlags.Ground) layer |= layerOfGround;
            if ((detectObjectFlags & DetectObjectFlags.Friend) == DetectObjectFlags.Friend) layer |= layerOfMachine;
            return layer;
        }

        public static float GetSpeedUnitRatio(this SpeedUnitType speedUnitType)
        {
            switch (speedUnitType)
            {
                case SpeedUnitType.MeterPerFrame:
                    return 1f / 60f;
                case SpeedUnitType.MeterPerSecond:
                default:
                    return 1;
                case SpeedUnitType.KiloMeterPerHour:
                    return 3.6f;
            }
        }

        public static string GetSpeedUnitText(this SpeedUnitType speedUnitType)
        {
            switch (speedUnitType)
            {
                case SpeedUnitType.MeterPerFrame:
                    return "m/f";
                case SpeedUnitType.MeterPerSecond:
                default:
                    return "m/s";
                case SpeedUnitType.KiloMeterPerHour:
                    return "km/h";
            }
        }

        public static string GetEnumFlagText(Type enumType, long flags, int? length = null)
        {
            var enumList = ((long[])Enum.GetValues(enumType)).ToList();
            return enumList.All(
                x => (flags & x) == x)
                ? "ALL"
                : string.Join(",", enumList.Where(x => (flags & x) == x).ToList().ConvertAll(x => (Enum.GetName(enumType, x)?.ToString())).ConvertAll(x => length.HasValue ? x[..length.Value] : x));
        }
        public static string GetEnumFlagText(Type enumType, int flags, int? length = null)
        {
            var enumList = ((int[])Enum.GetValues(enumType)).ToList();
            return enumList.All(
                x => (flags & x) == x)
                ? "ALL"
                : string.Join(",", enumList.Where(x => (flags & x) == x).ToList().ConvertAll(x => (Enum.GetName(enumType, x)?.ToString())).ConvertAll(x => length.HasValue ? x[..length.Value] : x));
        }
        public static string GetEllipsisString(string str, int maxByteCount, int prefixSuffixCount)
        {
            if (str == null) return null;
            // var byteCount = Encoding.GetEncoding("Shift_JIS").GetByteCount(str);
            var si = new StringInfo(str);
            var byteCount = si.LengthInTextElements;
            if (byteCount <= maxByteCount) return str;
            var pre = si.SubstringByTextElements(0, prefixSuffixCount);
            var suf = si.SubstringByTextElements(byteCount - prefixSuffixCount - 1, prefixSuffixCount);
            return $"{pre}...{suf}";
        }
        public static string GetComparatorStr(ComparatorType comparatorType)
        {
            var comparatorStr = comparatorType switch
            {
                ComparatorType.EqualTo => "=",
                ComparatorType.Over => ">=",
                ComparatorType.GreaterThan => ">",
                ComparatorType.Under => "<=",
                ComparatorType.LessThan => "<",
                _ => throw new ArgumentOutOfRangeException()
            };
            return comparatorStr;
        }
    }
}