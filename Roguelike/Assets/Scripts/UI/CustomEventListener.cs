using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomEventListener
{
    public string EventName { get; private set; }
    public Action<MouseDownEvent> MouseDownAction { get; private set; }
    public Action<MouseUpEvent> MouseUpAction { get; private set; }

    public CustomEventListener(string eventName, Action<MouseDownEvent> mouseDownAction, Action<MouseUpEvent> mouseUpAction)
    {
        EventName = eventName;
        MouseDownAction = mouseDownAction;
        MouseUpAction = mouseUpAction;
    }

    public void RegisterCallbacks(Button button)
    {
        button.RegisterCallback<MouseDownEvent>(evt =>
        {
            MouseDownAction.Invoke(evt);
            Debug.Log($"{EventName} MouseDownEvent");
        });

        button.RegisterCallback<MouseUpEvent>(evt =>
        {
            MouseUpAction.Invoke(evt);
            Debug.Log($"{EventName} MouseUpEvent");
        });
    }
}