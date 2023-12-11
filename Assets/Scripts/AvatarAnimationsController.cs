using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarAnimationsController : MonoBehaviour
{
    [SerializeField] Animator m_Animator;
    public enum AvatarState
    {
        Idle,Thinking,Talking
    }

    public AvatarState State { get; internal set; }
    public void SetState(AvatarState newState)
    {
        State = newState;
        switch (newState)
        {
            case AvatarState.Idle:
                m_Animator.SetBool("Thinking", false);
                m_Animator.SetBool("Talking", false);
                break;
            case AvatarState.Thinking:
                m_Animator.SetBool("Thinking", true);
                break;
            case AvatarState.Talking:
                m_Animator.SetBool("Talking", true);
                break;
            default:
                break;
        }
    }
}
