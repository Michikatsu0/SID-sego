using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeAuthResponse : MonoBehaviour
{
    private string sceneToLoad = "SceneGame";

    private Button buttonLogin;
    private Button buttonSingUp;

    private TMP_InputField inputFieldEmail;
    private TMP_InputField inputFieldPassword;

    void Start()
    {
        buttonLogin = GameObject.Find("Button_Login").GetComponent<Button>();
        buttonSingUp = GameObject.Find("Button_SignUp").GetComponent<Button>();
        inputFieldEmail = GameObject.Find("InputField_Email").GetComponent<TMP_InputField>();
        inputFieldPassword = GameObject.Find("InputField_Password").GetComponent<TMP_InputField>();

        buttonSingUp.onClick.AddListener(HandleSingUpButtonClicked);
        buttonLogin.onClick.AddListener(HandleLoginButtonClicked);
    }

    private void HandleSingUpButtonClicked()
    {
        var auth = FirebaseAuth.DefaultInstance;
        auth.CreateUserWithEmailAndPasswordAsync(inputFieldEmail.text, inputFieldPassword.text).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
        });
    }

    private void HandleLoginButtonClicked()
    {
        var auth = FirebaseAuth.DefaultInstance;
        auth.SignInWithEmailAndPasswordAsync(inputFieldEmail.text, inputFieldPassword.text).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            SceneManager.LoadScene(sceneToLoad);
        });

        
    }
}
