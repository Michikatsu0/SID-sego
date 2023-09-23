using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    private Button buttonLogin;
    private Button buttonSingUp;
    private Button restoreButton;

    private GameObject errorPanel;
    private TMP_Text errorTMP;

    private DatabaseReference mDatabaseRef;

    private void Awake()
    {
        errorPanel = GameObject.Find("Error");
        errorTMP = errorPanel.GetComponentInChildren<TMP_Text>();
        buttonLogin = GameObject.Find("Button_Login").GetComponent<Button>();
        buttonSingUp = GameObject.Find("Button_SignUp").GetComponent<Button>();
        restoreButton = GameObject.Find("Button_Restore").GetComponent<Button>();
    }
    void Start()
    {
        buttonSingUp.onClick.AddListener(HandleSingUpButtonClicked);
        buttonLogin.onClick.AddListener(HandleLoginButtonClicked);
        restoreButton.onClick.AddListener(HandleRestoreButtonClicked);
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseAuth.DefaultInstance.StateChanged += HandleAuthStateChange;
    }

    private void HandleAuthStateChange(object sender, EventArgs e)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
            SceneManager.LoadScene((int)AppScene.GAME);
    }

    private void HandleRestoreButtonClicked()
    {
        string email = GameObject.Find("InputField_Email").GetComponent<TMP_InputField>().text;

        StartCoroutine(RestorePassword(email));
    }

    private IEnumerator RestorePassword(string email)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var restoreTask = auth.SendPasswordResetEmailAsync(email);

        yield return new WaitUntil(() => restoreTask.IsCompleted);
        if (restoreTask.IsCanceled)
            Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
        else if (restoreTask.IsFaulted)
        {
            Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + restoreTask.Exception);
            SetErrorColorLabel(true);
            SetActiveError(true, restoreTask.Exception.Message);
        }
        else
        {
            SetErrorColorLabel(false);
            SetActiveError(true, "Password reset email sent successfully.");
            Debug.Log("Password reset email sent successfully.");
        }
    }

    private void HandleLoginButtonClicked()
    {
        string email = GameObject.Find("InputField_Email").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("InputField_Password").GetComponent<TMP_InputField>().text;

        StartCoroutine(LoginUser(email, password));
    }

    private IEnumerator LoginUser(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (registerTask.IsCanceled)
            Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
        else if (registerTask.IsFaulted)
        {
            Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + registerTask.Exception);
            SetErrorColorLabel(true);
            SetActiveError(true, registerTask.Exception.Message);
        }
        else
        {
            AuthResult result = registerTask.Result;
            SceneManagement.Instance.ChangeScene((int)AppScene.GAME);
        }

    }

    private void HandleSingUpButtonClicked()
    {
        string email = GameObject.Find("InputField_Email").GetComponent<TMP_InputField>().text;
        string username = GameObject.Find("InputField_Username").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("InputField_Password").GetComponent<TMP_InputField>().text;

        StartCoroutine(RegisterUser(email, username, password));
    }

    private IEnumerator RegisterUser(string email, string username, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (registerTask.IsCanceled)
            Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
        else if (registerTask.IsFaulted)
        {
            Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + registerTask.Exception);
            SetErrorColorLabel(true);
            SetActiveError(true, registerTask.Exception.Message);
        }
        else
        {
            AuthResult result = registerTask.Result;

            if (ValidUsername(username))
            {
                Debug.LogFormat(
                        "Firebase user created successfully: {0} ({1} : {2})",
                        username,
                        result.User.Email,
                        result.User.UserId);

                PlayerPrefs.SetString("UserID", result.User.UserId);
                mDatabaseRef.Child("users").Child(result.User.UserId).Child("score").SetValueAsync(0);
                mDatabaseRef.Child("users").Child(result.User.UserId).Child("username").SetValueAsync(username);


                SceneManagement.Instance.ChangeScene((int)AppScene.GAME);
            }
            else
            {
                FirebaseUser user = auth.CurrentUser;
                user.DeleteAsync();
                if (registerTask.IsCanceled)
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                else if (registerTask.IsFaulted)
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + registerTask.Exception.Message);
                else
                    Debug.Log("User deleted successfully.");
            }
        }
    }

    private bool ValidUsername(string username)
    {
        if (String.IsNullOrEmpty(username))
        {
            SetErrorColorLabel(true);
            SetActiveError(true, "An Username must be provided.");
            return false;
        }
        if (String.IsNullOrWhiteSpace(username))
        {
            SetErrorColorLabel(true);
            SetActiveError(true, "An Username Format no valid (Whitespace)");
            return false;
        }
        else
            return true;
    }

    private void SetErrorColorLabel(bool error)
    {
        Image image = errorPanel.GetComponent<Image>();
        Color color;

        if (error)
        {
            color = Color.red;
            color.a = 0.6f;
            image.color = color;
        }
        else
        {
            color = Color.green;
            color.a = 0.6f;
            image.color = color;
        }
    }

    private void SetActiveError(bool enable, string message)
    {
        errorPanel.SetActive(enable);
        errorTMP.text = message;
    }

    public void SetActiveError()
    {
        errorPanel.SetActive(false);
    }
}
