using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ScoreHandler : MonoBehaviour
{
    public string apiUrl = "https://sid-restapi.onrender.com/api/";
    public GameObject panelScore;

    [SerializeField] private TMP_Text[] myUsernameLabel;
    [SerializeField] private TMP_Text[] myScoreLabel;

    private TMP_InputField scoreInputField;
    private string username;
    private string token;
    private int bestNumber = 5;
    private User[] scoreList;

    void Start()
    {
        scoreInputField = GameObject.Find("Score_InputField").GetComponent<TMP_InputField>();
        token = PlayerPrefs.GetString("token");
        if (string.IsNullOrEmpty(token))
        {
            Debug.Log("No hay token almacenado");
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene("Home");

        }
        else
            username = PlayerPrefs.GetString("username");
    }


    public void SetScore()
    {
        if (int.TryParse(scoreInputField.text, out int result))
        {
            ScoreUser scoreUser = new ScoreUser();
            scoreUser.username = username;
            scoreUser.data = new DataUser();
            scoreUser.data.score = result;
           
            string json = JsonUtility.ToJson(scoreUser);

            StartCoroutine(SendScore(json));
        }
    }

    public void OpenScores()
    {
        StartCoroutine(GetScores());
        panelScore.SetActive(true);
    }

    public void CloseScores()
    {
        panelScore.SetActive(false);
    }

    public void ExitAccount()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Home");
    }

    IEnumerator SendScore(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put(apiUrl + "usuarios", json);
        request.SetRequestHeader("Content-Type", "application/json");
        request.method = "PATCH";
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
                Debug.Log("Ususario " + data.usuario.username + " su nuevo score es: " + data.usuario.data.score);
            }
            else
                Debug.Log(request.error);
        }
    }

    IEnumerator GetScores()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + "usuarios");
        request.SetRequestHeader("x-token", token);
        yield return request.SendWebRequest();

        if (request.isNetworkError)
            Debug.Log("NETWORK ERROR: " + request.error);
        else
        {
            Debug.Log(request.downloadHandler.text);
            if (request.responseCode == 200)
            {
                UserList userList = JsonUtility.FromJson<UserList>(request.downloadHandler.text);
                scoreList = userList.usuarios.OrderByDescending(u=>u.data.score).Take(bestNumber).ToArray();

                for (int i = 0; i < bestNumber; i++)
                {
                    myUsernameLabel[i].text = scoreList[i].username;
                    myScoreLabel[i].text = scoreList[i].data.score.ToString();
                }
            }
            else
                Debug.Log(request.error);
        }
    }
}

[System.Serializable]
public class ScoreUser
{
    public string username;
    public DataUser data;
}

[System.Serializable]
public class UserList
{
    public User[] usuarios;
}