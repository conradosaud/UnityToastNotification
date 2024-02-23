using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

/// <summary>
/// Controle do objeto ToastNotification da Hierarquia.
/// Você pode alterar as variáveis estáticas públicas para customizar a forma com que as mensagens aparecem na tela,
/// mas ao fazer isso, você irá sobrepor a configuração de todas as mensagens seguintes.
/// </summary>
public class ToastNotification : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    // The prefab used to display messages
    public Transform _messagePrefab;

    // Public static variables accessible throughout the project
    // Be careful when changing them at runtime, as as static variables, this will override existing settings
    public static bool isStoped = false;
    public static bool showTimerRender;
    public static TimerDirection timerDirection;
    public static MessageScreenPosition messageScreenPosition;
    public static Vector2 margin;
    public static bool darkTheme;
    public static float minimumMessageTime = 3;
    public static bool hideOnClick = true;
    public static bool isHiding = false;
    public static bool isCanvasGroup = false;

    // Private static variables
    private static Transform messagePrefab;
    private static Transform toastNotification;

    // Default message patterns configurable in the Unity Editor
    [Header("Default Message Patterns:")]
    [Tooltip("A countdown image will be displayed on message as a timer")]
    public bool _showTimerRender = true;
    [Tooltip("Disable it to use the default Light Theme on messages")]
    public bool _darkTheme = true;
    [Tooltip("Minimun time that all messages will be displayed.")]
    public float _minimumMessageTime = 3;
    [Tooltip("Margin X and Y on the corners. Margin X doens't works with centralized messages.")]
    public Vector2 _margin = new Vector2(20, 20);
    [Tooltip("Stop the timer when mouse cursor is over the ToastNotification object")]
    public bool _stopOnOver = true;
    [Tooltip("Hide/dismiss the message when it's clicked")]
    public bool _hideOnClick = true;
    [Tooltip("Position of messages on screen")]
    public enum MessageScreenPosition { TopLeft, TopCenter, TopRight, Center, BottomLeft, BottomCenter, BottomRight }
    public MessageScreenPosition _messageScreenPosition = MessageScreenPosition.TopRight;
    [Tooltip("Direction of timer countdown. Auto will choose the best position relative to the Message Screen Position option.")]
    public enum TimerDirection { LeftToRight, RightToLeft}
    public TimerDirection _timerDirection = TimerDirection.LeftToRight;

    // Awake function called when the script instance is being loaded
    // You can change Awake to Start if this is causing problems with other scripts in your game
    void Awake()
    {

        // Assign public variables to their static counterparts
        messagePrefab = _messagePrefab;
        toastNotification = transform;

        minimumMessageTime = _minimumMessageTime;
        hideOnClick = _hideOnClick;
        darkTheme = _darkTheme;
        showTimerRender = _showTimerRender;
        timerDirection = _timerDirection;
        messageScreenPosition = _messageScreenPosition;
        margin = _margin;

        // Setup the ToastNotification object
        setupToastNotificationObject();
        void setupToastNotificationObject(){
            // Check if the ToastNotification object has a CanvasGroup component
            // If you don't add it, there will be no fade animations in your messages
            if (toastNotification.GetComponent<CanvasGroup>())
                isCanvasGroup = true;

            // Set the RectTransform properties for positioning
            // The messages' parent object must be completely reset to function correctly
            toastNotification.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            toastNotification.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
            toastNotification.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
        }

    }

    // You can change FixedUpdate to Update. But I recommend keeping it this way to consume less processing
    private void FixedUpdate()
    {

        // Cancel if CanvasGroup is not present in ToastNotification object
        if (!isCanvasGroup)
            return;

        // If isHiding flag, gradually decrease CanvasGroup alpha
        if ( isHiding)
        {
            toastNotification.GetComponent<CanvasGroup>().alpha -= 0.08f;
            if (toastNotification.GetComponent<CanvasGroup>().alpha < 0.01f)
            {
                // Call Hide function and reset isHiding flag
                Hide();
                isHiding = false;
            }
        }
        else if(toastNotification.GetComponent<CanvasGroup>().alpha < 1)
        {
            // Gradually increase CanvasGroup alpha
            toastNotification.GetComponent<CanvasGroup>().alpha += 0.05f;
        }

    }

    // Interface function triggered when mouse pointer enters the object
    public void OnPointerEnter(PointerEventData eventData)
    {
        // If stopOnOver is enabled, stop the timer
        if ( _stopOnOver )
            isStoped = true;
    }
    // Interface function triggered when mouse pointer exits the object
    public void OnPointerExit(PointerEventData eventData)
    {
        // If stopOnOver is enabled, resume the timer
        if ( _stopOnOver )
            isStoped = false;
    }

    // ----------- OVERLOADS ----------- 
    // Message text is the minimun necessary
    public static void Show(string messageText)
    {
        Show(messageText, minimumMessageTime, "");
    }
    // Text and timer
    public static void Show(string messageText, float timerInSeconds)
    {
        Show(messageText, timerInSeconds, "");
    }
    // Text and icon
    public static void Show(string messageText, string iconName)
    {
        Show(messageText, minimumMessageTime, iconName);
    }
    // ---------------------------------- 

    public static void Show( string messageText, float timerInSeconds = -1, string iconName = "")
    {

        // Hide any existing messages
        Hide();

        // If timerInSeconds is not provided, set it to the default minimumMessageTime
        if ( timerInSeconds <= -1 )
            timerInSeconds = minimumMessageTime;

        // Instantiate message prefab and configure it
        Transform message = Instantiate(messagePrefab, toastNotification);
        message.gameObject.SetActive(true);
        message.name = "Message"; // <- You can change the name of messages that are created here
        if ( isCanvasGroup ) toastNotification.GetComponent<CanvasGroup>().alpha = 0;

        TextMeshProUGUI text = message.Find("Text").GetComponent<TextMeshProUGUI>();
        UnityEngine.UI.Image background = message.Find("Background").GetComponent<UnityEngine.UI.Image>();
        Transform icons = message.Find("Icons");
        UnityEngine.UI.Image timer = message.Find("Timer").GetComponent<UnityEngine.UI.Image>();
        UnityEngine.UI.Image selectedIcon = null;

        Vector2 backgroundSize = background.GetComponent<RectTransform>().sizeDelta;
        RectTransform parentRect = toastNotification.GetComponent<RectTransform>();

        // Set message text, icon, color, and position
        SetText();
        SetMessageIcon();
        SetMessageColor();
        SetupInvokeMessage();
        ResetToastNoticationPosition();
        SetMessagePositionOnScreen();

        void SetText()
        {
            text.text = messageText;
            text.alignment = TextAlignmentOptions.MidlineLeft;
            if( messageScreenPosition == MessageScreenPosition.Center )
                text.alignment = TextAlignmentOptions.Center;
        }

        void SetMessageIcon()
        {
            if( iconName != "") 
            {
                iconName = Capitalize(iconName);
                selectedIcon = icons.Find(iconName).transform.GetComponent<UnityEngine.UI.Image>();
                selectedIcon.enabled = true;
            }
            else
            {
                float iconSize = icons.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
                backgroundSize = new Vector2( backgroundSize.x - iconSize - iconSize / 2 , backgroundSize.y);
                background.GetComponent<RectTransform>().sizeDelta = backgroundSize;
                Vector2 newAnchor = background.GetComponent<RectTransform>().anchoredPosition;
                newAnchor = new Vector2(newAnchor.x - iconSize, newAnchor.y);
                toastNotification.GetComponent<RectTransform>().anchoredPosition = newAnchor;
                text.GetComponent<RectTransform>().sizeDelta *= 0.90f ;
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
            {
                selectedIcon.color = new Color( foreColor.r, foreColor.g, foreColor.b, 0.7f );
            }
            
        }

        void SetupInvokeMessage()
        {
            ToastNotificationMessage toastNotificationMessage = message.GetComponent<ToastNotificationMessage>();
            toastNotificationMessage.timerRectTransform = timer.GetComponent<RectTransform>();
            toastNotificationMessage.messageTime = timerInSeconds;

            timer.enabled = showTimerRender;
            timer.enabled = timerInSeconds == 0 ? false : timer.enabled;
            toastNotificationMessage.leftToRight = timerDirection == TimerDirection.LeftToRight;

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
            else if (messageScreenPosition == MessageScreenPosition.BottomCenter)
            {
                parentRect.anchorMax = new Vector2(0.5f, 0);
                parentRect.anchorMin = new Vector2(0.5f, 0);
                parentRect.anchoredPosition = new Vector2(-backgroundSize.x / 2, margin.y);
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
        if (toastNotification.childCount <= 0)
            return;
        for (int i = 0; i < toastNotification.childCount; i++)
        {
            if (toastNotification.GetChild(i).gameObject.activeSelf == true)
            {
                Destroy(toastNotification.GetChild(i).gameObject);
            }
        }
    }

    #region Utilities Functions

    public static string Capitalize( string text )
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
