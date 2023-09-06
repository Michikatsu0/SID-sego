using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeAuthResponse : MonoBehaviour
{
    private string sceneToLoad = "GameScene";

    private Button buttonLogin;
    private Button buttonSingUp;

    DatabaseReference mDatabaseRef;

    void Start()
    {
        buttonLogin = GameObject.Find("Button_Login").GetComponent<Button>();
        buttonSingUp = GameObject.Find("Button_SignUp").GetComponent<Button>();

        buttonSingUp.onClick.AddListener(HandleSingUpButtonClicked);
        buttonLogin.onClick.AddListener(HandleLoginButtonClicked);

        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void HandleSingUpButtonClicked()
    {
        string email = GameObject.Find("InputField_Email").GetComponent<TMP_InputField>().text;
        string username = GameObject.Find("InputField_Username").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("InputField_Password").GetComponent<TMP_InputField>().text;

        StartCoroutine(RegisterUser(email, username, password));
    }

    private void HandleLoginButtonClicked()
    {
        string email = GameObject.Find("InputField_Email").GetComponent<TMP_InputField>().text;
        string username = GameObject.Find("InputField_Username").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("InputField_Password").GetComponent<TMP_InputField>().text;

        StartCoroutine(LoginUser(email, username, password));
    }

    private IEnumerator RegisterUser(string email, string username, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (registerTask.IsCanceled)
            Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
        else if (registerTask.IsFaulted)
            Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + registerTask.Exception);
        else
        {
            Firebase.Auth.AuthResult result = registerTask.Result;
            string name = GameObject.Find("InputField_Username").GetComponent<TMP_InputField>().text;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                name, result.User.UserId);

            mDatabaseRef.Child("users").Child(result.User.UserId).Child("username").SetValueAsync(username);
        }
    }

    private IEnumerator LoginUser(string email, string username, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (registerTask.IsCanceled)
            Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
        else if (registerTask.IsFaulted)
            Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + registerTask.Exception);
        else
        {
            Firebase.Auth.AuthResult result = registerTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                username, result.User.UserId);

            SceneManager.LoadScene(sceneToLoad);
        }

            
            
            
        

        
    }
}
