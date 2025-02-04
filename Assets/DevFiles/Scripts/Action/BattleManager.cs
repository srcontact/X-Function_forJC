using clrev01.Bases;
using clrev01.ClAction.Bullets;
using clrev01.ClAction.Machines;
using clrev01.ClAction.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction
{
    public class BattleManager : BaseOfCL
    {
        [SerializeField]
        private MatchStatusIndicator matchStatusIndicator;
        [SerializeField]
        private GameObject actionNormalUI;
        [SerializeField]
        private int endDelayFrame = 120;

        [ReadOnly]
        public bool endAction;
        public int actionEndFrame => StaticInfo.Inst.actionEndFrame;

        [ShowInInspector, ReadOnly]
        private bool _notEndInThisFrame = false;
        private int _lastCheckedMachineTeamId = -1;
        private int _lastCheckedBulletTeamId = -1;
        private int _endDelayCount = 0;

        public void CheckMachineState(MachineHD machine)
        {
            if (_lastCheckedMachineTeamId != machine.teamID && _lastCheckedMachineTeamId != -1) _notEndInThisFrame = true;
            _lastCheckedMachineTeamId = machine.teamID;
        }
        public void CheckBulletState(BulletHD bullet)
        {
            if (_lastCheckedBulletTeamId != bullet.teamID && _lastCheckedBulletTeamId != -1) _notEndInThisFrame = true;
            _lastCheckedBulletTeamId = bullet.teamID;
        }

        public void CheckInitialize()
        {
            _notEndInThisFrame = false;
            _lastCheckedMachineTeamId = -1;
            _lastCheckedBulletTeamId = -1;
        }

        public void AggregateEndAction()
        {
            if (!_notEndInThisFrame)
            {
                if (_endDelayCount >= endDelayFrame) endAction = true;
                else _endDelayCount++;
            }
            AggregateTime();
            if (endAction)
            {
                matchStatusIndicator.gameObject.SetActive(true);
                actionNormalUI.SetActive(false);
            }
        }

        public void AggregateTime()
        {
            if (ACM.actionFrame > actionEndFrame) endAction = true;
        }
    }
}