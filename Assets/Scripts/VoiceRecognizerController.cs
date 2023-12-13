using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class VoiceRecognizerController : MonoBehaviour
{
    [SerializeField] private ChatGPTVoiceRecognizer m_ChatGPTVoiceRecognizer;
    [SerializeField] private PointerDownUpHandler m_RecordButton;
    [SerializeField] private TMP_InputField m_MessageField;
    [SerializeField] private ChatUI m_ChatUI;
    [SerializeField] private AvatarVoice m_AvatarVoice;
    [SerializeField] private TMP_Dropdown m_DevicesDropdown;

    private int microphoneIndex = 0;
    
    private void Start()
    {
        AddListeners();
        FillMicrophoneDevices();
        SetMicrophone();
    }

    private void AddListeners()
    {
        m_RecordButton.onPointerDown.AddListener(OnRecordButtonPressed);
        m_RecordButton.onPointerUp.AddListener(OnRecordButtonReleased);
        m_ChatGPTVoiceRecognizer.AudioTranscripted += OnAudioTranscripted;
    }

    private void FillMicrophoneDevices()
    {
        foreach(var device in Microphone.devices)
        {
            m_DevicesDropdown.options.Add(new TMP_Dropdown.OptionData(device));
        }
        m_DevicesDropdown.onValueChanged.AddListener(OnDeviceChanged);
    }

  
    private void SetMicrophone()
    {
        int microphoneIndex = PlayerPrefs.GetInt("user-mic-device-index", 1);
        PlayerPrefs.SetInt("user-mic-device-index", microphoneIndex);
        Debug.Log("Selected " + Microphone.devices[microphoneIndex] + " microphone device");
        m_DevicesDropdown.SetValueWithoutNotify(microphoneIndex);
        m_ChatGPTVoiceRecognizer.SetMicrophone(Microphone.devices[microphoneIndex]);
    }

    private void OnAudioTranscripted(string transcription)
    {
        m_MessageField.text = transcription;
        Debug.Log("Audio transcripted");
        m_ChatUI.AskChatGPT();
    }

    private void OnDeviceChanged(int index)
    {
        microphoneIndex = index;
        PlayerPrefs.SetInt("user-mic-device-index", microphoneIndex);
        m_ChatGPTVoiceRecognizer.SetMicrophone(Microphone.devices[microphoneIndex]);
    }

    private void OnRecordButtonReleased()
    {
        m_ChatGPTVoiceRecognizer.EndRecording();
        Debug.Log("Record button released");
    }

    private void OnRecordButtonPressed()
    {
        m_ChatUI.Interrupt();
        m_AvatarVoice.InterruptVoice();
        m_ChatGPTVoiceRecognizer.StartRecording();
    }
}
