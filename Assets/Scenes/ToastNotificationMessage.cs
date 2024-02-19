using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastNotificationMessage : MonoBehaviour
{
    [HideInInspector]
    public float messageTime;
    [HideInInspector]
    public RectTransform timerRectTransform;
    [HideInInspector]
    public bool leftToRight = true;

    private float initialWidth; // Largura inicial do timer
    float timeElapsed;

    void Start()
    {
        messageTime = messageTime <= 0 ? ToastNotification.static_minimumMessageTime : messageTime;

        RectTransform messageRect = transform.parent.GetComponent<RectTransform>();
        timerRectTransform.sizeDelta = new Vector2(messageRect.sizeDelta.x, messageRect.sizeDelta.y * 0.07f );

        timerRectTransform.anchorMin = new Vector2(1,1);
        timerRectTransform.anchorMax = new Vector2(1,1);
        timerRectTransform.pivot = new Vector2(1,1);
        timerRectTransform.anchoredPosition = Vector3.zero;

        initialWidth = timerRectTransform.sizeDelta.x;
    }

    void FixedUpdate()
    {

        if (ToastNotification.isStoped == true)
            return;

        if (timeElapsed > messageTime)
            Hide();
        

        timeElapsed += Time.deltaTime;

        RenderTimer();

    }

    void RenderTimer()
    {
        // Calcula a porcentagem do tempo restante em relação ao tempo total
        float remainingTimePercentage = Mathf.Clamp01(1f - (timeElapsed / messageTime));

        // Calcula a nova largura do timer com base na porcentagem do tempo restante
        float newWidth = initialWidth * remainingTimePercentage;

        timerRectTransform.sizeDelta = new Vector2(newWidth, timerRectTransform.sizeDelta.y);
        if (leftToRight == false)
        {
            timerRectTransform.anchoredPosition = new Vector2(-initialWidth + timerRectTransform.sizeDelta.x, timerRectTransform.anchoredPosition.y );
        }

    }

    public void Hide()
    {
        Destroy(gameObject);
    }

    public void HideOnClick()
    {
        if (ToastNotification.static_hideOnClick)
            Hide();
    }

}
