using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ToastNotification.Show("mensagem?");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.V)) {

            //ToastNotification.Show("Oh, you don't want icons? No problem at all", 30);
            ToastNotification.Show("If you hover over it, the timer pauses. Clicking will make it disappear", 5);
        }
        
    }
}
