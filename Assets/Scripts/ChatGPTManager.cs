using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Events;

public class ChatGPTManager : MonoBehaviour
{
    public UnityAction<string> ChatGPTResponded;
    [SerializeField] private TextAsset m_OpenAIAuthFile;
    private OpenAIApi openAI;
    private List<ChatMessage> messages = new List<ChatMessage>();
    private OpenAIAuth authData;

    private void Awake()
    {
        authData = JsonUtility.FromJson<OpenAIAuth>(m_OpenAIAuthFile.ToString());
    }
    private void Start()
    {
        InitOpenAI();
    }
    private void InitOpenAI()
    {
        openAI = new OpenAIApi(authData.api_key,authData.organization);
        AskChatGPT("Tu nombre es el profesor Andrés, un profesor en Ingeniería. A partir de ahora deberás responder a mis siguientes preguntas en base a tu conocimiento en Ingeniería siempre manteniendo un perfil de profesor.");
    }
    public async void AskChatGPT(string message)
    {
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = message;
        newMessage.Role = "user";

        messages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";

        var response = await openAI.CreateChatCompletion(request);

        if(response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);

            Debug.Log(chatResponse.Content);

            ChatGPTResponded?.Invoke(chatResponse.Content);
        }
    }
}

[System.Serializable]
public class OpenAIAuth
{
    public string api_key;
    public string organization;
}
