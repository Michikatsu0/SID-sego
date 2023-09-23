using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game_Manager : MonoBehaviour
{
    public static Game_Manager instance;

    public int currentScore;

    [HideInInspector] public int score;
    public GameObject InGameCanvas;
    public GameObject StartScreenCanvas;
    public GameObject DeadCanvas;

    public GameObject flappyBird;

    //propertys that cannot be changed by other classes
    public static bool IsPaused{ get; private set; }

    private bool isGameStarted = false;

    private DatabaseReference mDatabaseRef;
    public bool IsGameStarted
    {
        get { return isGameStarted; }
        set
        {
            GenerateScenario.isGameStarted = value;
            FlappyBirdController.isGameStarted = value;
            isGameStarted = value;
        }
    }

    void Awake()
    {
        IsPaused = false;
        instance = this;
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void Home()
    {
        IsGameStarted = false;
        GenerateScenario.instance.CleanAllBackground();
        flappyBird.GetComponent<FlappyBirdController>().Start();

        DeadCanvas.SetActive(false);
        InGameCanvas.SetActive(false);
        StartScreenCanvas.SetActive(true);

        flappyBird.transform.parent.position = new Vector3(0, -0.22f, -10);
    }


    public void StartGame()
    {
        InGameCanvas.transform.Find("ScoreTextWhite").GetComponent<Text>().text = currentScore.ToString();
        StartScreenCanvas.SetActive(false);
        DeadCanvas.SetActive(false);
        InGameCanvas.SetActive(true);

        IsGameStarted = true;
    }

    public void AddScorePoint()
    {
        currentScore++;
        InGameCanvas.transform.Find("ScoreTextWhite").GetComponent<Text>().text = currentScore.ToString();
    }

    public void Pause()
    {
        if (IsPaused)
        {
            IsPaused = false;
            flappyBird.GetComponent<FlappyBirdController>().Pause(false);

        }
        else
        {
            IsPaused = true;
            flappyBird.GetComponent<FlappyBirdController>().Pause(true);
            Handheld.Vibrate();
        }
    }

    public void Die()
    {
        InGameCanvas.SetActive(false);
        DeadCanvas.SetActive(true);

        var auth = FirebaseAuth.DefaultInstance;
        SetScore(auth);

    }

    private void SetScore(FirebaseAuth auth)
    {
        FirebaseDatabase.DefaultInstance
        .GetReference("users/" + auth.CurrentUser.UserId + "/score").GetValueAsync()
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
                Debug.Log(task.Exception);
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string _score = "" + snapshot.Value;
                score = int.Parse(_score);
                if (currentScore > score)
                {
                    mDatabaseRef.Child("users").Child(auth.CurrentUser.UserId).Child("score").SetValueAsync(currentScore);
                    DeadCanvas.GetComponent<DeadCanvas_Manager>().SetScoreValues(currentScore, currentScore);
                    HomeManager.Instance.GetScore(auth);
                    currentScore = 0;
                }
                else
                    DeadCanvas.GetComponent<DeadCanvas_Manager>().SetScoreValues(currentScore, score);
                
            }
        });
    }

    public void Exit()
    {
        Application.Quit();
    }
}
