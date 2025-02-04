using clrev01.Menu;
using clrev01.Programs;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;
using static EnumLocalizationWithI2Localization.LocalizedEnumUtility;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class PgbeEnumPanel : PgbeUnmanagedPanel<int>
    {
        List<string> nameList;
        List<int> valueList;
        [SerializeField]
        MenuButton button;
        [SerializeField]
        TextMeshProUGUI buttonText;
        private bool _flagsMode;

        protected override void Awake()
        {
            base.Awake();
            button.OnClick.AddListener(() => OpenSelecter());
        }
        public unsafe void OnOpenFromEnum(string title, int* pointer, Type et, params int[] ignoreList)
        {
            _flagsMode = false;
            nameList = GetLocalizedNames(et);
            valueList = ((int[])Enum.GetValues(et)).ToList();
            if (ignoreList != null)
            {
                for (var i = 0; i < ignoreList.Length; i++)
                {
                    if (!valueList.Contains(ignoreList[i])) continue;
                    valueList.RemoveAt(i);
                    nameList.RemoveAt(i);
                }
            }
            OnPgbeOpen(title, pointer);
        }
        public unsafe void OnOpenFromList(string title, int* pointer, List<string> nameList, List<int> valueList = null)
        {
            this.nameList = nameList;
            this.valueList = valueList;
            OnPgbeOpen(title, pointer);
        }
        unsafe void OpenSelecter()
        {
            Vector3 sp = Camera.main.WorldToScreenPoint(button.transform.position);
            PGEM2.quickMenuDialog.OpenQuickMenu(new List<string>(nameList), (int i) => OnValueChanged(i), sp);
        }
        private unsafe void OnValueChanged(int i)
        {
            *tgtPointer = valueList != null ? valueList[i] : i;
            SetIndicate(*tgtPointer);
            initPGBEPM.Invoke();
        }

        protected override void SetIndicate(int data)
        {
            //todo:サブルーチン実行ノードを開いた時にここでエラー。Array.IndexOf(valueList, data)で正しい値を入れられていない。
            buttonText.text = nameList[valueList != null ? Array.IndexOf(valueList.ToArray(), data) : data];
        }
    }
}