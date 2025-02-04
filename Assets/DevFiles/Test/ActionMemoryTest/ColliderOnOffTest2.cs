using UnityEngine;

public class ColliderOnOffTest2 : MonoBehaviour
{
    //public Collider c;
    public GameObject g;
    public BoxCollider c;
    public float s = 10;
    public float ls = 0;
    Transform t = null;
    private void FixedUpdate()
    {
        if (c != null)
        {
            for (int i = 0; i < 1000; i++)
            {
                c.size = Random.insideUnitSphere;
            }
        }
        else
        {
            if (t == null) t = g.transform;
            for (int i = 0; i < 1000; i++)
            {
                t.localScale = Random.insideUnitSphere;
            }
        }
    }
}