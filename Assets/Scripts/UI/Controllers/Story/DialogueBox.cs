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
        
        foreach (char letter in text)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _dialogueText.text = text;
                return OpResult.Aborted;
            }
            
            _dialogueText.text += letter;
            await UniTask.WaitForSeconds(textInterval, cancellationToken: cancellationToken).SuppressCancellationThrow();
        }
        
        return OpResult.Success;
    }

    /****** Private Members ******/

    [SerializeField] private TMPro.TextMeshProUGUI _nameText;
    [SerializeField] private TMPro.TextMeshProUGUI _dialogueText;
}