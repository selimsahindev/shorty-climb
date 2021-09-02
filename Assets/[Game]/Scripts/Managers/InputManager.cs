using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public bool touch = false;
    [HideInInspector] public Vector2 input = Vector2.zero;
    [HideInInspector] public Vector2 delta = Vector2.zero;

    private Vector2 mouseStartPosition;
    private Vector2 mouseLastPosition;
    private Vector2 _input;
    private PointerEventData eventData;

    #region Singleton
    public static InputManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }
    #endregion

    private void Update()
    {
        if (eventData == null) return;

        input = eventData.position - mouseStartPosition;
        delta = eventData.position - mouseLastPosition;

        mouseLastPosition = eventData.position;
    }

    public void OnPointerDown(PointerEventData _eventData)
    {
        touch = true;
        eventData = _eventData;
        mouseStartPosition = eventData.position;
        mouseLastPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData _eventData)
    {
        eventData = null;
        touch = false;
        delta = Vector2.zero;
        mouseStartPosition = Vector2.zero;
        mouseLastPosition = Vector2.zero;
        _input = Vector2.zero;
    }
}
