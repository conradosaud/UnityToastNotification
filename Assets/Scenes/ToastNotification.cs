using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ToastNotification : MonoBehaviour
{

    static List<Transform> messages;
    public Transform messagePrefab;
    public bool showTimerRender = true;

    public static bool isStoped = false;
    public static float minimunMessageTime = 3;

    void Start()
    {

        messages = new List<Transform>();
        Show("Texto de testes aqui", 3, "info");
        messagePrefab.gameObject.SetActive(false);

    }

    public void Show( string messageText, int timerInSeconds = 3, string iconName = "", bool darkTheme = true )
    {

        Transform canvas = transform.parent;
        Transform message = Instantiate(messagePrefab, canvas);
        message.gameObject.SetActive(true);        

        TextMeshProUGUI text = message.Find("Text").GetComponent<TextMeshProUGUI>();
        Image background = message.Find("Background").GetComponent<Image>();
        Transform icons = message.Find("Icons");
        Image timer = message.Find("Timer").GetComponent<Image>();

        text.text = messageText;

        Image selectedIcon = null;
        if( iconName != "")
        {
            iconName = Capitalize(iconName);
            selectedIcon = icons.Find(iconName).transform.GetComponent<Image>();
            selectedIcon.enabled = true;
        }

        // Dark theme default
        Color foreColor = text.color = new Color(255, 255, 255, 1);
        Color backgroundColor = new Color(0.26f, 0.26f, 0.26f, 0.78f);
        if ( darkTheme == false )
        {
            foreColor = new Color(0.26f, 0.26f, 0.26f, 1);
            backgroundColor = new Color(255, 255, 255, 0.78f);
        }
        // SecondaryColor (timer element) is based on foreColor
        Color secondaryColor = foreColor;
        secondaryColor.a = 0.39f;

        text.color = foreColor;
        background.color = backgroundColor;
        timer.color = secondaryColor;
        if (selectedIcon != null)
            selectedIcon.color = foreColor;


        ToastNotificationMessage toastNotificationMessage = message.GetComponent<ToastNotificationMessage>();
        toastNotificationMessage.timerRectTransform = timer.GetComponent<RectTransform>();
        toastNotificationMessage.messageTime = timerInSeconds;

        timer.enabled = showTimerRender;

    }

    #region Utilities Functions

/* *********************************
*              Utilities
* *********************************/

    string Capitalize( string text )
    {
        // Verifica se a string é nula ou vazia
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        // Converte a primeira letra para maiúscula e as demais para minúscula
        return char.ToUpper(text[0]) + text.Substring(1).ToLower();
    }

    #endregion

}
