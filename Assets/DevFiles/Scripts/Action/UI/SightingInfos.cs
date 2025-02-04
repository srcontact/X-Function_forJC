using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.ClAction.Machines.Motion;
using clrev01.Extensions;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using clrev01.ClAction.ActionCamera;
using UnityEngine;

namespace clrev01.ClAction.UI
{
    public class SightingInfos : BaseOfCL
    {
        [SerializeField]
        private SightingTargetSymbol origSightingTargetSymbol;
        [ReadOnly]
        private List<SightingTargetSymbol> _sightingTgts = new();

        [SerializeField]
        private SightingTargetSymbol origAimPointSymbol;
        [ReadOnly]
        private List<SightingTargetSymbol> _aimPoints = new();

        [SerializeField]
        private SightingTargetSymbol origObjectContainerSymbol;
        [ReadOnly]
        private List<SightingTargetSymbol> _objectContainers = new();
        [SerializeField]
        private Color friendColor, enemyColor;

        private Camera _cameraMain;
        private Transform _cameraTransform;


        private void Awake()
        {
            for (int i = 0; i < 10; i++)
            {
                var oc = origObjectContainerSymbol.SafeInstantiate(i.ToString("00"));
                oc.transform.SetParent(transform, false);
                oc.gameObject.SetActive(false);
                _objectContainers.Add(oc);
            }
            for (int i = 0; i < 10; i++)
            {
                var st = origSightingTargetSymbol.SafeInstantiate(i.ToString("00"));
                st.transform.SetParent(transform, false);
                st.gameObject.SetActive(false);
                st.UpdateText((i > 0 ? string.Concat(Enumerable.Repeat("\n", i - 1)) : "") + i.ToString("0"));
                _sightingTgts.Add(st);
            }
            for (int i = 0; i < 20; i++)
            {
                var ap = origAimPointSymbol.SafeInstantiate(i.ToString("00"));
                ap.transform.SetParent(transform, false);
                ap.gameObject.SetActive(false);
                _aimPoints.Add(ap);
            }
        }

        public void UpdateObjectContainer(List<MachineHD> machines, Vector3 cameraTgtPos, int nowSelectNum)
        {
            _cameraMain ??= Camera.main;
            _cameraTransform ??= _cameraMain.transform;
            for (int i = 0; i < _objectContainers.Count; i++)
            {
                var oc = _objectContainers[i];
                if (
                    i >= machines.Count ||
                    (StaticInfo.Inst.ActionSettingData.cameraMode is not CameraMode.LookingDown && i == nowSelectNum) ||
                    !machines[i].gameObject.activeSelf ||
                    _cameraTransform.InverseTransformPoint(machines[i].pos).z < 0
                )
                {
                    if (oc.gameObject.activeSelf) oc.gameObject.SetActive(false);
                    continue;
                }
                if (!oc.gameObject.activeSelf) oc.gameObject.SetActive(true);
                var m = machines[i];
                oc.UpdateText($"Team{m.teamID:00}-{m.machineIdInTeam:00}\n{m.ld.customData.dataName}\n{Vector3.Distance(m.pos, cameraTgtPos):0.00}m");
                oc.UpdatePos(RectTransformUtility.WorldToScreenPoint(_cameraMain, machines[i].pos));
                oc.UpdateColor(m.teamID == machines[nowSelectNum].teamID ? friendColor : enemyColor);
            }
        }
        public void UpdateSight(List<BaseAimIkPar> aimPars, Transform shooterTransform)
        {
            _cameraMain ??= Camera.main;
            _cameraTransform ??= _cameraMain.transform;
            for (int i = 0; i < _sightingTgts.Count; i++)
            {
                var st = _sightingTgts[i];
                var ap = _aimPoints[i];
                if (i >= aimPars.Count) continue;
                var aimIkPar = aimPars[i];
                var tgtPosGlobal = aimIkPar.wantToAimPosGlobal;
                if (aimIkPar.correspondingWeaponNum < 0 || !aimIkPar.nowAiming || (_cameraTransform.InverseTransformPoint(tgtPosGlobal).z < 0 && _cameraTransform.InverseTransformPoint(aimIkPar.ikPos).z < 0))
                {
                    if (st.gameObject.activeSelf) st.gameObject.SetActive(false);
                    if (ap.gameObject.activeSelf) ap.gameObject.SetActive(false);
                    continue;
                }
                if (!st.gameObject.activeSelf) st.gameObject.SetActive(true);
                if (!ap.gameObject.activeSelf) ap.gameObject.SetActive(true);
                st.UpdatePos(RectTransformUtility.WorldToScreenPoint(_cameraMain, tgtPosGlobal));
                ap.UpdatePos(RectTransformUtility.WorldToScreenPoint(_cameraMain, aimIkPar.ikPos));
                var weaponNumStr = (i > 0 ? string.Concat(Enumerable.Repeat("\n", i - 1)) : "") + i.ToString("0");
                var posAccuracyStr = aimIkPar.PosAccuracy?.ToString("0.##m");
                st.UpdateText(weaponNumStr, posAccuracyStr);
            }
        }
    }
}