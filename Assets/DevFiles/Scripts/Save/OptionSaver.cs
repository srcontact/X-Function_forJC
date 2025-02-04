using UnityEngine;

namespace clrev01.Save
{
    [CreateAssetMenu(menuName = "セーブ関連/オプション")]
    public class OptionSaver : Saver<OptionSaver.OptionData>
    {
        public override string fileExt => "clopt";

        [System.Serializable]
        public class OptionData : SaveData
        {
            #region bgmVolume
            [SerializeField]
            private float _bgmVolume = 0.5f;
            public float bgmVolume
            {
                get => _bgmVolume;
                set => _bgmVolume = value;
            }
            #endregion
            #region seVolume
            [SerializeField]
            private float _seVolume = 0.5f;
            public float seVolume
            {
                get => _seVolume;
                set => _seVolume = value;
            }
            #endregion

            #region minSiticInput
            [SerializeField]
            private float _minSiticInput = 0.25f;
            public float minSiticInput
            {
                get => _minSiticInput;
                set => _minSiticInput = value;
            }
            #endregion
        }

        public override string dataName => "OptionData";
    }
}