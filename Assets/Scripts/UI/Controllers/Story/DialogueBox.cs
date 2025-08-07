using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public void SetName(string name)
    {

    }

    public async UniTask DisplayText(string text, StoryDialogue.TextSpeedType speedType)
    {
        await UniTask.Delay(1000); // Simulate delay based on speed type
    }
}