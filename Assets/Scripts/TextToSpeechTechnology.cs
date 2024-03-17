using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class TextToSpeechTechnology : MonoBehaviour
{
    public bool isSpeaking { get; set; }
    public UnityAction SpeakCompleted;
    public abstract void Speak(string message);
    protected abstract void OnSpeakCompleted(string arg0);
}