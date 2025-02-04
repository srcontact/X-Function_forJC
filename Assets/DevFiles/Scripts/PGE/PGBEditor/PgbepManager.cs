using clrev01.Bases;
using clrev01.Extensions;
using clrev01.PGE.PGBEditor.PGBEDetailMenu;
using clrev01.PGE.PGBEditor.PGBEPanel;
using clrev01.PGE.VariableEditor;
using clrev01.Programs;
using clrev01.Programs.FieldPar;
using clrev01.Save;
using clrev01.Save.VariableData;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGBEditor
{
    public class PgbepManager : BaseOfCL
    {
        [SerializeField]
        PGBEditor pGBEditor;
        [SerializeField]
        Transform pgbepParent;
        [SerializeField]
        PgbeHeaderText headerTextOrig;
        [SerializeField]
        PgbeSlider sliderOrig;
        [SerializeField]
        PgbeSeparator separatorOrig;
        [SerializeField]
        PgbeIntPanel intPanelOrig;
        [SerializeField]
        PgbeFloatPanel floatPanelOrig;
        [SerializeField]
        PgbeVector3Panel vector3PanelOrig;
        [SerializeField]
        PgbeEnumPanel enumPanelOrig;
        [SerializeField]
        PgbeEnumFlagsPanel enumFlagsPanelOrig;
        [SerializeField]
        PgbeTogglePanel togglePanelOrig;
        [SerializeField]
        PgbeDirectionPanel directionPanelOrig;
        [SerializeField]
        PgbeFieldPanel fieldPanelOrig;
        [SerializeField]
        PgbeStringPanel stringPanelOrig, commentPanelOrig;
        [SerializeField]
        PgbeVariablePanel variablePanelOrig;
        [SerializeField]
        PgbeFieldEditDetail fieldEditDetail;
        [SerializeField]
        private Scrollbar pgbeScrollbar;

        Dictionary<int, List<PgbePanel>> pgbeps
            = new Dictionary<int, List<PgbePanel>>();

        [NonSerialized]
        public Action initOnEditAction;

        public PGBData editDataOrig => pGBEditor.pGBDataOrig;

        public void ResetPgbeps(bool keepScroll)
        {
            var verticalScroll = pGBEditor.scrollRect.verticalScrollbar.value;
            foreach (int id in pgbeps.Keys)
            {
                foreach (PgbePanel pgbePanel in pgbeps[id])
                {
                    if (pgbePanel.gameObject.activeSelf)
                    {
                        pgbePanel.OnReset();
                    }
                }
            }
            if (keepScroll)
            {
                UniTask.Create(async () =>
                {
                    await UniTask.Yield();
                    pGBEditor.scrollRect.verticalScrollbar.value = verticalScroll;
                }).Forget();
            }
        }

        public void SetHeaderText(string text, string link = null)
        {
            var np = GetPgbep(headerTextOrig);
            np.OnOpen(text.LinkTag(link));
        }
        public void SetSeparator()
        {
            GetPgbep(separatorOrig);
        }
        public unsafe void SetPgbepInt(float* data, IntSliderSettingPar sliderSettingPar = null, VariableData vd = null, List<VariableType> selectableVariableTypes = null)
        {
            if (sliderSettingPar != null)
            {
                var clamp = *data;
                if (sliderSettingPar.limitMinFlag) clamp = Mathf.Max(clamp, sliderSettingPar.sliderMin);
                if (sliderSettingPar.limitMaxFlag) clamp = Mathf.Min(clamp, sliderSettingPar.sliderMax);
                *data = clamp;
            }
            var np = GetPgbep(intPanelOrig);
            np.OnPgbeOpen("", data, vd, selectableVariableTypes);
            if (sliderSettingPar != null)
            {
                var slider = GetPgbep(sliderOrig);
                np.SetSlider(slider.Slider, sliderSettingPar.sliderMin, sliderSettingPar.sliderMax);
            }
        }
        public unsafe void SetPgbepFloat(float* data, FloatSliderSettingPar sliderSettingPar = null, VariableData vd = null, List<VariableType> selectableVariableTypes = null)
        {
            if (sliderSettingPar != null)
            {
                var clamp = *data;
                if (sliderSettingPar.limitMinFlag) clamp = Mathf.Max(clamp, sliderSettingPar.sliderMin);
                if (sliderSettingPar.limitMaxFlag) clamp = Mathf.Min(clamp, sliderSettingPar.sliderMax);
                *data = clamp;
            }
            var np = GetPgbep(floatPanelOrig);
            np.OnPgbeOpen("", data, vd, selectableVariableTypes);
            if (sliderSettingPar != null)
            {
                var slider = GetPgbep(sliderOrig);
                np.SetSlider(slider.Slider, sliderSettingPar.sliderMin, sliderSettingPar.sliderMax, sliderSettingPar.sliderInputUnit);
            }
        }
        public unsafe void SetPgbepVector3(Vector3* data, IReadOnlyList<bool> activeList = null, VariableData vd = null, List<VariableType> selectableVariableTypes = null)
        {
            var np = GetPgbep(vector3PanelOrig);
            np.OnPgbeOpen("", data, vd, selectableVariableTypes);
            np.SetInputFieldsActive(activeList);
        }
        public unsafe void SetPgbepEnum(Type enumType, int* data, params int[] ignoreList)
        {
            var np = GetPgbep(enumPanelOrig);
            np.OnOpenFromEnum("", data, enumType, ignoreList);
        }
        public unsafe void SetPgbepFlagsEnum(Type enumType, long* data, string separator = " & ")
        {
            var np = GetPgbep(enumFlagsPanelOrig);
            np.OnOpenFromEnum("", enumType, data, separator);
        }
        public unsafe void SetPgbepSelectOptions(int* data, List<string> options)
        {
            var np = GetPgbep(enumPanelOrig);
            np.OnOpenFromList("", data, options);
        }
        public unsafe void SetPgbepSelectFlagsOption(long* data, List<string> options, string separator = " & ")
        {
            var np = GetPgbep(enumFlagsPanelOrig);
            np.OnOpenFromList("", options.ToArray(), data, separator);
        }
        public List<string> GetEquipmentList()
        {
            var weapons = PGEM2.nowEditCD.mechCustom.weapons.ConvertAll(x => WHUB.GetBulletName(x));
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i] = $"{i + 1}:{weapons[i]}";
            }
            return weapons;
        }
        public List<string> GetOptionalList()
        {
            var optionals = PGEM2.nowEditCD.mechCustom.optionParts.ConvertAll(x => OpHub.GetOptionPartsData(x).partsName);
            for (int i = 0; i < optionals.Count; i++)
            {
                optionals[i] = $"{i + 1}:{optionals[i]}";
            }
            return optionals;
        }
        public unsafe void SetPgbepToggle(bool* data)
        {
            var np = GetPgbep(togglePanelOrig);
            np.OnPgbeOpen("", data);
        }
        public unsafe void SetPgbepString(StringRefObj data, Action formatAfterInput, TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard)
        {
            var np = GetPgbep(stringPanelOrig);
            np.OnPgbeOpen(data, formatAfterInput, contentType, pGBEditor.scrollRect);
        }
        public unsafe void SetPgbepComment(StringRefObj data, Action formatAfterInput, TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard)
        {
            var np = GetPgbep(commentPanelOrig);
            np.OnPgbeOpen(data, formatAfterInput, contentType, pGBEditor.scrollRect);
        }
        public unsafe void SetPgbepSubroutine(int* data, Dictionary<int, string> subroutineRootDict)
        {
            var np = GetPgbep(enumPanelOrig);
            np.OnOpenFromList("", data, subroutineRootDict.Values.ToList(), subroutineRootDict.Keys.ToList());
        }
        public unsafe void SetPgbepAngle(AngleLimit hAngleLimit = null, AngleLimit vAngleLimit = null, VariableDataNumericGet hVd = null, VariableDataNumericGet vVd = null)
        {
            var np = GetPgbep(directionPanelOrig);
            np.OnOpen("", hAngleLimit, vAngleLimit, hVd, vVd);
        }

        public unsafe void SetPgbepField(IFieldEditObject haveFieldFuncPar, Action<IFieldEditObject> resultSetAction, bool ignore3D = false, bool ignoreSphere = false)
        {
            var np = GetPgbep(fieldPanelOrig);
            np.OnOpen("", haveFieldFuncPar, resultSetAction, fieldEditDetail, ignore3D, ignoreSphere);
        }

        public void SetPgbepVariable(VariableData vd, bool indicateModeChangeButton, List<VariableType> selectableVariableTypes = null)
        {
            PGEM2.variableEditor.ResearchVariableData();
            vd.name ??= PGEM2.variableEditor.GetDefaultVariableName(vd.variableType);
            var np = GetPgbep(variablePanelOrig);
            np.OnOpen(vd, indicateModeChangeButton, selectableVariableTypes);
        }

        T GetPgbep<T>(T orig) where T : PgbePanel
        {
            var t = orig.GetInstanceID();
            if (!pgbeps.ContainsKey(t))
            {
                pgbeps.Add(t, new List<PgbePanel>());
            }
            T npp = null;
            foreach (PgbePanel pgbePanel in pgbeps[t])
            {
                if (pgbePanel.gameObject.activeSelf) continue;
                else
                {
                    pgbePanel.gameObject.SetActive(true);
                    npp = (T)pgbePanel;
                    break;
                }
            }
            if (npp == null)
            {
                npp = orig.SafeInstantiate();
                pgbeps[t].Add(npp);
                npp.transform.SetParent(pgbepParent);
                npp.initPGBEPM += initOnEditAction;
            }
            npp.transform.SetAsLastSibling();
            Vector3 p = npp.lpos;
            p.z = 0;
            npp.lpos = p;
            npp.scl = Vector3.one;
            return npp;
        }

        public class FloatSliderSettingPar
        {
            public readonly float sliderMin;
            public readonly float sliderMax;
            public readonly int sliderInputUnit;
            public readonly bool limitMinFlag;
            public readonly bool limitMaxFlag;

            public FloatSliderSettingPar(float min, float max, int unit = 1, bool limitMin = true, bool limitMax = true)
            {
                sliderMin = min;
                sliderMax = max;
                sliderInputUnit = unit;
                limitMinFlag = limitMin;
                limitMaxFlag = limitMax;
            }
        }

        public class IntSliderSettingPar
        {
            public readonly int sliderMin;
            public readonly int sliderMax;
            public readonly bool limitMinFlag;
            public readonly bool limitMaxFlag;

            public IntSliderSettingPar(int min, int max, bool limitMin = true, bool limitMax = true)
            {
                sliderMin = min;
                sliderMax = max;
                limitMinFlag = limitMin;
                limitMaxFlag = limitMax;
            }
        }
    }
}