using clrev01.Bases;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

namespace clrev01.Initialize
{
    public class Initializer : BaseOfCL
    {
        [SerializeField]
        TextMeshProUGUI titleText;
        [SerializeField]
        string title = "X-Function";
        [SerializeField]
        char paddingChar = 'X';
        string PaddingStr
        {
            get => new string(paddingChar, title.Length);
        }
        [SerializeField]
        float animateStart = 0.5f, animateTextInterval = 0.1f, titleWaitTime = 2;
        private void Awake()
        {
            TextAnimeAndGoNextScene().Forget();
        }

        private async UniTask TextAnimeAndGoNextScene()
        {
            titleText.text = PaddingStr;
            await UniTask.Delay(TimeSpan.FromSeconds(animateStart), DelayType.Realtime);
            for (int i = 0; i < title.Length; i++)
            {
                titleText.text = title.Substring(0, i + 1) + new string(paddingChar, title.Length - i - 1);
                await UniTask.Delay(TimeSpan.FromSeconds(animateTextInterval), DelayType.Realtime);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(titleWaitTime), DelayType.Realtime);
            StaticInfo.Inst.NextScene(1);
        }
    }
}