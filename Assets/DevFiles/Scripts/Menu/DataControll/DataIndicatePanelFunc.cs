using clrev01.Bases;
using clrev01.Menu.CycleScroll;
using TMPro;
using UnityEngine;

namespace clrev01.Menu.DataControll
{
    public class DataIndicatePanelFunc :
        BaseOfCL
    {
        public CycleScrollPanel panel;
        public TextMeshProUGUI titleTxt;
        [SerializeField]
        TMP_InputField inputField;
        public CycleScrollInputEvent onInput = new CycleScrollInputEvent();
        //public CycleScrollEvent onBeginDrag = new CycleScrollEvent();
        //public CycleScrollEvent onEndDrag = new CycleScrollEvent();

        protected void Awake()
        {
            panel = GetComponent<CycleScrollPanel>();

            inputField.onEndEdit.AddListener((string s) => onInput.Invoke(s, titleTxt.text, panel));
            inputField.onEndEdit.AddListener((string s) => CloseInputField());
        }
        public void OpenInputField(string defaultTxt)
        {
            inputField.gameObject.SetActive(true);
            inputField.text = defaultTxt;
            inputField.Select();
        }
        void CloseInputField()
        {
            inputField.gameObject.SetActive(false);
        }

        //public void OnBeginDrag(PointerEventData eventData)
        //{
        //    Debug.Log("drag");
        //    onBeginDrag.Invoke(panel);
        //}

        //public void OnEndDrag(PointerEventData eventData)
        //{
        //    Debug.Log("endDrag");
        //    onEndDrag.Invoke(panel);
        //}

        //public void OnDrag(PointerEventData eventData)
        //{ }
    }
}