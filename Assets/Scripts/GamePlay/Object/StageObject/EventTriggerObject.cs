using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class EventTriggerObject : MonoBehaviour
{
    /****** Public Members ******/



    /****** Private Members ******/

    [SerializeField] private GameEventInfo _triggerEvent;
    [SerializeField] private TextMeshPro _debuggingName;
    [SerializeField] private bool _isReusable;

    private IGameEvent _gameEvent;

    private void Awake()
    {
        Debug.Assert(null != _triggerEvent, "EventTriggerObject must have a valid EventInfo assigned.");
        Debug.Assert(null != _debuggingName, "EventTriggerObject must have a valid TextMeshPro assigned for debugging.");

        GetComponent<Collider2D>().isTrigger = true;
        GetComponent<SpriteRenderer>().enabled = false;
        _debuggingName.gameObject.SetActive(false);
    }

    private void Start()
    {
        _gameEvent = GameEventFactory.CreateFromInfo(_triggerEvent);
    }

    private void OnValidate()
    {
        if (null == _triggerEvent)
            return;

        _debuggingName.text = _triggerEvent.EventType.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameEventManager.Instance.Submit(_gameEvent);

            if (false == _isReusable)
            {
                gameObject.SetActive(false);
            }
        }
    }

}
