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
    bool leftToRight = true;

    private float initialWidth; // Largura inicial do timer
    float timeElapsed;

    void Start()
    {
        messageTime = messageTime <= 0 ? 0 : ToastNotification.minimunMessageTime;
        initialWidth = timerRectTransform.rect.width;
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

        float newXPosition = (initialWidth - newWidth) / 2f;
        timerRectTransform.sizeDelta = new Vector2(-newWidth, timerRectTransform.sizeDelta.y);

        if (leftToRight == false)
        {
            newXPosition = -(initialWidth - newWidth) / 2f;
            timerRectTransform.sizeDelta = new Vector2(newWidth, timerRectTransform.sizeDelta.y);
        }

        // Define a nova largura e posição X do timer
        timerRectTransform.sizeDelta = new Vector2(newWidth, timerRectTransform.sizeDelta.y);
        timerRectTransform.anchoredPosition = new Vector2(newXPosition, timerRectTransform.anchoredPosition.y);
    }

    void Hide()
    {
        Destroy(gameObject);
    }

}
