using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColliderOnOffTest1 : MonoBehaviour
{
    //public Collider c;
    public List<Transform> t;
    public List<BoxCollider> bc;
    public float s = 10;
    public float ls = 0;
    public bool transformOrCollider;
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            SceneManager.LoadScene(0);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            for (int i = t.Count - 1; i >= 0; i--)
            {
                Destroy(t[i].gameObject);
            }
            t.Clear();
            bc.Clear();
            System.GC.Collect();
            return;
        }
        if (transformOrCollider)
        {
            foreach (var o in t)
            {
                o.localScale = new Vector3(Random.value, Random.value, Random.value);
            }
        }
        else
        {
            foreach (var o in bc)
            {
                o.size = new Vector3(Random.value, Random.value, Random.value);
            }
        }
    }
}