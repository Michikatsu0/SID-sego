using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 rectTransfromVector = new Vector2(0f, 70);
    private void Awake()
    {
        rectTransform = GameObject.Find("InputField_Password").GetComponent<RectTransform>();
    }
    void Start()
    {
        UIElementsManager.Instance.DisableUI();
    }

    public void RegisterButton()
    {
        rectTransform.anchoredPosition -= rectTransfromVector;
    }

    public void ReturnButton()
    {
        rectTransform.anchoredPosition += rectTransfromVector;
    }
}
