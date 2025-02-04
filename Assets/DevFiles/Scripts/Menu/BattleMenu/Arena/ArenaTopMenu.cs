using clrev01.Bases;
using clrev01.Menu.CycleScroll;
using Cysharp.Text;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.BattleMenu.Arena
{
    public class ArenaTopMenu : BaseOfCL
    {
        [SerializeField]
        private CycleScroll.CycleScroll cycleScroll;
        [SerializeField]
        private List<ArenaTopButtonFunc> buttonList = new();
        [SerializeField]
        private MenuPage arenaMenuPage;
        [SerializeField]
        private ArenaMenu arenaMenu;

        private int selectedId
        {
            get => StaticInfo.Inst.selectedArena;
            set => StaticInfo.Inst.selectedArena = value;
        }


        private void Awake()
        {
            var cpl = cycleScroll.Initialize(cp => SettingPanel(cp));
            foreach (var cp in cpl)
            {
                var menuFunc = cp.GetComponent<ArenaTopButtonFunc>();
                buttonList.Add(menuFunc);
                cp.eventOnSelect.AddListener(cp => OnSelect(cp));
            }
        }

        private void OnEnable()
        {
            var selectedPanelNum = InitializeSelector();
            cycleScroll.UpdatePage(ArnHub.datas.Count);
            cycleScroll.SetSelect(selectedPanelNum);
            cycleScroll.SetScrollPosToFirstSelect();
            SetInfoIndicate();
        }

        private Utf8ValueStringBuilder sb = ZString.CreateUtf8StringBuilder();

        /// <summary>
        /// 選択中アリーナ情報表示更新
        /// </summary>
        private void SetInfoIndicate()
        { }

        private int InitializeSelector()
        {
            return selectedId;
        }

        private void SettingPanel(CycleScrollPanel cp)
        {
            buttonList[cp.panelId].SetIndicate(
                cp.tgtButtonInteractive ? ArnHub.datas[cp.itemId].Name : "",
                cp.tgtButtonInteractive ? ArnHub.datas[cp.itemId].description : ""
            );
        }

        private void OnSelect(CycleScrollPanel cp)
        {
            selectedId = cp.itemId;
            MPPM.OpenPage(arenaMenuPage);
        }
    }
}