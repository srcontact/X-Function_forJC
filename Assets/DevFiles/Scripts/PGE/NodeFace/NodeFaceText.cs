using clrev01.Bases;
using TMPro;
using UnityEngine;

namespace clrev01.PGE.NodeFace
{
    public class NodeFaceText : BaseOfCL
    {
        [SerializeField]
        private TextMeshProUGUI text;

        public void UpdateIndicate(bool indicate, string str)
        {
            gameObject.SetActive(indicate);
            if (indicate) text.text = str;
        }
    }
}