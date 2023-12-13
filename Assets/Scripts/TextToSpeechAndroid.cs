using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crosstales.RTVoice;
using Crosstales.RTVoice.Demo;
public class TextToSpeechAndroid : TextToSpeechTechnology
{
    [SerializeField] AudioSource m_Audio;
    private Crosstales.RTVoice.Model.Enum.Gender gender;
    private string culture = "es";
    private Crosstales.RTVoice.Model.Voice voice;
    private string uid = string.Empty;
    
    private void Start()
    {
        InitVoice();
    }

    private void InitVoice()
    {
        gender = Crosstales.RTVoice.Model.Enum.Gender.MALE;
        var item = Speaker.Instance.VoiceForGender(gender, culture,0,"es");
        if (item == null)
        {
            Debug.Log("Any male voices found");
            gender = Crosstales.RTVoice.Model.Enum.Gender.FEMALE;
            item = Speaker.Instance.VoiceForGender(gender, culture);
            if (item == null)
            {
                Debug.Log("There aren´t any voices for " + culture);
                return;
            }
        }
        Debug.Log("Voice is ready");
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
