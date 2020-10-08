using System;
using UnityEngine;
using GenericAnimationCore;

public abstract class BasePopUp : MonoBehaviour
{
    private Action _onOpenDone;
    private Action _onCloseDone;

    public bool IsOpen { get; private set; }
    public abstract Type GetConfigType();
    public CanvasGroup canvasGroup;

    public void RegisterOnOpenDone(Action action) => _onOpenDone += action;
    public void UnregisterOnOpenDone(Action action) => _onOpenDone -= action;

    public void RegisterOnCloseDone(Action action) => _onCloseDone += action;
    public void UnregisterOnCloseDone(Action action) => _onCloseDone -= action;

    [Header("Animations")] public GenericAnimationModule openAnimation;
    public GenericAnimationModule closeAnimation;

    private void OnValidate()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    protected virtual void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        canvasGroup.interactable = false;
        if (IsOpen)
        {
            Debug.LogWarning($"{this} already open");
            return;
        }

        IsOpen = true;
        if (openAnimation.HasAnimations)
            openAnimation.Play(this, () => _Open(OpenDone));
        else
            _Open(OpenDone);
    }

    public void Close()
    {
        if (!IsOpen)
        {
            Debug.LogWarning($"{this} already close");
            return;
        }

        canvasGroup.interactable = false;
        IsOpen = false;

        if (closeAnimation.HasAnimations)
            closeAnimation.Play(this, () => _Close(CloseDone));
        else
            _Close(CloseDone);
    }

    private void OpenDone()
    {
        canvasGroup.interactable = true;
        _onOpenDone?.Invoke();
    }

    private void CloseDone()
    {
        _onCloseDone?.Invoke();
    }

    protected virtual void _Open(Action onDone)
    {
        gameObject.SetActive(true);
        onDone?.Invoke();
    }

    protected virtual void _Close(Action onDone)
    {
        gameObject.SetActive(false);
        onDone?.Invoke();
    }
}