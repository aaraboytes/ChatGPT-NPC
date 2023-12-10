using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ChatUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_Response;
    [SerializeField] ChatGPTManager m_ChatGPTManager;
    private void Start()
    {
        m_ChatGPTManager.ChatGPTResponded += OnChatGPTResponded;
    }

    private void OnDestroy()
    {
        m_ChatGPTManager.ChatGPTResponded -= OnChatGPTResponded;
    }

    private void OnChatGPTResponded(string response)
    {
        m_Response.text = response;
    }
}
