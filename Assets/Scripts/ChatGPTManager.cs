using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Events;

public class ChatGPTManager : MonoBehaviour
{
    public UnityAction<string> ChatGPTResponded;
    
    [SerializeField] private TextAsset m_OpenAIAuthFile;
    [SerializeField] private ChatGPTVoiceRecognizer m_VoiceRecognizer;

    private OpenAIApi openAI;
    private List<ChatMessage> messages = new List<ChatMessage>();
    private OpenAIAuth authData;
    private readonly string initialPrompt = "Eres un profesor de Ingenier�a en una universidad. Vas a interactuar con alumnos de los primeros semestres de la carrera. Responde a las preguntas manteniendo tu perfil de profesor, con respuestas claras, cortas y que est�n orientadas a ser entendidas por alumnos. Da respuestas acad�micas que hagan referencias a f�rmulas y temas ense�ados en la universidad. Responde lo m�s r�pido posible.";

    private void Awake()
    {
        authData = JsonUtility.FromJson<OpenAIAuth>(m_OpenAIAuthFile.ToString());
    }
    private void Start()
    {
        InitOpenAI();
    }
    private async void InitOpenAI()
    {
        openAI = new OpenAIApi(authData.api_key,authData.organization);
        m_VoiceRecognizer.OpenAi = openAI;
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = initialPrompt;
        newMessage.Role = "user";

        messages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";

        var response = await openAI.CreateChatCompletion(request);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);

            Debug.Log(chatResponse.Content);
            ChatGPTResponded?.Invoke("Hola, soy tu profesor virtual de Ingenier�a, soy una Inteligencia Artificial dispuesta a proveerte de informaci�n y conocimientos en todas las ramas de Ingenier�a. Adelante, puedes preguntarme cualquier cosa.");
        }
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
