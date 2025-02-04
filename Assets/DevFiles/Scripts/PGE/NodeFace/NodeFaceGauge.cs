using clrev01.Bases;
using UnityEngine;
using UnityEngine.UI;

namespace clrev01.PGE.NodeFace
{
    public class NodeFaceGauge : BaseOfCL
    {
        public Image gaugeImage, gaugeImage2;
        public GaugeMode gaugeMode;

        public enum GaugeMode
        {
            Gauge,
            Rotate,
            Flip,
            PlusMinusGauge,
        }

        public void SetGaugeValue(float? value)
        {
            if (!value.HasValue)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            switch (gaugeMode)
            {
                case GaugeMode.Gauge:
                    gaugeImage.fillAmount = value.Value;
                    break;
                case GaugeMode.Rotate:
                    gaugeImage.transform.rotation = Quaternion.Euler(0, 0, -value.Value);
                    break;
                case GaugeMode.Flip:
                    gaugeImage.transform.localScale = value > 0 ? Vector3.one : new Vector3(-1, 1, 1);
                    break;
                case GaugeMode.PlusMinusGauge:
                    gaugeImage.fillAmount = value.Value;
                    gaugeImage2.fillAmount = -value.Value;
                    break;
            }
        }
    }
}