using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    public static HomeManager Instance;
    [SerializeField] private TMP_Text[] usernamesLabel;
    [SerializeField] private TMP_Text[] scoresLabel;

    [SerializeField] private TMP_Text usernameLabel;
    [SerializeField] private TMP_Text uRScoreLabel;
    [SerializeField] private Button topScoreLabel;

    private FireDataUser[] dataUsers = new FireDataUser[5];
    private Dictionary<string, int> usersDic = new Dictionary<string, int>();

    DatabaseReference mDatabase;
    private int iterations = 5;
    [HideInInspector] public int score;
    private void Awake()
    {
        Instance = this;
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        var auth = FirebaseAuth.DefaultInstance;
        GetUsername(auth);
        GetScore(auth);
    }

    void Start()
    {
        topScoreLabel.onClick.AddListener(GetUsersHighestScores);
        UIElementsManager.Instance.DisableUI();
    }

    public void GetUsername(FirebaseAuth auth)
    {
        FirebaseDatabase.DefaultInstance
        .GetReference("users/" + auth.CurrentUser.UserId + "/username")
        .GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
                Debug.Log(task.Exception);
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                usernameLabel.text = "Welcome " + (string)snapshot.Value;
                Debug.Log(usernameLabel.text);
            }
        });
    }

    public void GetScore(FirebaseAuth auth)
    {
        FirebaseDatabase.DefaultInstance
        .GetReference("users/" + auth.CurrentUser.UserId + "/score")
        .GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
                Debug.Log(task.Exception);
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                uRScoreLabel.text = "Your Best Score: " + snapshot.Value.ToString();
                Debug.Log(usernameLabel.text);
                score = int.Parse(snapshot.Value.ToString());
            }
        });
    }


    public void GetUsersHighestScores()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByValue().GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log(task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    foreach (var userDoc in (Dictionary<string, object>)snapshot.Value)
                    {
                        var userObject = (Dictionary<string, object>)userDoc.Value;
                        string username = (string)userObject["username"];
                        int score = Convert.ToInt32(userObject["score"]);

                        if (usersDic.ContainsKey(username))
                        {
                            if (usersDic[username] < score)
                                usersDic[username] = score;
                        }
                        else
                            usersDic.Add(username, score);
                    }

                    var sortedUsers = usersDic.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                    var list_Users_Name = sortedUsers.Keys.ToList();
                    var list_Users_Score = sortedUsers.Values.ToList();

                    for (int i = 0; i < iterations; i++)
                    {
                        FireDataUser user = new FireDataUser();
                        user.username = list_Users_Name[i];
                        user.score = list_Users_Score[i];
                        dataUsers[i] = user;
                        usernamesLabel[i].text = (i + 1) + ". " + dataUsers[i].username;
                        scoresLabel[i].text = dataUsers[i].score.ToString();

                    }
                }
            });
    }

    public void SignOut()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene((int)AppScene.LOGIN);
    }
}

[System.Serializable]
public class FireDataUser
{
    public string username;
    public int score;
}