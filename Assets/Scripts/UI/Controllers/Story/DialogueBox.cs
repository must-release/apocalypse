using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    /****** Public Memvers ******/

    public void SetName(string name) { _nameText.text = name; }

    public async UniTask DisplayText(string text, float textInterval, CancellationToken cancellationToken = default)
    {
        _dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            _dialogueText.text += letter;
            await UniTask.WaitForSeconds(textInterval, cancellationToken: cancellationToken);
        }
    }

    public void DisplayText(string text)
    {
        _dialogueText.text = text;
    }

    /****** Private Members ******/

    [SerializeField] private TMPro.TextMeshProUGUI _nameText;
    [SerializeField] private TMPro.TextMeshProUGUI _dialogueText;
}