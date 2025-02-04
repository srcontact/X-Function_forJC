using clrev01.Menu;
using clrev01.Programs;
using clrev01.Save.VariableData;
using System;
using TMPro;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class PgbeDirectionPanel : PgbePanel
    {
        [SerializeField]
        protected MenuButton hMainButton, vMainButton;
        [SerializeField]
        TMP_InputField horizontalAngleInput, VerticalAngleInput;
        [SerializeField]
        private GameObject figureHBase, figureVBase;
        [SerializeField]
        Transform figureHObj, figureVObj;
        [SerializeField]
        GameObject hPanel, vPanel;
        [SerializeField, Range(0, 360)]
        float figureHInit = -90, figureVInit = 180;
        [SerializeField]
        protected TextMeshProUGUI hVariableText, vVariableText;
        [SerializeField]
        protected MenuButton hModeChangeButton, vModeChangeButton;
        [SerializeField]
        protected TextMeshProUGUI hModeChangeButtonText, vModeChangeButtonText;
        protected VariableDataNumericGet hVariableData, vVariableData;


        private AngleLimit hAngleLimit, vAngleLimit;

        private unsafe void Awake()
        {
            horizontalAngleInput.onEndEdit.AddListener((string s) => OnEndEdit(s, hVariableData, hAngleLimit));
            VerticalAngleInput.onEndEdit.AddListener((string s) => OnEndEdit(s, vVariableData, vAngleLimit));
            hModeChangeButton.OnClick.AddListener(() =>
            {
                if (hVariableData == null) return;
                if (hVariableData.useVariable)
                {
                    hVariableData.useVariable = false;
                    initPGBEPM.Invoke();
                }
                else
                {
                    PGEM2.variableEditor.OpenEditorAddAndSelect(hVariableData, initPGBEPM);
                }
            });
            hMainButton.OnClick.AddListener(() =>
            {
                if (hVariableData is { useVariable: true }) PGEM2.variableEditor.OpenEditorAddAndSelect(hVariableData, initPGBEPM);
            });
            vModeChangeButton.OnClick.AddListener(() =>
            {
                if (vVariableData == null) return;
                if (vVariableData.useVariable)
                {
                    vVariableData.useVariable = false;
                    initPGBEPM.Invoke();
                }
                else
                {
                    PGEM2.variableEditor.OpenEditorAddAndSelect(vVariableData, initPGBEPM);
                }
            });
            vMainButton.OnClick.AddListener(() =>
            {
                if (vVariableData is { useVariable: true }) PGEM2.variableEditor.OpenEditorAddAndSelect(vVariableData, initPGBEPM);
            });
        }
        public unsafe void OnOpen(string title, AngleLimit hAngleLimit = null, AngleLimit vAngleLimit = null, VariableDataNumericGet hVd = null, VariableDataNumericGet vVd = null)
        {
            if (titleLabel != null) titleLabel.text = title;

            this.hAngleLimit = hAngleLimit;
            var hVariableActive = hVd != null;
            hModeChangeButton.gameObject.SetActive(hVariableActive);
            if (hVariableActive) hVariableData = hVd;

            this.vAngleLimit = vAngleLimit;
            var vVariableActive = vVd != null;
            vModeChangeButton.gameObject.SetActive(vVariableActive);
            if (vVariableActive) vVariableData = vVd;

            SetIndicate(hVariableData, horizontalAngleInput, hVariableText, hModeChangeButtonText, figureHBase, figureHObj, figureHInit, hPanel, this.hAngleLimit);
            SetIndicate(vVariableData, VerticalAngleInput, vVariableText, vModeChangeButtonText, figureVBase, figureVObj, figureVInit, vPanel, this.vAngleLimit);
        }
        private unsafe void OnEndEdit(string s, VariableDataNumericGet vdn, AngleLimit angleLimit)
        {
            if (float.TryParse(s, out var f)) vdn.constValue = angleLimit?.CalcLimitedAngle(f) ?? f;
            SetIndicate(hVariableData, horizontalAngleInput, hVariableText, hModeChangeButtonText, figureHBase, figureHObj, figureHInit, hPanel, this.hAngleLimit);
            SetIndicate(vVariableData, VerticalAngleInput, vVariableText, vModeChangeButtonText, figureVBase, figureVObj, figureVInit, vPanel, this.vAngleLimit);
            initPGBEPM.Invoke();
        }
        unsafe void SetIndicate
            (VariableDataNumericGet vdn, TMP_InputField inputField, TextMeshProUGUI variableText, TextMeshProUGUI modeChangeText, GameObject figureBase, Transform fig, float figInitRot, GameObject indPanel, AngleLimit angleLimit)
        {
            if (vdn is { useVariable: true })
            {
                indPanel.SetActive(true);
                inputField.gameObject.SetActive(false);
                figureBase.SetActive(false);
                fig.gameObject.SetActive(false);
                variableText.gameObject.SetActive(true);

                variableText.text = vdn.name;
                modeChangeText.text = "(n)";
            }
            else if (vdn != null)
            {
                indPanel.SetActive(true);
                inputField.gameObject.SetActive(true);
                figureBase.SetActive(true);
                fig.gameObject.SetActive(true);
                variableText.gameObject.SetActive(false);

                var angle = angleLimit?.CalcLimitedAngle(vdn.constValue) ?? vdn.constValue;
                inputField.text = angle.ToString();
                Vector3 r = fig.localRotation.eulerAngles;
                r.z = figInitRot + angle;
                fig.localRotation = Quaternion.Euler(r);
                modeChangeText.text = "(X)";
            }
            else
            {
                indPanel.SetActive(false);
            }
        }
        protected override unsafe void ResetTgtData()
        {
            hVariableData = null;
            vVariableData = null;
        }
    }
}