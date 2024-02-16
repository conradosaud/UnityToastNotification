using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToastNotification : MonoBehaviour
{

    static List<Transform> messages;
    public Transform messagePrefab;

    float elapsedTime = 0;

    void Start()
    {

        messages = new List<Transform>();
        Show("Texto de testes aqui", "Info", false);

    }

    void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;
    }

    public void Show( string messageText, string iconName = "", bool darkTheme = true, int timerInSeconds = 3 )
    {

        Transform canvas = transform.parent;
        Transform message = Instantiate(messagePrefab, canvas);

        TextMeshProUGUI text = message.Find("Text").GetComponent<TextMeshProUGUI>();
        Image background = message.Find("Background").GetComponent<Image>();
        Transform icons = message.Find("Icons");
        Image timer = message.Find("Timer").GetComponent<Image>();

        text.text = messageText;

        Image selectedIcon = null;
        if( iconName != "")
        {
            //iconName = iconName.Substring(0,1).ToUpper() + iconName.Substring(1).ToLower();
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

        text.color = foreColor;
        background.color = backgroundColor;
        if (selectedIcon != null)
            selectedIcon.color = foreColor;

        StartCoroutine( Hide( message.gameObject, timerInSeconds) );

    }

    IEnumerator Hide( GameObject message, int time )
    {
        yield return new WaitForSeconds(time);
        Destroy(message);
    }

}
