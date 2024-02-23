using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script : MonoBehaviour
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

            ToastNotification.Show("Opa e ae, bãum?", 0, "error");
        }
        
    }
}
