using UnityEngine;

public class SearchMark : MonoBehaviour
{
    public bool isDestroy;
    [SerializeField]
    int count = 120;

    // Update is called once per frame
    void Update()
    {
        if (!isDestroy) return;
        if (count < 0) Destroy(gameObject);
        count--;
    }
}