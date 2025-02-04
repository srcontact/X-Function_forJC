using clrev01.Bases;
using clrev01.Menu;
using clrev01.Programs.FuncPar;
using clrev01.Programs.FuncPar.Comment;
using clrev01.Programs.FuncPar.FuncParType;
using System;
using UnityEngine;

namespace clrev01.Programs
{
    [CreateAssetMenu(menuName = "ProgramCommonData")]
    public class ProgramCommonData : SOBaseOfCL
    {
        [SerializeField]
        private ColorBlockAsset startColorInfo, actionColorInfo, perceptionColorInfo, branchColorInfo, subroutineColorInfo, commentColorInfo;
        [SerializeField]
        private ColorBlockAsset toggledStartColorInfo, toggledActionColorInfo, toggledPerceptionColorInfo, toggledBranchColorInfo, toggledSubroutineColorInfo, toggledCommentColorInfo;


        public (ColorBlockAsset cba, ColorBlockAsset cbat) GetColorBlockAsset(IPGBFuncUnion funcPar)
        {
            return funcPar switch
            {
                StartFuncPar => (startColorInfo, toggledStartColorInfo),
                ActionFuncPar => (actionColorInfo, toggledActionColorInfo),
                FunctionFuncPar => (perceptionColorInfo, toggledPerceptionColorInfo),
                BranchFuncPar => (branchColorInfo, toggledBranchColorInfo),
                SubroutineFuncPar => (subroutineColorInfo, toggledSubroutineColorInfo),
                CommentFuncPar => (commentColorInfo, toggledCommentColorInfo),
                _ => throw new ArgumentOutOfRangeException(nameof(funcPar))
            };
        }
    }
}