using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TikTokLiveUnity;
using TikTokLiveSharp.Client;
using TikTokLiveSharp.Events;
using TMPro;
using System;

public class TikTokLiveCommentsManager : MonoBehaviour
{
    [SerializeField] TextToSpeechTechnology m_TTS;
    [SerializeField] ChatGPTManager m_ChatGptManager;
    [SerializeField] Transform m_MessagesContainer;
    [SerializeField] GameObject m_MessagePrefab;

    [SerializeField] private bool isAnsweringComment = false;
    private float minTimeBtwAnswers = 2f;
    private float timer = 0;

    private Queue<string[]> Comments = new Queue<string[]>();
    private void Start()
    {
        TikTokLiveManager.Instance.OnChatMessage += ReceiveChatMessage;
        m_TTS.SpeakCompleted += OnSpeakCompleted;
    }

    private void Update()
    {
        if(Comments.Count > 0 && timer>= minTimeBtwAnswers && !m_TTS.isSpeaking && !isAnsweringComment)
        {
            isAnsweringComment = true;
            ReadComment();
            timer = 0;
        }
        timer += Time.deltaTime;
    }

    private void OnDestroy()
    {
        TikTokLiveManager.Instance.OnChatMessage -= ReceiveChatMessage;
        m_TTS.SpeakCompleted -= OnSpeakCompleted;
    }

    private void ReceiveChatMessage(TikTokLiveClient sender, Chat e)
    {
        string nickname = e.Sender.NickName;
        string message = e.Message;
        string[] newCommment = new string[2] { nickname, message };
        Comments.Enqueue(newCommment);
        if (Comments.Count >= 10)
            Comments.Dequeue();

        var newMessage = Instantiate(m_MessagePrefab, m_MessagesContainer);
        newMessage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{nickname}:{message}";
        newMessage.transform.SetAsFirstSibling();
    }

    private void OnSpeakCompleted()
    {
        isAnsweringComment = false;
    }

    private void ReadComment()
    {
        if (Comments.Count > 0)
        {
            string[] comment = Comments.Dequeue();
            m_ChatGptManager.AskChatGPT($"{comment[0]} dice: {comment[1]}");
        }
    }
}
