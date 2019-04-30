using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YKEngine;

namespace BarrageEditor
{
    public class Main : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            UIManager.GetInstance().Init();
            ResourceManager.GetInstance().Init();

            ViewID.Init();

            UIManager.GetInstance().OpenView(ViewID.MainView);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
