using System;
public interface IUIState
{
    public static bool isConsole = false;

    public void StartUI();
    public void UpdateUI();
    public void EndUI();
    public UIManager.STATE GetState();
    public void Move(float move);
    public void Stop();
    public void Attack();
    public void Submit();
    public void Cancel();
}

