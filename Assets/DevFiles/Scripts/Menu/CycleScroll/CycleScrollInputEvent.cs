using UnityEngine.Events;

namespace clrev01.Menu.CycleScroll
{
    [System.Serializable]
    public class CycleScrollInputEvent : UnityEvent<string, string, CycleScrollPanel>
    { }
}