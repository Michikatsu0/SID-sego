using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpTest : MonoBehaviour
{
    public TMP_InputField TMP_UserId;
    public string url = "https://my-json-server.typicode.com/Michikatsu0/SID-Sego";
    public string apiRickAndMorty = "https://rickandmortyapi.com/api/character";
    
    [SerializeField] private TMP_Text usernameLabel;
    [SerializeField] private RawImage[] myDeck;
    [SerializeField] private TMP_Text[] myDeckLabel;
    [SerializeField] private TMP_Text[] myDeckLabel2;
    [SerializeField] private TMP_Text[] myDeckLabel3;
    
    private UserRYM myUser;
    private int userId = 0;

    public void SendRequest()
    {
        if (int.TryParse(TMP_UserId.text,out int result))
            userId = result;
        
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
                myUser = JsonUtility.FromJson<UserRYM>(request.downloadHandler.text);

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
                myDeckLabel2[index].text = character.species;
                myDeckLabel3[index].text = character.gender;
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
    public List<UserRYM> users;
}

[System.Serializable]
public class UserRYM
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
    public string species;
    public string gender;
}