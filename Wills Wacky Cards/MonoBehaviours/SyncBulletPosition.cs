using Photon.Pun;
using UnityEngine;

namespace WWC.MonoBehaviours
{
    internal class SyncBulletPosition : MonoBehaviour
    {
        public float interval;
        public float lastSent;
        public PhotonView view;
        private void Awake()
        {
            this.view = GetComponentInParent<PhotonView>();
            GetComponentInParent<ChildRPC>().childRPCsVector2.Add("PosSync", SyncPosition);
        }

        private void Update()
        {
            if (view != null && (view.IsMine) && Time.time > (this.lastSent + this.interval)) 
            {
                GetComponentInParent<ChildRPC>().CallFunction("PosSync", (Vector2)this.transform.root.position);
                this.lastSent = Time.time;
            }
        }

        public void SyncPosition(Vector2 pos)
        {
            if (!view.IsMine) 
            { 
                this.transform.root.position = pos; 
            }
        }

        private void OnDestroy()
        {
            GetComponentInParent<ChildRPC>().childRPCsVector2.Remove("PosSync");
        }
    }
}
