using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameAuthResponse : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> usernameLabel = new List<TMP_Text>();
    private string sceneToLoad = "HomeAuth";

    void Start()
    {
        FirebaseAuth.DefaultInstance.StateChanged += HandleAuthChange;
    }

    private void HandleAuthChange(object sender, EventArgs e)
    {
        var currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        if (currentUser != null)
        {
            usernameLabel[0].text = "Bienvenido " + currentUser.Email;
            usernameLabel[1].text = "Score: " + 0;
        }
            
    }

    public void SignOut()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene(sceneToLoad);
    }

    public void PlayGame()
    {
        
    }
}
