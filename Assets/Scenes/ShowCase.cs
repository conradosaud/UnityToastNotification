using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Hey! If you use this code, you can invoke ToastNotification just writing Show instead ToastNotification.Show ^_~
using static ToastNotification;

public class ShowCase : MonoBehaviour
{

    private void Update()
    {
        // This is a example where you can use ToastNotification anywhere, like in a character script, item script, shop UI...
        // Anywhere you want, just call ToastNotification.Show :D
        if( Input.GetKeyDown( KeyCode.V ))
        {
            ToastNotification.Show("Yeah, a simple Key can display a message. And this message doens't have a \"timer\" display", 5);
        }
    }


    /* Welcome to Toast Notification! */

    public void ShowMessageDefault()
    {
        ToastNotification.Show("What you think about this messages? Looks good, dont?", 3, "info");
    }
    public void ShowMessageWhitoutIcon()
    {
        ToastNotification.Show("You dont need to use icons if you dont want it.");
    }
    public void ShowMessageWithoutTimer()
    {
        ToastNotification.Show("If timer is zero, this message will be infinite... or until you click to hide XD", 0 );
    }

    /* Custom Messages Secene */

    public void ShowMessageNewSetup()
    {
        ToastNotification.Show("This is a Light Theme example message, but you can change the background. Check the docs!");
    }

    public void ShowCenterMsg()
    {
        ToastNotification.messageScreenPosition = ToastNotification.MessageScreenPosition.Center;
        ToastNotification.Show("This is a Light Theme example message, but you can change the background. Check the docs!");
    }

    public void LeftBottomAndDark()
    {
        ToastNotification.messageScreenPosition = ToastNotification.MessageScreenPosition.BottomRight;
        ToastNotification.darkTheme = true;
        ToastNotification.Show("This is a Light Theme example message, but you can change the background. Check the docs!");
    }

    /* Scene manager */
    public void BackToHome()
    {
        SceneManager.LoadScene("ShowCase");
    }
    public void GoToCustom()
    {
        SceneManager.LoadScene("CustomMessages");
    }

}
