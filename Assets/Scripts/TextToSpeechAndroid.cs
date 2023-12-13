using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crosstales.RTVoice;
using Crosstales.RTVoice.Demo;
using System;

public class TextToSpeechAndroid : TextToSpeechTechnology
{
    [SerializeField] AudioSource m_Audio;
    private Crosstales.RTVoice.Model.Enum.Gender gender = Crosstales.RTVoice.Model.Enum.Gender.MALE;
    private string culture = "es";
    private Crosstales.RTVoice.Model.Voice voice;
    private string uid = string.Empty;
    
    private void Start()
    {
        Speaker.Instance.OnVoicesReady += OnVoicesReady;
    }

    private void OnVoicesReady()
    {
        InitVoice();
    }

    private void InitVoice()
    {
        var item = Speaker.Instance.VoiceForGender(gender,culture);
        this.voice = item;
        if(item != null)
        {
            Debug.Log("Voice is ready");
        }
        else
        {
            Debug.Log("Male voice not found");
        }
    }

    public override void Speak(string message)
    {
        if(message!= string.Empty)
        {
            if(!string.IsNullOrEmpty(uid))
                Speaker.Instance.Silence(uid);
            uid = Speaker.Instance.Speak(message, m_Audio, voice, true, GUISpeech.Rate, GUISpeech.Pitch, GUISpeech.Volume);
        }
    }
}
