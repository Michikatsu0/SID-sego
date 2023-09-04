using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AuthHandler : MonoBehaviour
{
    public string apiUrl = "https://sid-restapi.onrender.com/api/";
    private TMP_InputField usernameInputField;
    private TMP_InputField passwordInputField;

    private string token;
    private string username;

    void Start()
    {
        token = PlayerPrefs.GetString("token");

        if (string.IsNullOrEmpty(token))
            Debug.Log("No hay token almacenado");
        else
        {
            username = PlayerPrefs.GetString("username");
            StartCoroutine(GetProfile(username));
        }

        usernameInputField = GameObject.Find("Username_InputField").gameObject.GetComponent<TMP_InputField>();
        passwordInputField = GameObject.Find("Password_InputField").gameObject.GetComponent<TMP_InputField>();
    }

    public void Register()
    {
        AuthData authData = new AuthData();
        authData.username = usernameInputField.text;
        authData.password = passwordInputField.text;

        string json = JsonUtility.ToJson(authData);

        StartCoroutine(SendRegister(json));
    }

    public void Login()
    {
        AuthData authData = new AuthData();
        authData.username = usernameInputField.text;
        authData.password = passwordInputField.text;

        string json = JsonUtility.ToJson(authData);

        StartCoroutine(SendLogin(json));
    }

    IEnumerator GetProfile(string username)
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + "usuarios/" + username);
        request.SetRequestHeader("x-token", token);
        yield return request.SendWebRequest();

        if (request.isNetworkError)
            Debug.Log("NETWORK ERROR: " + request.error);
        else
        {
            Debug.Log(request.downloadHandler.text);
            if (request.responseCode == 200)
            {
                AuthData data = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);
                Debug.Log("Sesión activa del ususario: " + data.usuario.username);
                Debug.Log("Su Score es: " + data.usuario.data.score);
                SceneManager.LoadScene("Game");
            }
            else
                Debug.Log(request.error);
        }
    }

    IEnumerator SendRegister(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put(apiUrl + "usuarios", json);
        request.SetRequestHeader("Content-Type", "application/json");
        request.method = "POST";
        yield return request.SendWebRequest();

        if (request.isNetworkError)
            Debug.Log("NETWORK ERROR: " + request.error);
        else 
        {
            Debug.Log(request.downloadHandler.text);
            if (request.responseCode == 200)
            {
                AuthData data = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);
                Debug.Log("Se registro el usuario con id: " + data.usuario._id);
                PlayerPrefs.SetString("username", data.usuario.username);
            }
            else
                Debug.Log(request.error);
        }
    }

    IEnumerator SendLogin(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put(apiUrl + "auth/login", json);
        request.SetRequestHeader("Content-Type", "application/json");
        request.method = "POST";
        yield return request.SendWebRequest();

        if (request.isNetworkError)
            Debug.Log("NETWORK ERROR: " + request.error);
        else
        {
            Debug.Log(request.downloadHandler.text);
            if (request.responseCode == 200)
            {
                AuthData data = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);
                Debug.Log("Inicio de sesión del usuario: " + data.usuario.username);
                Debug.Log(data.token);
                PlayerPrefs.SetString("token", data.token);
                PlayerPrefs.SetString("username", data.usuario.username);
                SceneManager.LoadScene("Game");
            }
            else
                Debug.Log(request.error);
        }
    }
}

[System.Serializable]
public class AuthData
{
    public string username;
    public string password;
    public User usuario;
    public string token;
}

[System.Serializable]
public class User
{
    public string _id;
    public string username;
    public bool estado;
    public DataUser data;
}

[System.Serializable]
public class DataUser
{
    public int score;
}