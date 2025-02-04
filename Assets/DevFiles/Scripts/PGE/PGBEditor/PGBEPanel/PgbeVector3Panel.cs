using UnityEngine;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class PgbeVector3Panel : PgbeUnmanagedMultiInputPanel<Vector3>
    {
        protected override void SetIndicate(Vector3 data)
        {
            inputFields[0].text = data.x.ToString();
            inputFields[1].text = data.y.ToString();
            inputFields[2].text = data.z.ToString();
        }
        protected override unsafe bool TryParseExe(int i, string s, out Vector3 res)
        {
            res = *tgtPointer;
            if (!float.TryParse(s, out var f)) return false;
            res[i] = f;
            return true;
        }
    }
}