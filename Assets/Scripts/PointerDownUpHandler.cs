using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class PointerDownUpHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public UnityEvent onPointerDown;
    public UnityEvent onPointerUp;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        // ignore if button not interactable
        if (!_button.interactable) return;

        // just to be sure kill all current routines
        // (although there should be none)
        onPointerDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        onPointerUp?.Invoke();
    }

    // Afaik needed so Pointer exit works .. doing nothing further
    public void OnPointerEnter(PointerEventData eventData) { }
}