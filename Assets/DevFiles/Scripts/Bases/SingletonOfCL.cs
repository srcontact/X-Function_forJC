using UnityEngine;

namespace clrev01.Bases
{
    public class SingletonOfCL<T> : BaseOfCL where T : BaseOfCL
    {
        [Header("シングルトンパラメータ")]
        public bool dontDestroyOnLoad;
        public bool gameObjDestroy = true;
        private static T inst;
        public static T Inst
        {
            get
            {
                if (!inst)
                {
                    inst = (T)FindAnyObjectByType(typeof(T));
                }
                return inst;
            }
        }
        private void SingletonCheck()
        {
            if (this != Inst)
            {
                if (gameObjDestroy) Destroy(gameObject);
                else Destroy(this);
                return;
            }

            if (dontDestroyOnLoad) DontDestroyOnLoad(this.gameObject);
        }

        public virtual void Awake()
        {
            SingletonCheck();
        }
    }
}