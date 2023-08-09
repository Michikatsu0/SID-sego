using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpTest : MonoBehaviour
{
    public int userId = 2;
    public string url = "https://my-json-server.typicode.com/Michikatsu0/SID-Sego";
    public string apiRickAndMorty = "https://rickandmortyapi.com/api/character";
    [SerializeField] private TMP_Text usernameLabel;
    [SerializeField] private RawImage[] myDeck;
    [SerializeField] private TMP_Text[] myDeckLabel;
    private User myUser;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SendRequest()
    {
        StartCoroutine(GetUsers());
    }

    IEnumerator GetUsers()
    {
        UnityWebRequest request = UnityWebRequest.Get(url + "/users/" + userId);
        yield return request.SendWebRequest();
        if (request.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);

            if (request.responseCode == 200)
            {
                myUser = JsonUtility.FromJson<User>(request.downloadHandler.text);

                usernameLabel.text = myUser.username;

                for (int i = 0; i < myUser.deck.Length; i++)
                {
                    StartCoroutine(GetCharacter(i));
                }
            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }

    IEnumerator GetCharacter(int index)
    {
        int characterID = myUser.deck[index];
        UnityWebRequest request = UnityWebRequest.Get(apiRickAndMorty + "/" + characterID);
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            if (request.responseCode == 200)
            {
                Character character = JsonUtility.FromJson<Character>(request.downloadHandler.text);
                StartCoroutine(DownloadImage(character.image, myDeck[index]));
                myDeckLabel[index].text = character.name;
            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }

    IEnumerator DownloadImage(string mediaUrl, RawImage image)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log(request.error);
        }
        else if (!request.isHttpError)
        {
            image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
}

[System.Serializable]
public class UsersList
{
    public List<User> users;
}

[System.Serializable]
public class User
{
    public int id;
    public string username;
    public int[] deck;
}

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string image;
}