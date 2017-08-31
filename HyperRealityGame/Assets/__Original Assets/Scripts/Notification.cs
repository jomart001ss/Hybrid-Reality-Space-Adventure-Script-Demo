using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Notification : MonoBehaviour
{
    public void Init (GameObject notificationPlate, float seconds, Transform parent)
    {
        StartCoroutine(NotifyPlayerCoroutine(notificationPlate, seconds, parent));
    }

    public bool dontDelay;

    IEnumerator NotifyPlayerCoroutine(GameObject notification, float seconds, Transform parent)
    {

        GameObject notificationInst;
        Transform container = SpawnAndMoveNotification(notification, parent, out notificationInst);
        Image image = null;
        Text text = null;
        GetUIComponents(notificationInst, ref image, ref text);
        float originalImageAlpha = image.color.a;
        float originalTextAlpha = text.color.a;
        yield return StartCoroutine(TransitionIn(container, image, text,
                                                   originalImageAlpha, originalTextAlpha));
        float timeLeft = seconds - 0.6f;
        do
        {
            timeLeft -= Time.unscaledDeltaTime;
            yield return null;
        }
        while (timeLeft > 0 && !dontDelay);
        yield return StartCoroutine(TransitionOut(container, image, text,
                                                   originalImageAlpha, originalTextAlpha));
        Destroy(container.gameObject);
        Destroy(this);
    }

    public float movementDist = 3;

    Transform SpawnAndMoveNotification(GameObject notification, Transform parent, out GameObject notificationInst)
    {
        notificationInst = Instantiate(notification) as GameObject;
        GameObject containerGO = new GameObject("Notification") as GameObject;
        Transform container = containerGO.GetComponent<Transform>();
        RectTransform rectTransform = notificationInst.GetComponent<RectTransform>();
        rectTransform.SetParent(container);
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
        container.SetParent(parent);
        container.localRotation = Quaternion.identity;
        return container;
    }

    void GetUIComponents(GameObject notificationInst, ref Image image, ref Text text)
    {
        foreach (Transform child in notificationInst.transform)
        {
            Image _image = child.GetComponent<Image>();
            image = _image ?? image;
            Text _text = child.GetComponent<Text>();
            text = _text ?? text;
        }
    }

    IEnumerator TransitionIn(Transform container, Image image, Text text,
                              float originalImageAlpha, float originalTextAlpha)
    {
        Vector3 startPosition = new Vector3(-movementDist, 0, 0);
        float lerp = 0;
        while (lerp != 1)
        {
            lerp = Mathf.MoveTowards(lerp, 1, 2 * Time.unscaledDeltaTime);
            UpdateNotificationAlpha(lerp, image, text, 0, originalImageAlpha, 0, originalTextAlpha);
            container.localPosition = Vector3.Lerp(startPosition, Vector3.zero, lerp);
            yield return null;
        }
    }

    void UpdateNotificationAlpha(float lerp, Image image, Text text,
                                  float imageStart, float imageGoal, float textStart, float textGoal)
    {
        Color newColor = image.color;
        newColor.a = Mathf.Lerp(imageStart, imageGoal, lerp);
        image.color = newColor;
        newColor = text.color;
        newColor.a = Mathf.Lerp(textStart, textGoal, lerp);
        text.color = newColor;
    }

    IEnumerator TransitionOut(Transform container, Image image, Text text,
                              float originalImageAlpha, float originalTextAlpha)
    {
        Vector3 goalPosition = new Vector3(movementDist, 0, 0);
        float lerp = 0;
        while (lerp != 1)
        {
            lerp = Mathf.MoveTowards(lerp, 1, 2 * Time.unscaledDeltaTime);
            UpdateNotificationAlpha(lerp, image, text, originalImageAlpha, 0, originalTextAlpha, 0);
            container.localPosition = Vector3.Lerp(Vector3.zero, goalPosition, lerp);
            yield return null;
        }
    }
}
