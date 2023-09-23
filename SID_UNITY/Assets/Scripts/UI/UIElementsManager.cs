using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElementsManager : MonoBehaviour
{
    public static UIElementsManager Instance;
    [SerializeField] private GameObject[] uIElementsList;
    private void Awake()
    {
        Instance = this;
    }

    public void DisableUI()
    {
        foreach (var element in uIElementsList) {
            element.SetActive(false);
        }
    }
}
