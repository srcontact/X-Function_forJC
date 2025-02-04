using clrev01.PGE.PGBEditor;
using clrev01.Programs.FieldPar;
using clrev01.Programs.FuncPar.Comment;
using MemoryPack;

namespace clrev01.Programs.FuncPar.FuncParType
{
    [MemoryPackable()]
    [MemoryPackUnion(0, typeof(StartFuncPar))]
    [MemoryPackUnion(1, typeof(MoveFuncPar))]
    [MemoryPackUnion(2, typeof(JumpFuncPar))]
    [MemoryPackUnion(3, typeof(RotateFuncPar))]
    [MemoryPackUnion(4, typeof(FireFuncPar))]
    [MemoryPackUnion(5, typeof(LockOnFuncPar))]
    [MemoryPackUnion(6, typeof(SearchFuncPar))]
    [MemoryPackUnion(7, typeof(AssessTargetPosFuncPar))]
    [MemoryPackUnion(8, typeof(AssessSelfActionFuncPar))]
    [MemoryPackUnion(9, typeof(BreakFuncPar))]
    [MemoryPackUnion(10, typeof(StopProgramFuncPar))]
    [MemoryPackUnion(11, typeof(StopActionFuncPar))]
    [MemoryPackUnion(12, typeof(AssessSelfStatusFuncPar))]
    [MemoryPackUnion(13, typeof(AssessLineHittingFuncPar))]
    [MemoryPackUnion(14, typeof(CalcFuncPar))]
    [MemoryPackUnion(15, typeof(AssessVariableFuncPar))]
    [MemoryPackUnion(16, typeof(AssessAreaLimitFuncPar))]
    [MemoryPackUnion(17, typeof(ThrustFuncPar))]
// [MemoryPackUnion(18, typeof(GetStatusValueFuncPar))]
    [MemoryPackUnion(19, typeof(DashFuncPar))]
    [MemoryPackUnion(20, typeof(SubroutineRootFuncPar))]
    [MemoryPackUnion(21, typeof(SubroutineExecuteFuncPar))]
    [MemoryPackUnion(22, typeof(FightFuncPar))]
    [MemoryPackUnion(23, typeof(AssessSelfPosFuncPar))]
    [MemoryPackUnion(24, typeof(AssessTargetActionFuncPar))]
    [MemoryPackUnion(26, typeof(CalcVector3dFuncPar))]
    [MemoryPackUnion(27, typeof(ShieldFuncPar))]
    [MemoryPackUnion(28, typeof(AimFuncPar))]
    [MemoryPackUnion(29, typeof(AssessMoveDirectionFuncPar))]
    [MemoryPackUnion(30, typeof(AssessTargetStatusFuncPar))]
    [MemoryPackUnion(31, typeof(AssessNumberOfAimingMeFuncPar))]
    [MemoryPackUnion(32, typeof(AssessLockOnNumber))]
    [MemoryPackUnion(33, typeof(GetBattleStatusValueFuncPar))]
    [MemoryPackUnion(34, typeof(GetSelfStatusValueFuncPar))]
    [MemoryPackUnion(35, typeof(GetTargetStatusValueFuncPar))]
    [MemoryPackUnion(36, typeof(GetEnvironmentStatusValueFuncPar))]
    [MemoryPackUnion(37, typeof(AssessTargetTypeFuncPar))]
    [MemoryPackUnion(38, typeof(NopFuncPar))]
    [MemoryPackUnion(39, typeof(CommentFuncPar))]
    [MemoryPackUnion(40, typeof(GetListStatusFuncPar))]
    [MemoryPackUnion(41, typeof(GetTargetStatusListFuncPar))]
    [MemoryPackUnion(42, typeof(CalcNumericListFuncPar))]
    [MemoryPackUnion(43, typeof(CalcVector3dListFuncPar))]
    [MemoryPackUnion(44, typeof(ManageLockOnFuncPar))]
    [MemoryPackUnion(45, typeof(LockOnListRemoveNullFuncPar))]
    [MemoryPackUnion(46, typeof(UseOptionalFuncPar))]
    [MemoryPackUnion(47, typeof(DefenceFuncPar))]
    public partial interface IPGBFuncUnion
    {
        string BlockTypeStr { get; }
        void SetPointers(PgbepManager pgbepManager);
        int GetFuncParNum { get; }
        int calcCost { get; }
        bool OpenEditorOnCreateNode { get; }
        bool IsConnectable(IPGBFuncUnion connectionFrom);
        string[] GetNodeFaceText();
        float?[] GetNodeFaceValue();
        StopActionType? GetNodeFaceStopActionType();
        long? GetNodeFaceSelectedWeapons();
        int? GetNodeFaceWeaponIcon();
        IFieldEditObject GetNodeFaceIFieldEditObject();
    }
}