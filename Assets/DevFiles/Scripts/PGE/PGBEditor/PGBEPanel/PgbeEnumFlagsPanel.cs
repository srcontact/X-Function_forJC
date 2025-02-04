using clrev01.Menu;
using clrev01.Programs;
using Cysharp.Text;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;
using static EnumLocalizationWithI2Localization.LocalizedEnumUtility;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class PgbeEnumFlagsPanel : PgbeUnmanagedPanel<long>
    {
        private string _separator = "&";
        private string[] _selectList;
        [SerializeField]
        private MenuButton button;
        [SerializeField]
        private TextMeshProUGUI buttonText;
        private Type _enumType;

        protected override void Awake()
        {
            base.Awake();
            button.OnClick.AddListener(() => OpenSelector());
        }
        public unsafe void OnOpenFromEnum(string title, Type et, long* pointer, string separator)
        {
            _enumType = et;
            _selectList = GetLocalizedNames(et).ToArray();
            OnPgbeOpen(title, pointer);
            _separator = separator;
        }
        public unsafe void OnOpenFromList(string title, string[] equipments, long* pointer, string separator)
        {
            _enumType = null;
            _selectList = equipments;
            OnPgbeOpen(title, pointer);
            _separator = separator;
        }
        private unsafe void OpenSelector()
        {
            Vector3 sp = Camera.main.WorldToScreenPoint(button.transform.position);
            PGEM2.quickMenuDialog.OpenQuickMenuFlags(
                new List<string>(_selectList),
                (long i) => OnValueChanged(i),
                _enumType != null ? ConvertEnumToListFlag(*tgtPointer) : *tgtPointer,
                sp
            );
        }

        private unsafe void OnValueChanged(long i)
        {
            *tgtPointer = _enumType == null ? i : ConvertListFlagToEnumFlag(i);
            SetIndicate(*tgtPointer);
            initPGBEPM.Invoke();
        }

        private Utf8ValueStringBuilder sb = ZString.CreateUtf8StringBuilder();

        protected override unsafe void SetIndicate(long data)
        {
            if (_enumType != null) data = ConvertEnumToListFlag(data);
            if (data <= 0)
            {
                buttonText.text = "None";
                return;
            }
            sb.Clear();
            bool b = true;
            for (int i = 0; i < _selectList.Length; i++)
            {
                if (((1L << i) & data) != 0)
                {
                    if (sb.Length > 0) sb.Append(_separator);
                    sb.Append(_selectList[i]);
                }
                else b = false;
            }
            buttonText.text = b ? "All" : sb.ToString();
        }

        private long ConvertListFlagToEnumFlag(long l)
        {
            long result = 0;
            for (var i = 0; i < ((long[])Enum.GetValues(_enumType)).Length; i++)
            {
                if (((1 << i) & l) > 0) result |= ((long[])Enum.GetValues(_enumType))[i];
            }
            return result;
        }

        private long ConvertEnumToListFlag(long l)
        {
            long result = 0;
            var values = (long[])Enum.GetValues(_enumType);
            for (var i = 0; i < values.Length; i++)
            {
                if ((values[i] & l) > 0) result |= 1L << i;
            }
            return result;
        }
    }
}