using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    /****** Public Memvers ******/

    public void SetName(string name) { _nameText.text = name; }

    public async UniTask<OpResult> DisplayText(string text, float textInterval, CancellationToken cancellationToken = default)
    {
        _dialogueText.text = "";
        
        foreach (char letter in text.ToCharArray())
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _dialogueText.text = text;
                return OpResult.Aborted;
            }
            
            _dialogueText.text += letter;
            await UniTask.Delay((int)(textInterval * 1000), cancellationToken: cancellationToken);
        }
        
        return OpResult.Success;
    }

    public void DisplayText(string text)
    {
        _dialogueText.text = text;
    }

    /****** Private Members ******/

    [SerializeField] private TMPro.TextMeshProUGUI _nameText;
    [SerializeField] private TMPro.TextMeshProUGUI _dialogueText;
}