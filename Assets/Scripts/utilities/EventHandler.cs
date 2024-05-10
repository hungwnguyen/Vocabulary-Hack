using System;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public event Action OnAnimationFinished;
    public void OnAnimationFinishedTrigger() => OnAnimationFinished?.Invoke();
}
