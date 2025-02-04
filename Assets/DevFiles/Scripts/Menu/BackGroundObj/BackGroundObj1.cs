using clrev01.Bases;
using System.Linq;
using UnityEngine;

namespace clrev01.Menu.BackGroundObj
{
    public class BackGroundObj1 : BaseOfCL
    {
        public float rollSpeed = 1;
        public float rollAcceleLate = 1;
        public Vector3 rollVector = new Vector3();
        public int randomSwitchFrame = 60;
        public Vector3 rollAccele = new Vector3();
        int count = 0;

        private void Awake()
        {
            var mfs = GetComponentsInChildren<MeshFilter>().ToList().ConvertAll(x => x.mesh).Distinct().ToList();
            foreach (var m in mfs)
            {
                m.SetIndices(m.GetIndices(0), MeshTopology.LineStrip, 0);
            }
        }
        void Update()
        {
            if (count % randomSwitchFrame == 0)
            {
                count = 0;
                rollAccele = Random.insideUnitSphere;
            }
            rollVector += rollAccele * rollAcceleLate;
            transform.Rotate(rollVector.normalized * rollSpeed);
        }
    }
}