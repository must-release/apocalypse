using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AD.UI
{
    public class SideDialoguePanel : MonoBehaviour
    {
        /****** Public Members ******/

        public void ShowPanel()
        {
            _hideCts?.Cancel();
            gameObject.SetActive(true);
        }

        public async UniTask HidePanelAsync()
        {
            Debug.Assert(gameObject.activeInHierarchy, "Trying to hide active side dialogue panel");

            _hideCts = new CancellationTokenSource();

            await UniTask.Delay(_HidePanelWaitTime, cancellationToken: _hideCts.Token).SuppressCancellationThrow();
            if (false == _hideCts.IsCancellationRequested)
            {
                gameObject.SetActive(false);
            }
        }

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
                await UniTask.Delay((int)(textInterval * 1000), cancellationToken: cancellationToken).SuppressCancellationThrow();
            }

            await UniTask.Delay(text.Length * _TextReadDelay, cancellationToken: cancellationToken).SuppressCancellationThrow();

            return cancellationToken.IsCancellationRequested ? OpResult.Aborted : OpResult.Success;
        }
        
        /****** Private Members ******/

        [SerializeField] private Image _characterImage;
        [SerializeField] private TextMeshProUGUI _dialogueText;

        private CancellationTokenSource _hideCts;
        private const int _HidePanelWaitTime = 1000;
        private const int _TextReadDelay = 50;

        private void OnValidate()
        {
            Debug.Assert(null != _characterImage, "CharacterImage is not assigned in SideDialoguePanel.");
            Debug.Assert(null != _dialogueText, "DialogueText is not assigned in SideDialoguePanel.");
        }
    }
}