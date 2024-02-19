using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
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


    /// <summary>
    /// Especificações do default
    /// </summary>
    /// 

    public Transform _messagePrefab;

    public static bool isStoped = false;
    private static Transform messagePrefab;
    private static Transform toastNotification;
    private static bool showTimerRender;
    private static TimerDirection timerDirection;
    private static MessageScreenPosition messageScreenPosition;
    private static Vector2 margin;
    private static bool darkTheme;
    public static float minimumMessageTime = 3;
    public static bool hideOnClick = true;

    /* 
     * Variáveis públicas são definidas com o prefixo underline (_) antes delas 
     * Dessa forma, as variáveis estáticas que serão realmente usadas no código
     *      podem usar o nome das variáveis públicas e fazer seu trabalho
     */

    [Header("Default Messages Patterns:")]
    [Tooltip("A countdown image will be displayed on message like a timer")]
    public bool _showTimerRender = true;
    [Tooltip("Disable it to use Light Theme on messages")]
    public bool _darkTheme = true;
    [Tooltip("Minimun time that all messages will be displayed.")]
    public float _minimumMessageTime = 3;
    [Tooltip("Margin X and Y on the corners. Margin X doens't works with centralized messages.")]
    public Vector2 _margin = new Vector2(20, 20);
    [Tooltip("Stop the timer when mouse cursor is over the ToastNotification component")]
    public bool _stopOnOver = true;
    [Tooltip("Hide the message when it's clicked")]
    public bool _hideOnClick = true;

    [Tooltip("Position of messages on screen")]
    public enum MessageScreenPosition { TopLeft, TopCenter, TopRight, Center, BottomLeft, BottomCenter, BottomRight }
    public MessageScreenPosition _messageScreenPosition = MessageScreenPosition.TopRight;
    [Tooltip("Direction of timer countdown. Auto will choose the best position relative to the Message Screen Position option.")]
    public enum TimerDirection { Auto, LeftToRight, RightToLeft}
    public TimerDirection _timerDirection = TimerDirection.Auto;

    public float tempoDEV = 300;

    void Start()
    {


        minimumMessageTime = _minimumMessageTime;
        hideOnClick = _hideOnClick;

        messagePrefab = _messagePrefab;
        toastNotification = transform;
        darkTheme = _darkTheme;
        showTimerRender = _showTimerRender;
        timerDirection = _timerDirection;
        messageScreenPosition = _messageScreenPosition;
        margin = _margin;

        messagePrefab.gameObject.SetActive(false);

        Show("Texto de testes aqui", tempoDEV, "info", darkTheme);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_stopOnOver)
            isStoped = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if(_stopOnOver)
            isStoped = false;
    }

    public static void Show( string messageText)
    {
        Show( messageText, minimumMessageTime, "info", darkTheme );
    }

    public static void Show( string messageText, float timerInSeconds, string iconName, bool darkTheme )
    {

        Transform message = Instantiate(messagePrefab, toastNotification);
        message.gameObject.SetActive(true);        

        TextMeshProUGUI text = message.Find("Text").GetComponent<TextMeshProUGUI>();
        UnityEngine.UI.Image background = message.Find("Background").GetComponent<UnityEngine.UI.Image>();
        Transform icons = message.Find("Icons");
        UnityEngine.UI.Image timer = message.Find("Timer").GetComponent<UnityEngine.UI.Image>();

        text.text = messageText;
        UnityEngine.UI.Image selectedIcon = null;

        Vector2 backgroundSize = background.GetComponent<RectTransform>().sizeDelta;
        RectTransform parentRect = toastNotification.GetComponent<RectTransform>();

        SetMessageIcon();
        SetMessageColor();
        SetupInvokeMessage();
        ResetToastNoticationPosition();
        SetMessagePositionOnScreen();

        void SetMessageIcon()
        {
            
            if( iconName != "")
            {
                iconName = Capitalize(iconName);
                selectedIcon = icons.Find(iconName).transform.GetComponent<UnityEngine.UI.Image>();
                selectedIcon.enabled = true;
            }
        }

        void SetMessageColor()
        {
            // Dark theme default
            Color foreColor = text.color = new Color(255, 255, 255, 1);
            Color backgroundColor = new Color(0.26f, 0.26f, 0.26f, 0.78f);
            if (darkTheme == false)
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
        }

        void SetupInvokeMessage()
        {
            ToastNotificationMessage toastNotificationMessage = message.GetComponent<ToastNotificationMessage>();
            toastNotificationMessage.timerRectTransform = timer.GetComponent<RectTransform>();
            toastNotificationMessage.messageTime = timerInSeconds;

            timer.enabled = showTimerRender;
            toastNotificationMessage.leftToRight = timerDirection == TimerDirection.LeftToRight;
            if (timerDirection == TimerDirection.Auto)
                toastNotificationMessage.leftToRight = messageScreenPosition != MessageScreenPosition.TopLeft || messageScreenPosition != MessageScreenPosition.BottomLeft;

        }

        void ResetToastNoticationPosition()
        {
            parentRect.anchoredPosition = Vector3.zero;

            message.GetComponent<RectTransform>().sizeDelta = backgroundSize;
            parentRect.sizeDelta = backgroundSize;
        }
        
        void SetMessagePositionOnScreen()
        {

            RectTransform parentRect = toastNotification.GetComponent<RectTransform>();
            Vector2 backgroundSize = background.GetComponent<RectTransform>().sizeDelta;

            if (messageScreenPosition == MessageScreenPosition.TopLeft)
            {
                parentRect.anchorMax = new Vector2(0, 1);
                parentRect.anchorMin = new Vector2(0, 1);
                parentRect.anchoredPosition = new Vector2(margin.x, -backgroundSize.y - margin.y);
            }
            else if (messageScreenPosition == MessageScreenPosition.TopRight)
            {
                parentRect.anchorMax = new Vector2(1, 1);
                parentRect.anchorMin = new Vector2(1, 1);
                parentRect.anchoredPosition = new Vector2(-backgroundSize.x - margin.x, -backgroundSize.y - margin.y);
            }
            else if (messageScreenPosition == MessageScreenPosition.TopCenter)
            {
                parentRect.anchorMax = new Vector2(0.5f, 1);
                parentRect.anchorMin = new Vector2(0.5f, 1);
                parentRect.anchoredPosition = new Vector2(-backgroundSize.x / 2, -backgroundSize.y - margin.y);
            }
            else if (messageScreenPosition == MessageScreenPosition.BottomLeft)
            {
                parentRect.anchorMax = new Vector2(0, 0);
                parentRect.anchorMin = new Vector2(0, 0);
                parentRect.anchoredPosition = new Vector2(margin.x, margin.y);
            }
            else if (messageScreenPosition == MessageScreenPosition.BottomRight)
            {
                parentRect.anchorMax = new Vector2(1, 0);
                parentRect.anchorMin = new Vector2(1, 0);
                parentRect.anchoredPosition = new Vector2(-backgroundSize.x - margin.x, margin.y);
            }
            else // Center
            {
                parentRect.anchorMax = new Vector2(0.5f, 0.5f);
                parentRect.anchorMin = new Vector2(0.5f, 0.5f);
                parentRect.anchoredPosition = new Vector2(-backgroundSize.x / 2, -backgroundSize.y / 2);
            }
        }


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

    static string Capitalize( string text )
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
