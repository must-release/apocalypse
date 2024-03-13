using System;
public interface IUIState
{
    public static bool isConsole = true;

    public void StartUI();
    public void UpdateUI();
    public void EndUI();
    public void Move(float move);
    public void Stop();
    public void Attack();
    public void Submit();
    public void Cancel();
}

