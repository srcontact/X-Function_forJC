using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;
using clrev01.Save;

namespace clrev01.Editor.ClScriptedImporter
{
    [ScriptedImporter(2, "xft")]
    public class TeamDataScriptedImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var text = File.ReadAllBytes(ctx.assetPath);
            var textAsset = ScriptableTextAsset.CreateFromBytes(text);
            ctx.AddObjectToAsset("Main", textAsset);
            ctx.SetMainObject(textAsset);
        }
    }
}