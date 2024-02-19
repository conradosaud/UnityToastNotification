using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCase : MonoBehaviour
{

    private void Update()
    {
        if( Input.GetKeyDown( KeyCode.V ))
        {
            ToastNotification.Show("Minha mensagem aqui hihih");
        }
    }

}
