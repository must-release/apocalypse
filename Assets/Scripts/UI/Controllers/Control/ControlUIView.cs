using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AD.UI
{
    public class ControlUIView : MonoBehaviour, IUIView<BaseUI>
    {
        /****** Public Members ******/

        public BaseUI UIType => BaseUI.Control;

        public void EnterUI()
        {
            gameObject.SetActive(true);
        }

        public void UpdateUI()
        {

        }

        public void ExitUI()
        {
            gameObject.SetActive(false);
        }

        public void Cancel()
        {
            // Change to Pause UI
            //UIController.Instance.ChangeState(UIController.STATE.PAUSE, false);
        }

        public async UniTask<OpResult> ShowSideDialogue(string text, float textInterval, CancellationToken cancellationToken)
        {
            _sideDialoguePanel.ShowPanel();

            OpResult result = await _sideDialoguePanel.DisplayText(text, textInterval, cancellationToken);

            _sideDialoguePanel.HidePanelAsync().Forget();

            return result;
        }

        public void UpdateHPBar(int currentHP, int maxHP)
        {
            _statusPanel.UpdateHPBar(currentHP, maxHP);
        }

        /****** Private Members ******/

        [SerializeField] private SideDialoguePanel _sideDialoguePanel;
        [SerializeField] private StatusPanel _statusPanel;

        private void OnValidate()
        {
            Debug.Assert(null != _sideDialoguePanel, "SideDialoguePanel is not assigned in ControlUIView.");
            Debug.Assert(null != _statusPanel, "Status panel is not assigned in ControlUIView");
        }

    }
}