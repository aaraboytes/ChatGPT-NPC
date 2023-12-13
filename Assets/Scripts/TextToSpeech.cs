using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
public class TextToSpeech : TextToSpeechTechnology
{
    [SerializeField] TextAsset m_AmazonCredentials;
    [SerializeField] AudioSource m_Audio;
    AmazonCredentialsAuth amazonCredentials;
    private AmazonPollyClient client;
    private BasicAWSCredentials credentials;
    private void Awake()
    {
        amazonCredentials = JsonUtility.FromJson<AmazonCredentialsAuth>(m_AmazonCredentials.ToString());
    }
    async void Start()
    {
        InitAmazonPolly();
        
    }
    private void InitAmazonPolly()
    {
        credentials = new BasicAWSCredentials(amazonCredentials.accessKey, amazonCredentials.secretKey);
        client = new AmazonPollyClient(credentials, Amazon.RegionEndpoint.EUCentral1);
    }

    private void WriteIntoFile(Stream stream)
    {
        using(var fileStream = new FileStream(Application.persistentDataPath+"/audio.mp3", FileMode.Create))
        {
            byte[] buffer = new byte[8 + 4092];
            int bytesRead;
            while((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                fileStream.Write(buffer, 0, bytesRead);
            }
        }
    }

    private async void PlaySpeech()
    {
        using (var www = UnityWebRequestMultimedia.GetAudioClip($"{ Application.persistentDataPath}/audio.mp3", AudioType.OGGVORBIS))
        {
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
                var clip = DownloadHandlerAudioClip.GetContent(www);
                m_Audio.clip = clip;
                m_Audio.Play();
            }
        }
    }

    public override async void Speak(string message)
    {
        var request = new SynthesizeSpeechRequest()
        {
            Text = message,
            Engine = Engine.Neural,
            VoiceId = VoiceId.Sergio,
            OutputFormat = OutputFormat.Ogg_vorbis
        };

        var response = await client.SynthesizeSpeechAsync(request);
        WriteIntoFile(response.AudioStream);
        PlaySpeech();
    }
}

[System.Serializable]
public class AmazonCredentialsAuth
{
    public string accessKey;
    public string secretKey;
}
