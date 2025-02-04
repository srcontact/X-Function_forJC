using clrev01.Bases;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace clrev01.ClAction.UI
{
    public class SceneChange : BaseOfCL, IPointerClickHandler
    {
        public int nextScene;

        public void OnPointerClick(PointerEventData eventData)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}