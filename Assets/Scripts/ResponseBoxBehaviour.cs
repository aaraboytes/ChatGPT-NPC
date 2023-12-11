using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseBoxBehaviour : MonoBehaviour
{
    private float speed = 0.5f;
    private RectTransform rect;
    [SerializeField] AvatarAnimationsController m_Avatar;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    private void Update()
    {
        if(!Input.GetMouseButton(0))
        {
            if (m_Avatar.State == AvatarAnimationsController.AvatarState.Talking)
            {
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, Mathf.Lerp(rect.anchoredPosition.y, rect.sizeDelta.y, 0.5f));
            }
        }
    }
}
