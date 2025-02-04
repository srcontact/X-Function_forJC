using System.Text;
using UnityEngine;

namespace clrev01.Save
{
    /// <summary>
    /// バイナリデータを自由な拡張子でアセットとして扱うためのScriptableObject
    /// TextAssetを使用していたが、.bytes拡張子以外の場合バイナリデータの扱いに問題があるのでこちらを使用する。
    /// 参考：https://discussions.unity.com/t/how-create-a-textasset-from-external-binary-file/841545
    /// </summary>
    public sealed class ScriptableTextAsset : ScriptableObject
    {
        [SerializeField]
        byte[] m_Bytes;

        public string text => Encoding.UTF8.GetString(m_Bytes);

        public byte[] bytes => (byte[])m_Bytes.Clone();

        public static ScriptableTextAsset CreateFromString(string text)
        {
            var asset = CreateInstance<ScriptableTextAsset>();
            asset.m_Bytes = Encoding.UTF8.GetBytes(text);
            return asset;
        }

        public static ScriptableTextAsset CreateFromBytes(byte[] bytes)
        {
            var asset = CreateInstance<ScriptableTextAsset>();
            asset.m_Bytes = (byte[])bytes.Clone();
            return asset;
        }

        public static ScriptableTextAsset CreateFromTextAsset(TextAsset text)
        {
            return CreateFromBytes(text.bytes);
        }

        public override string ToString()
        {
            return text;
        }
    }
}