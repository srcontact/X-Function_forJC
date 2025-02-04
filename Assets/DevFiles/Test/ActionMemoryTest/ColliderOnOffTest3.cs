using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColliderOnOffTest3 : MonoBehaviour
{
    //public Collider c;
    public List<Transform> t;
    public List<CapsuleCollider> bc;
    public bool transformOrCollider;
    private void Update()
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
                o.height = Random.value;
            }
        }
    }
}