using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TextToSpeechTechnology : MonoBehaviour
{
    public abstract void Speak(string message);
}