using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnboundLib;

namespace WWC
{
    public static class RandomKilling
    {
        static Coroutine killRoutine;

        public static void StartKilling()
        {
            if (!PlayerManager.instance)
            {
                return;
            }
            if (!(PlayerManager.instance.players.Count > 0))
            {
                return;
            }
            if (!(PlayerManager.instance.players.Any(p => !p.data.dead)))
            {
                return;
            }

            killRoutine = Unbound.Instance.StartCoroutine(IKill());
        }

        public static IEnumerator IKill()
        {
            List<Player> playersToKill = PlayerManager.instance.players.Where(p => !p.data.dead).ToList();

            float wait;

            while (playersToKill.Count > 0)
            {
                wait = UnityEngine.Random.Range(15f, 60f);
                yield return new WaitForSeconds(wait);

                int i = UnityEngine.Random.Range(0, playersToKill.Count);
                Player p = playersToKill[i];
                playersToKill.RemoveAt(i);

                if (!p.data.dead)
                {
                    p.data.view.RPC("RPCA_Die", RpcTarget.All, new object[] { Vector2.up });
                }
            }

            yield break;
        }
    }
}
