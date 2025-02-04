using clrev01.Bases;
using clrev01.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.Menu
{
    public class TabMenu : BaseOfCL
    {
        [SerializeField]
        private GameObject tabArea, contentArea;
        [SerializeField]
        private TabMenuTabButton origTabMenuTabButton;
        [SerializeField, Range(0f, 1f)]
        private float unselectedTabHeight = 0.8f;
        [SerializeField]
        private List<TabMenuTabButton> tabMenuTabButtons = new();
        public int nowSelectedTabNumber { get; private set; }
        private Action<int> _indicateTabContentAction;

        public void SetIndicateTabContentAction(Action<int> indicateTabContent)
        {
            _indicateTabContentAction = indicateTabContent;
        }
        public void SetTab(IReadOnlyList<string> tabTitles)
        {
            tabMenuTabButtons.ForEach(x => x.gameObject.SetActive(false));
            for (var i = 0; i < tabTitles.Count; i++)
            {
                var title = tabTitles[i];
                TabMenuTabButton tabButton;
                if (tabMenuTabButtons.Count <= i)
                {
                    tabButton = origTabMenuTabButton.SafeInstantiate();
                    tabButton.transform.parent = tabArea.transform;
                    tabButton.lpos = Vector3.zero;
                    tabButton.scl = Vector3.one;
                    tabMenuTabButtons.Add(tabButton);
                }
                else
                {
                    tabButton = tabMenuTabButtons[i];
                }
                tabButton.gameObject.SetActive(true);
                tabButton.text.text = title;
                var i1 = i;
                tabButton.button.OnClick.RemoveAllListeners();
                tabButton.button.OnClick.AddListener(() => { _indicateTabContentAction.Invoke(i1); });
                tabButton.button.OnClick.AddListener(() => OnTabButtonClicked(tabButton));
                tabButton.tabNumber = i;
            }
            FirstTabSelect();
        }
        private void OnTabButtonClicked(TabMenuTabButton tabMenuTabButton)
        {
            for (var i = 0; i < tabMenuTabButtons.Count; i++)
            {
                var tabButton = tabMenuTabButtons[i];
                var isSelected = tabButton == tabMenuTabButton || (tabMenuTabButton == null && i == 0);
                TabSelectChange(tabButton, isSelected);
            }
        }
        private void TabSelectChange(TabMenuTabButton tabButton, bool b)
        {
            tabButton.button.isHighlight = b;

            var rectTransformAnchorMax = tabButton.rectTransform.anchorMax;
            rectTransformAnchorMax.y = b ? 1f : unselectedTabHeight;
            tabButton.rectTransform.anchorMax = rectTransformAnchorMax;
            if (b)
            {
                nowSelectedTabNumber = tabButton.tabNumber;
            }
        }

        private void FirstTabSelect()
        {
            var first = true;
            for (var i = 0; i < tabMenuTabButtons.Count; i++)
            {
                var tabButton = tabMenuTabButtons[i];
                if (first && tabButton.gameObject.activeSelf)
                {
                    TabSelectChange(tabButton, true);
                    first = false;
                }
                else
                {
                    TabSelectChange(tabButton, false);
                }
            }
        }

        public void SetTabActives(IReadOnlyList<bool> tabActives)
        {
            for (int i = 0; i < tabMenuTabButtons.Count && i < tabActives.Count; i++)
            {
                var tabButton = tabMenuTabButtons[i];
                tabButton.gameObject.SetActive(tabActives[i]);
            }
            FirstTabSelect();
        }
    }
}