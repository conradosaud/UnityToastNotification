using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
/// Este script controla o movimento de um objeto.
/// </summary>
public class ToastNotification : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    static List<Transform> messages;
    public Transform messagePrefab;

    /// <summary>
    /// Especificações do default
    /// </summary>
    /// 
    
    [Header("Default Messages Patterns:")]
    [Tooltip("A countdown image will be displayed on message like a timer")]
    public bool showTimerRender = true;
    [Tooltip("All messages will be displayed on right top corner of screen. Disabled it to show on LEFT top corner.")]
    public bool rightTopCorner = true;
    [Tooltip("Disable it to use Light Theme on messages")]
    public bool darkTheme = true;
    [Tooltip("Minimun time that all messages will be displayed.")]
    public float minimumMessageTime = 3;
    [Tooltip("Margin X and Y on the corners. Margin X doens't works with centralized messages.")]
    public Vector2 margin = new Vector2(20, 20);
    [Tooltip("Stop the timer when mouse cursor is over the ToastNotification component")]
    public bool stopOnOver = true;
    [Tooltip("Hide the message when it's clicked")]
    public bool hideOnClick = true;

    public float tempoDEV = 300;

    public static bool isStoped = false;

    // Static variables don't show on Unity Editor
    public static float static_minimumMessageTime = 3;
    public static bool static_hideOnClick = true;


    void Start()
    {

        static_minimumMessageTime = minimumMessageTime;
        static_hideOnClick = hideOnClick;

        messages = new List<Transform>();
        messagePrefab.gameObject.SetActive(false);

        Show("Texto de testes aqui", tempoDEV, "info");

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(stopOnOver)
            isStoped = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if(stopOnOver)
            isStoped = false;
    }

    public void Show( string messageText, float timerInSeconds = 3, string iconName = "", bool darkTheme = true )
    {

        //Transform canvas = transform.parent;
        Transform message = Instantiate(messagePrefab, transform);
        message.gameObject.SetActive(true);        

        TextMeshProUGUI text = message.Find("Text").GetComponent<TextMeshProUGUI>();
        UnityEngine.UI.Image background = message.Find("Background").GetComponent<UnityEngine.UI.Image>();
        Transform icons = message.Find("Icons");
        UnityEngine.UI.Image timer = message.Find("Timer").GetComponent<UnityEngine.UI.Image>();

        text.text = messageText;

        UnityEngine.UI.Image selectedIcon = null;
        if( iconName != "")
        {
            iconName = Capitalize(iconName);
            selectedIcon = icons.Find(iconName).transform.GetComponent<UnityEngine.UI.Image>();
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

        toastNotificationMessage.leftToRight = rightTopCorner;
        timer.enabled = showTimerRender;

        RectTransform parentRect = GetComponent<RectTransform>();
        parentRect.anchoredPosition = Vector3.zero;

        Vector2 backgroundSize = background.GetComponent<RectTransform>().sizeDelta;
        message.GetComponent<RectTransform>().sizeDelta = backgroundSize;
        parentRect.sizeDelta = backgroundSize;

        if( rightTopCorner == true)
        {
            parentRect.anchorMax = new Vector2(1, 1);
            parentRect.anchorMin = new Vector2(1, 1);
            parentRect.anchoredPosition = new Vector2( -backgroundSize.x - margin.x , -backgroundSize.y - margin.y ) ;
        }
        else
        {
            parentRect.anchorMax = new Vector2(0, 1);
            parentRect.anchorMin = new Vector2(0, 1);
            parentRect.anchoredPosition = new Vector2( margin.x , -backgroundSize.y - margin.y );
        }




        //transform.position = Vector3.zero;
        //novo.y = Screen.height - message.GetComponent<RectTransform>().sizeDelta.y / 2;
        //novo.x = Screen.width - message.GetComponent<RectTransform>().sizeDelta.x*2 - message.GetComponent<RectTransform>().sizeDelta.x;
        //message.position = novo - margin;


    }

    #region Utilities Functions

/* *********************************
*              Utilities
* *********************************/

    void ListComponents( Transform obj )
    {
        // Lista todos os componentes do próprio objeto
        Component[] components = obj.GetComponents<Component>();

        // Itera sobre os componentes e os imprime no console
        foreach (Component component in components)
        {
            Debug.Log("Componente do objeto: " + component.GetType().Name);
        }
    }

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
