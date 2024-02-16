using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToastNotificationGameObjects : MonoBehaviour
{

    static List<Transform> messages;
    public static Transform messagePrefab;
    static Transform este;

    void Start()
    {
        messages = new List<Transform>();

        Show("Texto de testes aqui");

    }

    void Update()
    {
    }

    public static void Show( string text )
    {
        Transform message = createObjects();
        messages.Add(message);

        Vector2 referenceResolution = FindObjectOfType<CanvasScaler>().referenceResolution;
        Vector3 centerPosition = new Vector3(referenceResolution.x / 2, referenceResolution.y / 2, 0);
        message.GetComponent<RectTransform>().localPosition = centerPosition;

        TextMeshProUGUI messageText = message.Find("Text").GetComponent<TextMeshProUGUI>();
        Image background = message.Find("Background").GetComponent<Image>();
        Image icon = message.Find("Icon").GetComponent<Image>();
        Image timer = message.Find("Timer").GetComponent<Image>();

        background.GetComponent<RectTransform>().sizeDelta = new Vector2(950, 140);
        messageText.text = text;
        messageText.color = Color.black;
        icon.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
        icon.transform.position = new Vector2( background.transform.position.x * 0.50f, background.transform.position.y);
        icon.color = Color.black;


    }

    private static Transform createObjects()
    {
        Transform canvas = GameObject.FindObjectOfType<Canvas>().transform;

        GameObject messageObject = new GameObject("Message");
        messageObject.AddComponent<RectTransform>();
        Transform message = messageObject.transform;
        message.SetParent(canvas,false);

        GameObject background = new GameObject("Background");
        background.AddComponent<Image>();
        background.transform.SetParent(message, false);
        background.transform.position = Vector3.zero;

        GameObject text = new GameObject("Text");
        text.AddComponent<TextMeshProUGUI>();
        text.transform.SetParent(message, false);
        text.transform.position = Vector3.zero;

        GameObject icon = new GameObject("Icon");
        icon.AddComponent<Image>();
        icon.transform.SetParent(message, false);
        icon.transform.position = Vector3.zero;

        GameObject timer = new GameObject("Timer");
        timer.AddComponent<Image>();
        timer.transform.SetParent(message, false);
        timer.transform.position = Vector3.zero;

        return message;

    }

}
