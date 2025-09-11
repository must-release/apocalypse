using UnityEngine;

namespace AD.GamePlay
{
    [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public class PushableObject : ActorBase
    {
        /****** Public Members ******/

        public Vector3 CurrentPosition => transform.position;


        /****** Private Members ******/

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.TryGetComponent(out IObjectPusher pusher))
            {
                pusher.CurrentPushableObject = this;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.transform.TryGetComponent(out IObjectPusher pusher))
            {
                if (pusher.CurrentPushableObject == this)
                {
                    pusher.CurrentPushableObject = null;
                }
            }
        }
    }
}