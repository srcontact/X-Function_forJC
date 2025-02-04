using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class ForeachTest1 : MonoBehaviour
    {
        public int objNum = 1000;
        List<int> intList = new List<int>();

        public class TestClass1
        {
            public int i = 0;
        }

        List<TestClass1> classList = new List<TestClass1>();
        public bool doIntList, doClassList, forOrForeach;

        int count;

        private void Start()
        {
            for (int i = 0; i < objNum; i++)
            {
                intList.Add(i);
                classList.Add(new TestClass1());
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                doIntList = !doIntList;
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                doClassList = !doClassList;
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                forOrForeach = !forOrForeach;
            }

            if (forOrForeach)
            {
                if (doIntList)
                {
                    foreach (var i in intList)
                    {
                        count += i;
                    }
                }
                if (doClassList)
                {
                    foreach (var c in classList)
                    {
                        c.i++;
                    }
                }
            }
            else
            {
                if (doIntList)
                {
                    for (int i = 0; i < intList.Count; i++)
                    {
                        count += i;
                    }
                }
                if (doClassList)
                {
                    for (int i = 0; i < classList.Count; i++)
                    {
                        classList[i].i++;
                    }
                }
            }
        }
    }
}