using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public void SetName(string name)
    {

    }

    public async UniTask DisplayText(string text, float textSpeed)
    {
        await UniTask.Delay(1000); // Simulate delay based on speed type
    }
}