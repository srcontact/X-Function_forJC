using clrev01.Extensions;
using clrev01.Programs;
using clrev01.Save;
using UnityEngine;
using static clrev01.Extensions.ExUI;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGB
{
    public partial class PGBlock2
    {
        public PGBlock2 NextPgb => editorPar is null || editorPar.nextIndex < 0 ? null : PGEM2.pgbList[editorPar.nextIndex];
        public int nextIndex => editorPar?.nextIndex ?? -1;
        public PGBlock2 FalseNextPgb => editorPar is null || editorPar.falseNextIndex < 0 ? null : PGEM2.pgbList[editorPar.falseNextIndex];
        public int falseNextIndex => editorPar?.falseNextIndex ?? -1;
        [SerializeField]
        private PGBConnectLine.PGBConnectLine trueLine, falseLine, commentLine;

        public virtual void ConnectionChanging(PGBData connectChangeTgt, int connectNum)
        {
            Debug.Log("connect__" + gameObject);
            pgbd.ConnectionChanging(connectChangeTgt, connectNum);
        }
        public void ConnectLineUpdate()
        {
            ConnectLineUpdate(trueLine, NextPgb);
            ConnectLineUpdate(falseLine, FalseNextPgb);
            ConnectLineUpdate(commentLine, NextPgb);
        }
        public void ConnectLineUpdate(PGBConnectLine.PGBConnectLine tgtLine, PGBlock2 tgtBlock)
        {
            if (!tgtLine.gameObject.activeSelf) return;
            if (pgbd == PGEM2.nowConnectionChangingPGB && PGEM2.connectNum == tgtLine.LineNum)
            {
                Vector3 tgtPos = GetPointerPos();
                tgtPos.z = PGEM2.pos.z;
                tgtLine.ConnectUpdate(tgtPos);
            }
            else
            {
                tgtLine.ConnectUpdate(this, tgtBlock);
            }
        }
        public void ConnectLineTextureScaleUpdate(float scale)
        {
            trueLine.TextureScaleUpdate(scale);
            falseLine.TextureScaleUpdate(scale);
        }
    }
}