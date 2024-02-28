using System;
public interface IUIState
{
    public static bool isConsole = true;

    public void StartUI();
    public void EndUI();
    public void Move(float move);
}

