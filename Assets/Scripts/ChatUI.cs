using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChatUI : MonoBehaviour
{
    [SerializeField] float m_ResponseSpeed;
    [SerializeField] TextMeshProUGUI m_Response;
    [SerializeField] TMP_InputField m_UserMessage;
    [SerializeField] Button m_SendButton;
    [SerializeField] ChatGPTManager m_ChatGPTManager;
    [SerializeField] AvatarAnimationsController m_AvatarAnimationsController;
    private bool isReadingMessage = false;
    private bool endMessageForced = false;
    private void Start()
    {
        m_ChatGPTManager.ChatGPTResponded += OnChatGPTResponded;
        m_SendButton.onClick.AddListener(AskChatGPT);
    }

    private void OnDestroy()
    {
        m_ChatGPTManager.ChatGPTResponded -= OnChatGPTResponded;
    }

    private void OnChatGPTResponded(string response)
    {
        StartCoroutine(ReadMessage(response));
        m_SendButton.interactable = true;
        m_AvatarAnimationsController.SetState(AvatarAnimationsController.AvatarState.Talking);
    }

    private IEnumerator ReadMessage(string message)
    {
        isReadingMessage = true;
        endMessageForced = false;
        int messageLenght = message.Length;
        int currentMessageLenght = 0;
        m_Response.text = "";
        while(currentMessageLenght < messageLenght || endMessageForced)
        {
            m_Response.text += message[currentMessageLenght];
            currentMessageLenght++;
            yield return new WaitForSeconds(m_ResponseSpeed);
        }
        if (endMessageForced)
        {
            m_Response.text = message;
            endMessageForced = false;
        }
        isReadingMessage = false;
        m_AvatarAnimationsController.SetState(AvatarAnimationsController.AvatarState.Idle);
    }

    public void AskChatGPT()
    {
        string message = m_UserMessage.text;
        m_SendButton.interactable = false;
        m_AvatarAnimationsController.SetState(AvatarAnimationsController.AvatarState.Thinking);
        m_UserMessage.text = string.Empty;
        m_ChatGPTManager.AskChatGPT(message);
    }
}
