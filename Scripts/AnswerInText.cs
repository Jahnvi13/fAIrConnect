using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

public class AnswerInText : MonoBehaviour
{
    // Start is called before the first frame update
    private string API_KEY = "fancy-shoes"; // Replace with your API key
    private string URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.0-pro:generateContent?key=";
    private string TS_URL = "https://texttospeech.googleapis.com/v1/text:synthesize";
    private string input = "";
    private string audioFilePath = "Assets/my_voice.mp3";
    private string whattospeak = "";
    public Text OutputText;
    private string chance = "go";

    void Start()
    {
        
        if (input != "" && chance=="go")
        {
            StartCoroutine(PostData(input));
        }
        if (whattospeak != "" && chance == "come")
        {
            StartCoroutine(GetAudio());
            StartCoroutine(WaitAndDoSomething());
        }
        else
        {
            Debug.Log("emptyyyyy");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator WaitAndDoSomething()
    {
        yield return new WaitForSeconds(5f);
        ImportAudioFile(audioFilePath);
        Debug.Log("Waited for 5 seconds!");
    }
    IEnumerator PostData(string input)
    {
        string jsonPayload = @"
        {
            ""contents"": [
                {
                    ""parts"": [
                        {
                            ""text"": ""input: What is the job title of the open role?""
                        },
                        {
                            ""text"": ""output: The job title is Product Manager, Data Analytics and Business Intelligence at Google which requires 8 years of experience.""
                        },
                        {
                            ""text"": ""input: What is an open role for undergraduates?""
                        },
                        {
                            ""text"": ""output: The job title is Software Engineer, Berlin at Google for university graduates.""
                        },
                        {
                            ""text"": ""input:";
        jsonPayload += input;
        jsonPayload += @"
            "" 
                        }
                    ]
                }
            ]
        }";
        Debug.Log(jsonPayload);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(URL + API_KEY, ""))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("From here" + www.error);
            }
            else
            {
                string json = "[" + www.downloadHandler.text + "]";
                SimpleJSON.JSONNode stats = SimpleJSON.JSON.Parse(json);
                string texts = stats[0]["candidates"][0]["content"]["parts"][0]["text"];
                OutputText.text = texts.Substring(8);
                whattospeak = texts.Substring(8);
                chance = "come";
            }
        }
    }
    IEnumerator GetAudio()
    {

        string jsonPayload = @"
        {
              ""input"": {
                ""text"": """;

        jsonPayload += whattospeak;
        jsonPayload += @"""
              },
              ""voice"": {
                ""languageCode"": ""en-gb"",
                ""name"": ""en-GB-Standard-A"",
                ""ssmlGender"": ""FEMALE""
              },
              ""audioConfig"": {
                ""audioEncoding"": ""MP3""
              }
         }";


        Debug.Log(jsonPayload);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(TS_URL, ""))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Authorization", "Bearer bearer-token-for-auth");
            www.SetRequestHeader("x-goog-user-project", "secret-voice-417315");
            www.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
            Debug.Log(www);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("From here" + www.error);
            }
            else
            {
                string json = "[" + www.downloadHandler.text + "]";
                SimpleJSON.JSONNode stats = SimpleJSON.JSON.Parse(json);
                string goodByteString = stats[0]["audioContent"];
                Debug.Log(goodByteString);

                byte[] bytes = Convert.FromBase64String(goodByteString);
                File.WriteAllBytes("Assets/my_voice.mp3", bytes);
               
            }
        }
    }
    void ImportAudioFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("Audio file does not exist: " + filePath);
            return;
        }
        AssetDatabase.ImportAsset(filePath, ImportAssetOptions.Default);
        Debug.Log("Audio file imported successfully: " + filePath);

        AudioClip audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(filePath);

        if (audioClip != null)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.clip = audioClip;

            audioSource.Play();

        }
        else
        {
            Debug.LogError("Failed to load audio clip from path: " + audioFilePath);
        }
        chance = "go";
    }
    public void readStringInput(string s)
    {
        input = s;
        Debug.Log("Input read" + input);
        Start();
    }
}
