using clrev01.Bases;
using clrev01.Menu;
using clrev01.Menu.CycleScroll;
using clrev01.Menu.Dialog;
using clrev01.Programs;
using clrev01.Save;
using clrev01.Save.VariableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.VariableEditor
{
    public class VariableEditor : BaseOfCL
    {
        [SerializeField]
        private CycleScroll cycleScroll;
        [SerializeField]
        private TMP_InputField addInputField;
        [SerializeField]
        private MenuButton addButton, typeChangeButton;
        [SerializeField]
        private TextMeshProUGUI typeText;
        [SerializeField]
        private GameObject addAreaObj;
        [SerializeField]
        private BackPanel backPanel;
        private readonly List<VariableIndicater> _indicators = new();
        private PGData pGData => StaticInfo.Inst.nowEditMech.program;
        private List<string> _searchedVariables = new();
        private readonly Dictionary<string, List<VariableData>> _searchedVariablesDict = new();

        private VariableType? _currentEditVariableType;
        private VariableType? CurrentEditVariableType
        {
            get => _currentEditVariableType;
            set
            {
                _currentEditVariableType = value;
                typeText.text = _currentEditVariableType.ToString();
            }
        }

        private PGBData nowEditPgbDataOrig => PGEM2.editMenu.pGBDataOrig;
        private PGBData nowEditPgbDataCopy => PGEM2.editMenu.pgbDataCopy;
        private VariableData _editTargetVariableData;
        private readonly List<VariableType> _selectableVariableTypes = new();
        private Action _initEditorAction;


        private void Awake()
        {
            var initList = cycleScroll.Initialize(c => CycleScrollPanelSettingAction(c));
            _indicators.AddRange(initList.ConvertAll(x => x.GetComponentInChildren<VariableIndicater>()));
            foreach (var item in _indicators)
            {
                item.variableEditor = this;
                item.cycleScrollPanel.eventOnSelect.AddListener(csp =>
                {
                    if (_editTargetVariableData != null)
                    {
                        var newName = _searchedVariables[csp.itemId];
                        ApplyChanges(newName, CurrentEditVariableType.GetValueOrDefault());
                    }
                    else
                    {
                        item.inputField.gameObject.SetActive(true);
                        item.inputField.Select();
                    }
                });
                item.inputField.onEndEdit.AddListener(text =>
                {
                    var isValid = StaticInfo.Inst.UndoManager.UpdatePgbdStart();
                    string currentName = item.variableName;
                    var updateNameTgts = _searchedVariablesDict[item.variableName];
                    foreach (var unt in updateNameTgts)
                    {
                        unt.name = text;
                    }
                    StaticInfo.Inst.UndoManager.UpdatePgbdLog(isValid, updateNameTgts.ConvertAll(x => x.ownerDara.editorPar.myIndex));
                    StaticInfo.Inst.UndoManager.UpdatePgbdEnd(isValid);
                    ResearchVariableData();
                    cycleScroll.UpdatePage(_searchedVariables.Count);
                });
            }
            addButton.OnClick.AddListener(() => AddExecution());
            if (typeChangeButton != null) typeChangeButton.OnClick.AddListener(() => TypeChange(typeChangeButton));
            backPanel.onClick.AddListener(() => CloseEditor());
        }
        private void ApplyChanges(string applyName, VariableType applyVariableType)
        {
            _editTargetVariableData.name = applyName;
            _editTargetVariableData.useVariable = true;
            _editTargetVariableData.variableType = applyVariableType;
            CloseEditor();
            _initEditorAction.Invoke();
        }

        public void ResearchVariableData()
        {
            _searchedVariables.Clear();
            _searchedVariablesDict.Clear();
            var fieldsCache = new Dictionary<Type, FieldInfo[]>();
            foreach (var pdInList in pGData.pgList)
            {
                if (pdInList == null) continue;
                PGBData pd;
                if (_editTargetVariableData != null && pdInList == nowEditPgbDataOrig) pd = nowEditPgbDataCopy;
                else pd = pdInList;

                var funcPar = pd.funcPar;
                FieldInfo[] fields;
                if (fieldsCache.ContainsKey(funcPar.GetType()))
                {
                    fields = fieldsCache[funcPar.GetType()];
                }
                else
                {
                    fields = funcPar.GetType().GetFields();
                    fieldsCache.Add(funcPar.GetType(), fields);
                }
                foreach (var f in fields)
                {
                    if (f.FieldType != typeof(VariableData) && !f.FieldType.IsSubclassOf(typeof(VariableData))) continue;
                    var vd = (VariableData)f.GetValue(funcPar);
                    if (!vd.useVariable || vd.name == null) continue;
                    vd.ownerDara = pd;
                    if (_searchedVariablesDict.TryGetValue(vd.name, out var value))
                    {
                        value.Add(vd);
                    }
                    else
                    {
                        _searchedVariablesDict.Add(vd.name, new List<VariableData>() { vd });
                    }
                }
            }
            _searchedVariables = _searchedVariablesDict.Where(x => x.Value[0].variableType == CurrentEditVariableType).ToList().ConvertAll(x => x.Key);
            _searchedVariables = _searchedVariables.OrderBy(x => x).ToList();
        }

        private void CycleScrollPanelSettingAction(CycleScrollPanel c)
        {
            if (c.itemId >= _searchedVariables.Count)
            {
                _indicators[c.panelId].HideSetting();
            }
            else
            {
                _indicators[c.panelId].ShowSetting(_searchedVariables[c.itemId]);
            }
        }

        /// <summary>
        /// 変数エディタを開く
        /// </summary>
        public void OpenEditor()
        {
            CurrentEditVariableType ??= VariableType.Numeric;
            _selectableVariableTypes.AddRange((VariableType[])Enum.GetValues(typeof(VariableType)));
            gameObject.SetActive(true);
            typeChangeButton.interactable = true;
            addAreaObj.SetActive(false);
            ResearchVariableData();
            cycleScroll.UpdatePage(_searchedVariables.Count);
        }

        /// <summary>
        /// 変数エディタを開く
        /// </summary>
        /// <param name="variableData"></param>
        /// <param name="initEditorAction"></param>
        /// <param name="selectableVariableTypes"></param>
        public void OpenEditorAddAndSelect(VariableData variableData, Action initEditorAction, List<VariableType> selectableVariableTypes = null)
        {
            _editTargetVariableData = variableData;
            CurrentEditVariableType = variableData.variableType;
            _selectableVariableTypes.AddRange(selectableVariableTypes ?? variableData.selectableVariableTypes);
            _initEditorAction = initEditorAction;
            gameObject.SetActive(true);
            typeChangeButton.interactable = _selectableVariableTypes.Count > 1;
            addAreaObj.SetActive(true);
            ResearchVariableData();
            cycleScroll.UpdatePage(_searchedVariables.Count);
            ResetAddInputField(variableData);
        }

        private void ResetAddInputField(VariableData variableData)
        {
            if (variableData == null) return;
            addInputField.SetTextWithoutNotify(variableData.name != null && variableData.variableType == CurrentEditVariableType ? variableData.name : GetDefaultVariableName(CurrentEditVariableType.GetValueOrDefault()));
        }

        private void CloseEditor()
        {
            _editTargetVariableData = null;
            _selectableVariableTypes.Clear();
            gameObject.SetActive(false);
        }

        public string GetDefaultVariableName(VariableType variableType)
        {
            var defaultVariableName = variableType.ToString();
            var i = 1;
            while (true)
            {
                if (_searchedVariables.All(x => x != defaultVariableName + i.ToString("00"))) break;
                i++;
            }
            return defaultVariableName + i.ToString("00");
        }

        private void AddExecution()
        {
            if (_searchedVariablesDict.ContainsKey(addInputField.text))
            {
                MenuPagePanelManager.Inst.dialogManager.simpleDialog.OpenSimpleDialogCloseOnTouchBack($"'{addInputField.text}' is already registered.", new[] { "OK" });
                return;
            }
            ApplyChanges(addInputField.text, CurrentEditVariableType.GetValueOrDefault());
        }

        private void TypeChange(MenuButton button)
        {
            Vector3 sp = Camera.main.WorldToScreenPoint(button.transform.position);
            PGEM2.quickMenuDialog.OpenQuickMenu(_selectableVariableTypes.ConvertAll(x => x.ToString()), i => TypeChangeExe(i), sp);
        }

        private void TypeChangeExe(int i)
        {
            CurrentEditVariableType = _selectableVariableTypes[i];
            ResearchVariableData();
            ResetAddInputField(_editTargetVariableData);
            cycleScroll.UpdatePage(_searchedVariables.Count);
        }
    }
}