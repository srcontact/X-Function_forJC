using RootMotion.FinalIK;
using System.Collections.Generic;

namespace clrev01.Bases
{
    public interface IIkUser
    {
        public List<IK> ikList { get; }

        public void InitIkOnAwake()
        {
            if (ikList is null) return;
            foreach (var ik in ikList)
            {
                ik.enabled = false;
            }
        }

        public void FixTransformsIk()
        {
            if (ikList is null) return;
            foreach (var ik in ikList)
            {
                var ikSolver = ik.GetIKSolver();
                ikSolver.FixTransforms();
            }

        }

        public void UpdateIk()
        {
            if (ikList is null) return;
            foreach (var ik in ikList)
            {
                var ikSolver = ik.GetIKSolver();
                ikSolver.Update();
            }
        }
    }
}