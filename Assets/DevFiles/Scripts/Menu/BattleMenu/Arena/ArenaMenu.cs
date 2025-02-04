using System.Collections.Generic;
using System.Linq;
using clrev01.Bases;
using clrev01.Menu.CycleScroll;
using Cysharp.Text;
using UnityEngine;
using UnityEngine.Serialization;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.BattleMenu.Arena
{
    public class ArenaMenu : BaseOfCL
    {
        [SerializeField]
        private CycleScroll.CycleScroll cycleScroll;
        [SerializeField]
        private List<ArenaButtonFunc> buttonList = new();

        private int selectedArenaId => StaticInfo.Inst.selectedArena;
        private int selectedId
        {
            get => StaticInfo.Inst.selectedArenaBattle;
            set => StaticInfo.Inst.selectedArenaBattle = value;
        }
        private ArenaHubData arenaHubData => selectedArenaId >= 0 && selectedArenaId < ArnHub.datas.Count ? ArnHub.datas[selectedArenaId] : null;
        [SerializeField]
        private MenuPage arenaBattleMenuPage;
        [SerializeField]
        private ArenaBattleMenu arenaBattleMenu;


        private void Awake()
        {
            var cpl = cycleScroll.Initialize(cp => SettingPanel(cp));
            foreach (var cp in cpl)
            {
                var menuFunc = cp.GetComponent<ArenaButtonFunc>();
                buttonList.Add(menuFunc);
                cp.eventOnSelect.AddListener(cp => OnSelect(cp));
            }
        }

        private void OnEnable()
        {
            var selectedPanelNum = InitializeSelector();
            cycleScroll.UpdatePage(arenaHubData.arenaData.arenaBattleCount);
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
            var battleData = arenaHubData.arenaData.GetArenaBattle(cp.itemId)?.battleData;
            var resultData = StaticInfo.Inst.arenaSaveData.GetBattleResult(selectedArenaId, cp.itemId);
            if (battleData == null)
            {
                buttonList[cp.panelId].SetIndicate("", "");
                return;
            }
            if (resultData == null) return;
            var performanceText =
                battleData.GetPerformanceMetricsText(battleData, resultData);
            buttonList[cp.panelId].SetIndicate(
                cp.tgtButtonInteractive ? battleData.titleStr : "",
                cp.tgtButtonInteractive ? $"{battleData.descriptionStr}\n{performanceText}" : ""
            );
        }

        private void OnSelect(CycleScrollPanel cp)
        {
            selectedId = cp.itemId;
            MPPM.OpenPage(arenaBattleMenuPage);
        }
    }
}