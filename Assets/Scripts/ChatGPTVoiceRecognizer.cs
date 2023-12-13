using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using OpenAI;
public class ChatGPTVoiceRecognizer : MonoBehaviour
{
    public UnityAction<string> AudioTranscripted;
    public bool IsRecording => isRecording;
    public string MicrophoneDevice { private get; set; }
    public OpenAIApi OpenAi { private get; set; }
    private AudioClip clip;
    private bool isRecording = false;
    private int microphoneDuration = 5;
    private string microphone;
    private string recordFilename = "output.wav";

    public void SetMicrophone(string deviceName)
    {
        microphone = deviceName;
    }

    public void StartRecording()
    {
        isRecording = true;
#if !UNITY_WEBGL
        clip = Microphone.Start(microphone, false, microphoneDuration, 44100);
#endif
    }

    public async void EndRecording()
    {
#if !UNITY_WEBGL
        Microphone.End(null);
#endif
        byte[] data = SaveWav.Save(recordFilename, clip);
        var req = new CreateAudioTranscriptionsRequest
        {
            FileData = new FileData() { Data = data, Name = "audio.wav" },
            Model = "whisper-1",
            Language = "es"
        };
        var res = await OpenAi.CreateAudioTranscription(req);
        AudioTranscripted?.Invoke(res.Text);
        isRecording = false;
    }
}
