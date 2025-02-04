using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using I2.Loc;


namespace EnumLocalizationWithI2Localization
{
    public static class LocalizedEnumUtility
    {
        public static string ToLocalizedString(this Enum enumValue)
        {
            var term = $"_autoEnumLocalize/{enumValue.GetType()}/{enumValue}";
            return LocalizationManager.GetTranslation(term) ?? enumValue.ToString();
        }

        public static string[] GetLocalizedNamesArray(Type enumType)
        {
            var temp = Enum.GetValues(enumType);
            var names = new string[temp.Length];
            for (var i = 0; i < temp.Length; i++)
            {
                names[i] = (temp.GetValue(i) as Enum).ToLocalizedString();
            }
            return names;
        }

        public static List<string> GetLocalizedNames(Type enumType)
        {
            return GetLocalizedNamesArray(enumType).ToList();
        }
    }

#if UNITY_EDITOR
    public static class LocalizedEnumTermUpdater
    {
        [MenuItem("Tools/Localization/Update Localized Enums")]
        public static void UpdateLocalizedEnums()
        {
            // 1. プロジェクト内の [LocalizedEnum] が付いた enum を探す
            //    ※ ここでは Reflection で全アセンブリを探す例
            var enumTypes = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsEnum && t.Namespace != null && t.Namespace.StartsWith("clrev01"))
                .ToArray();

            // 2. I2Localization の Language Source Asset をロード
            //    実際のプロジェクトのパスや方法に合わせてください
            var sourceAsset = Resources.Load<LanguageSourceAsset>(LocalizationManager.GlobalSources[0]);
            if (sourceAsset == null)
            {
                Debug.LogError("I2Localization Source Asset not found.");
                return;
            }

            // 3. 追加が必要な Term を洗い出し
            var termsToAdd = new List<(string term, string value)>();
            foreach (var enumType in enumTypes)
            {
                var enumName = enumType;
                var enumValues = System.Enum.GetNames(enumType);

                foreach (var valName in enumValues)
                {
                    var term = $"_autoEnumLocalize/{enumName}/{valName}";
                    // I2Localization側にTermが存在するかチェック
                    if (!CheckTermExists(sourceAsset, term))
                    {
                        termsToAdd.Add((term, valName));
                    }
                }
            }

            // 4. 不足分を実際に追加
            foreach (var tv in termsToAdd)
            {
                Debug.Log($"Add Term: {tv.term}");
                AddTermToI2Localization(sourceAsset, tv.term, tv.value);
            }

            // 5. 保存
            EditorUtility.SetDirty(sourceAsset);
            AssetDatabase.SaveAssets();

            Debug.Log("Localized enums updated successfully.");
        }

        private static bool CheckTermExists(LanguageSourceAsset sourceAsset, string term)
        {
            return sourceAsset.SourceData.GetTermData(term) != null;
        }

        private static void AddTermToI2Localization(LanguageSourceAsset sourceAsset, string term, string value)
        {
            var nt = sourceAsset.SourceData.AddTerm(term, eTermType.Text);
            nt.SetTranslation(0, value);
        }
    }
#endif
}