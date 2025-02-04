using clrev01.Menu;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FieldPar;
using MemoryPack;
using System;
using System.Collections.Generic;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.Programs.FuncPar.FuncParType
{
    [System.Serializable]
    public abstract class PGBFuncPar : IPGBFuncUnion
    {
        public static readonly List<Type> AddablePGBTypes = new()
        {
            //typeof(StartFuncPar),
            typeof(NopFuncPar),
            typeof(MoveFuncPar),
            typeof(DashFuncPar),
            typeof(JumpFuncPar),
            typeof(BreakFuncPar),
            typeof(RotateFuncPar),
            typeof(ThrustFuncPar),
            typeof(AimFuncPar),
            typeof(FireFuncPar),
            typeof(FightFuncPar),
            typeof(DefenceFuncPar),
            typeof(ShieldFuncPar),
            typeof(UseOptionalFuncPar),
            typeof(StopProgramFuncPar),
            typeof(StopActionFuncPar),
            typeof(CalcFuncPar),
            typeof(CalcVector3dFuncPar),
            typeof(CalcNumericListFuncPar),
            typeof(CalcVector3dListFuncPar),
            typeof(ManageLockOnFuncPar),
            typeof(LockOnFuncPar),
            typeof(SearchFuncPar),
            typeof(AssessTargetPosFuncPar),
            typeof(AssessSelfPosFuncPar),
            typeof(AssessTargetTypeFuncPar),
            typeof(AssessTargetStatusFuncPar),
            typeof(AssessSelfStatusFuncPar),
            typeof(AssessTargetActionFuncPar),
            typeof(AssessSelfActionFuncPar),
            typeof(AssessLineHittingFuncPar),
            typeof(AssessMoveDirectionFuncPar),
            // typeof(GetStatusValueFuncPar),
            typeof(GetBattleStatusValueFuncPar),
            typeof(GetSelfStatusValueFuncPar),
            typeof(GetTargetStatusValueFuncPar),
            typeof(GetTargetStatusListFuncPar),
            typeof(GetEnvironmentStatusValueFuncPar),
            typeof(GetListStatusFuncPar),
            typeof(LockOnListRemoveNullFuncPar),
            typeof(AssessVariableFuncPar),
            typeof(AssessAreaLimitFuncPar),
            typeof(AssessNumberOfAimingMeFuncPar),
            typeof(AssessLockOnNumber),
            typeof(SubroutineRootFuncPar),
            typeof(SubroutineExecuteFuncPar),
        };

        private static List<IPGBFuncUnion> instanceCache;
        public static List<IPGBFuncUnion> InstanceCache => instanceCache ??= AddablePGBTypes.ConvertAll(x => (IPGBFuncUnion)Activator.CreateInstance(x));

        private static List<string> addableTypeStrings;
        public static List<string> AddableTypeStrings => addableTypeStrings ??= InstanceCache.ConvertAll(x => x.BlockTypeStr);

        private static List<(ColorBlockAsset cba, ColorBlockAsset cbat)> colorBlockAssets;
        public static List<(ColorBlockAsset cba, ColorBlockAsset cbat)> ColorBlockAssets => colorBlockAssets ??= InstanceCache.ConvertAll(x => PCD.GetColorBlockAsset(x));

        [MemoryPackIgnore]
        public abstract string BlockTypeStr { get; }
        public abstract void SetPointers(PgbepManager pgbepManager);

        public static IPGBFuncUnion GetFuncParInstance(int i)
        {
            return (IPGBFuncUnion)Activator.CreateInstance(AddablePGBTypes[i]);
        }

        public int GetFuncParNum => AddablePGBTypes.IndexOf(GetType());

        [MemoryPackIgnore]
        public virtual int calcCost => 0;
        [MemoryPackIgnore]
        public virtual bool OpenEditorOnCreateNode => true;
        public virtual bool IsConnectable(IPGBFuncUnion connectionFrom)
        {
            return true;
        }
        public virtual string[] GetNodeFaceText()
        {
            return null;
        }
        public virtual float?[] GetNodeFaceValue()
        {
            return null;
        }
        public virtual StopActionType? GetNodeFaceStopActionType()
        {
            return null;
        }
        public virtual long? GetNodeFaceSelectedWeapons()
        {
            return null;
        }
        public virtual int? GetNodeFaceWeaponIcon()
        {
            return null;
        }

        public float CalcHorizontalAngle(Vector3 v)
        {
            return Vector3.SignedAngle(Vector3.forward, Vector3.ProjectOnPlane(v, Vector3.up), Vector3.up);
        }
        public float CalcVerticalAngle(Vector3 v)
        {
            if (v.x == 0 && v.z == 0)
            {
                return v.y switch
                {
                    0 => 0,
                    > 0 => 90,
                    < 0 => -90
                };
            }
            var hv = new Vector3(v.x, 0, v.z);
            var hvc = Vector3.Cross(v, Vector3.up);
            return Vector3.SignedAngle(hv, v, hvc);
        }
        public virtual IFieldEditObject GetNodeFaceIFieldEditObject()
        {
            return null;
        }
    }
}