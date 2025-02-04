using clrev01.Bases;
using clrev01.PGE.PGEM;
using clrev01.Programs.FieldPar;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Programs
{
    public static class UtlOfProgram
    {
        public static PGEManager PGEM2 => PGEManager.Inst;
        private static ProgramCommonData pcd;
        public static ProgramCommonData PCD => pcd ??= Addressables.LoadAssetAsync<ProgramCommonData>("ProgramCommonData").WaitForCompletion();

        #region PG
        //public enum PGBType : long
        //{
        //    start,
        //    //ここから動作(Action)ブロック。
        //    move,
        //    jump,
        //    rotate,
        //    quickRotate,
        //    aim,
        //    shot,
        //    fight,
        //    breaking,
        //    boost,
        //    seald,
        //    haltAction,
        //    //ここから機能(Function)ブロック。
        //    halt,
        //    lockOn,
        //    //ここより下は分岐(Branch)ブロック。
        //    search,
        //    assessTargetPos,
        //}

        public enum SearchFieldType
        {
            Box,
            Circle,
            Sphere,
        }

        public static ISearchFieldUnion GetFieldPar(this SearchFieldType searchFieldType)
        {
            switch (searchFieldType)
            {
                case SearchFieldType.Box:
                    return new BoxSearchFieldParVariable();
                case SearchFieldType.Circle:
                    return new CircleSearchFieldParVariable();
                case SearchFieldType.Sphere:
                    return new SphereSearchFieldParVariable();
                default:
                    return null;
            }
        }

        public enum LockOnDistancePriorityType
        {
            Near,
            Far,
        }

        public enum LockOnAngleOfMovementToSelfType
        {
            None,
            SmallerThan,
            BiggerThan,
        }

        [Flags]
        public enum IdentificationType : long
        {
            Enemy = 1,
            Friend = 2,
            Unknown = 4,
        }

        [Flags]
        public enum ObjType : long
        {
            Machine = 1 << 0,
            Bullet = 1 << 1,
            Missile = 1 << 2,
            Mine = 1 << 3,
            AerialSmallObject = 1 << 4,
            Shield = 1 << 16,
        }

        public static int GetLayerFromSearchFieldType(int searchFieldType)
        {
            int result = 0;
            if ((searchFieldType & (1 + (1 << 1) + (1 << 2))) != 0)
            {
                result += layerOfMachine;
            }
            if ((searchFieldType & (1 << 3)) != 0)
            {
                result += layerOfBullet;
            }
            if ((searchFieldType & (1 << 4)) != 0)
            {
                //ミサイル
            }

            return result;
        }
        public static bool JadgeBulletCollider(Collider c, string tag, Transform hdTransform)
        {
            int l = 1 << c.gameObject.layer;
            bool res = false;
            if (l == layerOfMachine)
            {
                res = !c.CompareTag(tag);
            }
            else if (l == layerOfBullet)
            {
                res = c.transform.InverseTransformPoint(hdTransform.position).z > 0;
            }
            return res;
        }
        #endregion
    }
}