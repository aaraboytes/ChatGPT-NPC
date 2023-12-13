using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Gigadrillgames.AUP.Common;
using Gigadrillgames.AUP.SpeechTTS;
using System;

public class TextToSpeechAndroid : TextToSpeechTechnology
{
    private SpeechPlugin speechPlugin;
    private TextToSpeechPlugin textToSpeechPlugin;
    private Dispatcher dispatcher;
    private UtilsPlugin utilsPlugin;
    private int synthesizeCounter;
    private string ttsEngine = "com.google.android.tts";
    private string fileName = "dialogue.mp3";
    private string[] _synthesizeFileNames;
    private string _synthesizeFileName;
    private StringBuilder _synthesizeFilenames;
    private void Awake()
    {
        dispatcher = Dispatcher.GetInstance();
        utilsPlugin = UtilsPlugin.GetInstance();
        utilsPlugin.Init();
        utilsPlugin.SetDebug(0);
        speechPlugin = SpeechPlugin.GetInstance();
        speechPlugin.SetDebug(0);
        textToSpeechPlugin = TextToSpeechPlugin.GetInstance();
        textToSpeechPlugin.Init();
        textToSpeechPlugin.SetDebug(0);
        _synthesizeFilenames = new StringBuilder();
    }
    private void Start()
    {
        Invoke("DelayStartTTSEngine", 5f);
    }
    private void OnDestroy()
    {
        RemoveEventListener();
        if (textToSpeechPlugin != null)
        {
            textToSpeechPlugin.ShutDownTextToSpeechService();
        }
    }
    private void OnApplicationPause(bool val)
    {
        //for text to speech events
        if (textToSpeechPlugin != null)
        {
            if (textToSpeechPlugin.isInitialized())
            {
                if (val)
                {
                    textToSpeechPlugin.UnRegisterBroadcastEvent();
                }
                else
                {
                    textToSpeechPlugin.RegisterBroadcastEvent();
                }
            }
        }
    }
    private void DelayStartTTSEngine()
    {
        textToSpeechPlugin.StartTTSEngine(ttsEngine);
        AddEventListener();
    }

    #region Listeners
    private void AddEventListener()
    {
        if (textToSpeechPlugin)
        {
            textToSpeechPlugin.OnInitialize += OnInit;
            textToSpeechPlugin.OnErrorSpeech += OnErrorSpeech;
            textToSpeechPlugin.OnStartSpeech += OnStartSpeech;
            textToSpeechPlugin.OnEndSpeech += OnEndSpeech;
            textToSpeechPlugin.OnErrorTTSSynthesize += OnErrorTTSSynthesize;
        }
    }

    private void RemoveEventListener()
    {
        if (textToSpeechPlugin)
        {
            textToSpeechPlugin.OnInitialize -= OnInit;
            textToSpeechPlugin.OnErrorSpeech -= OnErrorSpeech;
            textToSpeechPlugin.OnStartSpeech -= OnStartSpeech;
            textToSpeechPlugin.OnEndSpeech -= OnEndSpeech;
            textToSpeechPlugin.OnErrorTTSSynthesize -= OnErrorTTSSynthesize;
        }
    }
    private void OnEndSpeech(string utteranceId)
    {
        dispatcher.InvokeAction(
            () =>
            {
                Debug.Log("Speech ended " + utteranceId);
            });
    }

    private void OnStartSpeech(string utteranceId)
    {
        dispatcher.InvokeAction(
            () =>
            {
                Debug.Log("Speech started " + utteranceId);
            });
    }

    private void OnErrorTTSSynthesize(string errorMessage, string utteranceId)
    {
        dispatcher.InvokeAction(
                () =>
                {
                    Debug.LogError("OnErrorTTSSynthesize errorMessage: " + errorMessage);
                    Debug.LogError("OnErrorTTSSynthesize utteranceId: " + utteranceId);
                }
            );
    }

    private void OnErrorSpeech(string status)
    {
        dispatcher.InvokeAction(
            () =>
            {
                Debug.LogError("Error on speech " + status);
            });
    }

    private void OnInit(int status)
    {
        dispatcher.InvokeAction(
            () =>
            {
                Debug.Log("On init status:" + status);
                if(status == 1)
                {
                    //Set language pronunciation
                    TTSLocaleCountry ttsLocaleCountry = TTSLocaleCountry.MEXICO;
                    string countryISO2Alpha = textToSpeechPlugin.GetCountryISO2Alpha(ttsLocaleCountry);
                    textToSpeechPlugin.SetLocaleByCountry(countryISO2Alpha);
                    textToSpeechPlugin.SetPitch(1f);
                    textToSpeechPlugin.SetSpeechRate(1f);
                }
                else
                {
                    Debug.LogError("Init speech service failed!");
                }

            });
    }
    #endregion

    public override void MessageToSpeech(string message)
    {
        if(message!= string.Empty)
        {
            if (textToSpeechPlugin.isInitialized())
            {
                string utteranceId = "tts_synthesize_" + synthesizeCounter;
                synthesizeCounter++;
                Debug.Log("SynthesizeToFile utteranceId " + utteranceId);
                utilsPlugin.UnMuteBeep();
                textToSpeechPlugin.SynthesizeToFile(message, utteranceId, fileName, Application.persistentDataPath);
            }
        }
    }

    public void PlayLastSpeech()
    {
        textToSpeechPlugin.GetSynthesizeFiles(Application.persistentDataPath);
        if (_synthesizeFileNames != null)
        {
            if (_synthesizeFileNames.Length > 0)
            {
                _synthesizeFileName = _synthesizeFileNames[_synthesizeFileNames.Length - 1];
                Debug.Log("Loaded file " + _synthesizeFileName);
                textToSpeechPlugin.LoadSynthesizeFile(_synthesizeFileName);
                textToSpeechPlugin.PlaySynthesizeFile();
            }
            else
            {
                Debug.LogError("Dialogue audio files not found"); 
            }
        }
        else
        {
            Debug.LogError("Dialogue audio files not found");
        }
    }

    public bool IsSpeaking()
    {
        return textToSpeechPlugin.IsSpeaking();
    }
}
