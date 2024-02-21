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
using static ToastNotification;

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
    private static MessageSize messageSize;
    public static MessageScreenPosition messageScreenPosition;
    private static Vector2 margin;
    public static bool darkTheme;
    public static float minimumMessageTime = 3;
    public static bool hideOnClick = true;
    private static float icon_size = 90;

    public static bool isHiding = false;

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
    [Tooltip("Direction of timer countdown. Auto will choose the best position relative to the Message Screen Position option.")]
    public enum MessageSize { Normal, Small }
    public MessageSize _messageSize = MessageSize.Normal;

    public int limitOnScreen = 5;

    void Start()
    {

        minimumMessageTime = _minimumMessageTime;
        hideOnClick = _hideOnClick;

        messagePrefab = _messagePrefab;
        toastNotification = transform;
        darkTheme = _darkTheme;
        showTimerRender = _showTimerRender;
        messageSize = _messageSize;
        timerDirection = _timerDirection;
        messageScreenPosition = _messageScreenPosition;
        margin = _margin;

        _messagePrefab.gameObject.SetActive(false);

        //Show("Texto de testes aqui", tempoDEV, "info", darkTheme);
        Show("Minha mensagem aqui hihih", false, minimumMessageTime, "Alert");

    }

    private void FixedUpdate()
    {
          
        if( isHiding)
        {
            toastNotification.GetComponent<CanvasGroup>().alpha -= 0.08f;
            if (toastNotification.GetComponent<CanvasGroup>().alpha < 0.01f)
            {
                Hide();
                isHiding = false;
            }
        }
        else if(toastNotification.GetComponent<CanvasGroup>().alpha < 1)
        {
            toastNotification.GetComponent<CanvasGroup>().alpha += 0.05f;
        }

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


    // Message text is the minimun necessary
    public static void Show( string messageText)
    {
        Show( messageText, false, minimumMessageTime, "" );
    }
    // Text and timer only
    public static void Show(string messageText, float timerInSeconds)
    {
        Show(messageText, false, timerInSeconds, "");
    }
    // Text and icon only
    public static void Show(string messageText, string iconName)
    {
        Show(messageText, false, minimumMessageTime, iconName);
    }
    // Text, timer and icon
    public static void Show(string messageText, float timerInSeconds, string iconName)
    {
        Show(messageText, false, timerInSeconds, iconName);
    }

    // SMALL MESSAGE - Message text is the minimun necessary
    // Message text is the minimun necessary
    public static void ShowSmall(string messageText)
    {
        Show(messageText, true, 300, "");
    }
    // Text and timer only
    public static void ShowSmall(string messageText, float timerInSeconds)
    {
        Show(messageText, true, timerInSeconds, "");
    }
    // Text and icon only
    public static void ShowSmall(string messageText, string iconName)
    {
        Show(messageText, true, minimumMessageTime, iconName);
    }
    // Text, timer and icon
    public static void ShowSmall(string messageText, float timerInSeconds, string iconName)
    {
        Show(messageText, true, timerInSeconds, iconName);
    }

    public static void Show( string messageText, bool small, float timerInSeconds, string iconName)
    {

        Hide();

        Transform message = Instantiate(messagePrefab, toastNotification);
        message.gameObject.SetActive(true);
        message.name = "Message";
        toastNotification.GetComponent<CanvasGroup>().alpha = 0;

        TextMeshProUGUI text = message.Find("Text").GetComponent<TextMeshProUGUI>();
        UnityEngine.UI.Image background = message.Find("Background").GetComponent<UnityEngine.UI.Image>();
        Transform icons = message.Find("Icons");
        UnityEngine.UI.Image timer = message.Find("Timer").GetComponent<UnityEngine.UI.Image>();
        UnityEngine.UI.Image selectedIcon = null;

        Vector2 backgroundSize = background.GetComponent<RectTransform>().sizeDelta;
        RectTransform parentRect = toastNotification.GetComponent<RectTransform>();
        float customIconSize = icon_size;

        SetText();
        SetMessageSize();
        SetMessageIcon();
        SetMessageColor();
        SetupInvokeMessage();
        ResetToastNoticationPosition();
        SetMessagePositionOnScreen();

        void SetText()
        {
            text.text = messageText;
            if (messageScreenPosition == MessageScreenPosition.TopLeft || messageScreenPosition == MessageScreenPosition.BottomLeft)
                text.alignment = TextAlignmentOptions.MidlineRight;
            else if(messageScreenPosition == MessageScreenPosition.TopRight || messageScreenPosition == MessageScreenPosition.BottomRight)
                text.alignment = TextAlignmentOptions.MidlineLeft;
            else
                text.alignment = TextAlignmentOptions.Center;
        }

        void SetMessageSize()
        {
            if (messageSize != MessageSize.Normal || small == true)
            {
                text.GetComponent<RectTransform>().sizeDelta = new Vector2(backgroundSize.x / 2f, backgroundSize.y / 2);
                text.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                text.fontSize = text.fontSize / 2;
                backgroundSize = new Vector2(backgroundSize.x / 1.5f, backgroundSize.y / 2);
                background.GetComponent<RectTransform>().sizeDelta = backgroundSize;
                toastNotification.GetComponent<RectTransform>().anchoredPosition = backgroundSize;
                customIconSize = customIconSize / 2.5f;
            }
        }

        void SetMessageIcon()
        {
            if( iconName != "") 
            {
                iconName = Capitalize(iconName);
                selectedIcon = icons.Find(iconName).transform.GetComponent<UnityEngine.UI.Image>();
                selectedIcon.enabled = true;
                selectedIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(customIconSize, customIconSize);
                if (messageSize != MessageSize.Normal || small == true)
                    selectedIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(-backgroundSize.x / 2 + customIconSize, 0);
            }
            else
            {
                backgroundSize = new Vector2( backgroundSize.x - customIconSize - customIconSize / 2 , backgroundSize.y);
                background.GetComponent<RectTransform>().sizeDelta = backgroundSize;
                Vector2 newAnchor = background.GetComponent<RectTransform>().anchoredPosition;
                newAnchor = new Vector2(newAnchor.x - customIconSize, newAnchor.y);
                toastNotification.GetComponent<RectTransform>().anchoredPosition = newAnchor;
                text.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
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
            timer.enabled = timerInSeconds == 0 ? false : timer.enabled;

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

    public static void Hide()
    {
        for (int i = 0; i < toastNotification.childCount; i++)
        {
            if (toastNotification.GetChild(i).gameObject.activeSelf == true)
            {
                Destroy(toastNotification.GetChild(i).gameObject);
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
