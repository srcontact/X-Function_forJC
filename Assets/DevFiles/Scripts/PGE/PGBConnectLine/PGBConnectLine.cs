using clrev01.Bases;
using clrev01.PGE.PGB;
using clrev01.Programs;
using System.Collections.Generic;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGBConnectLine
{
    public class PGBConnectLine : BaseOfCL
    {
        [SerializeField]
        private int lineNum;
        public int LineNum => lineNum;
        [SerializeField]
        private LineRenderer connectLine;
        [SerializeField]
        private float
            connectLineOffset = 5,
            connectLineBackOffset = 10,
            connectLineWidth = 3;
        private Material _material;
        private Vector2 _textureScale;
        private Vector3[] _pl = new Vector3[2];
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            _material = connectLine.material;
            _textureScale = _material.mainTextureScale;
        }
        public void ConnectUpdate(PGBlock2 owner, PGBlock2 tgt)
        {
            if (tgt == null || owner == tgt)
            {
                connectLine.enabled = false;
                return;
            }
            connectLine.enabled = true;
            ConnectUpdate(tgt.pos);
            connectLine.enabled = IsLineRendererVisible(connectLine, _camera);
        }
        public void ConnectUpdate(Vector3 tgtPos)
        {
            /*
            接続を実装。
            useLineでtgtとつなぐ。
            その後中心からずらす。
            */
            connectLine.positionCount = 2;

            _pl[0] = Vector3.zero;
            _pl[1] = transform.InverseTransformPoint(tgtPos);
            Vector3 fromTo = Vector3.Normalize(_pl[1] - _pl[0]);
            Vector3 offset = Vector3.Cross(fromTo, Vector3.forward) * connectLineOffset;
            offset.z = connectLineBackOffset;
            _pl[0] += offset;
            _pl[1] += offset;
            connectLine.SetPositions(_pl);
            connectLine.widthMultiplier = connectLineWidth * PGEM2.nowScale / PGEM2.uiCameraSizer.sizeRate;
            connectLine.enabled = true;
        }
        public void TextureScaleUpdate(float scale)
        {
            var ts = _textureScale;
            ts.x /= scale;
            _material.mainTextureScale = ts;
        }

        public static bool IsLineRendererVisible(LineRenderer lineRenderer, Camera targetCamera)
        {
            if (lineRenderer == null || targetCamera == null)
            {
                return false;
            }

            // LineRenderer の頂点数を取得
            int positionCount = lineRenderer.positionCount;
            if (positionCount == 0) return false;

            // すべての頂点をループしてビューポートに変換
            for (int i = 0; i < positionCount; i++)
            {
                // LineRenderer のローカル座標をワールド座標に変換
                Vector3 localPos = lineRenderer.GetPosition(i);
                Vector3 worldPos = lineRenderer.transform.TransformPoint(localPos);

                // ワールド座標をビューポート座標に変換
                Vector3 viewportPos = targetCamera.WorldToViewportPoint(worldPos);

                // z < 0 の場合はカメラの背面
                if (viewportPos.z < 0f) continue;

                // ビューポート座標 (0～1) の範囲内にあれば、少なくともその頂点は画面内に表示されている
                if (viewportPos.x >= 0f && viewportPos.x <= 1f &&
                    viewportPos.y >= 0f && viewportPos.y <= 1f)
                {
                    return true;
                }
            }

            // どの頂点もビューポート内になければ不可視とみなす
            return false;
        }
    }
}