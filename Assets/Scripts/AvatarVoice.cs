using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarVoice : MonoBehaviour
{
    [SerializeField] TextToSpeechTechnology m_TTS;
    [SerializeField] ChatGPTManager m_ChatGPTManager;
    [SerializeField] AudioSource m_Audio;
    [SerializeField] SkinnedMeshRenderer m_AvatarSkinnedMeshRenderer;
    [SerializeField] int m_BlendShapeIndex;
    [SerializeField] float m_VoiceMovementSensitivity = 100;
    [SerializeField] float m_MaxVolume = 1; 

    private float loadness;
    private void Awake()
    {
        m_ChatGPTManager.ChatGPTResponded += OnChatGPTResponse;
    }

    private void OnDestroy()
    {
        m_ChatGPTManager.ChatGPTResponded -= OnChatGPTResponse;
    }

    private void Update()
    {
        loadness = GetAverageVolume() * m_VoiceMovementSensitivity;
        if(loadness > 0)
        {
            float movement = Mathf.Clamp(loadness / m_MaxVolume, 0, 1);
            SetMouthMovement(movement * 100);
        }
    }

    private void OnChatGPTResponse(string message)
    {
        m_TTS.Speak(message);
    }

    private void SetMouthMovement(float movement)
    {
        m_AvatarSkinnedMeshRenderer.SetBlendShapeWeight(m_BlendShapeIndex,movement);
    }

    private float GetAverageVolume()
    {
        float[] data = new float[256];
        float a = 0;
        m_Audio.GetOutputData(data, 0);
        foreach(float sample in data)
        {
            a += Mathf.Abs(sample);
        }
        return a / 256;
    }

    public void InterruptVoice()
    {
        m_Audio.Stop();
        SetMouthMovement(0);
    }
}
