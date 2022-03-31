using System.Collections.Generic;
using System.Linq;
using WWC.Extensions;
using UnityEngine;
using WillsWackyManagers.Utils;
using WWC.Interfaces;
using Photon.Pun;

namespace WWC.MonoBehaviours
{
    public class CorruptionHitEffect : RayHitEffect
    {
        public Player owner;
        public override HasToReturn DoHitEffect(HitInfo hit)
        {
            if (!hit.transform)
            {
                return HasToReturn.canContinue;
            }
            var target = hit.transform.GetComponent<Player>();
            if (target)
            {
                UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Corrupted Ammunition][Debugging] Player {target.playerID} was hit by a corrupted bullet.");
                if (ModdingUtils.AIMinion.Extensions.CharacterDataExtension.GetAdditionalData(target.data).isAIMinion)
                {
                    target = ModdingUtils.AIMinion.Extensions.CharacterDataExtension.GetAdditionalData(target.data).spawner;
                }

                var tracker = owner.GetComponent<CorruptedAmmunition_Mono>();

                if (tracker.corruptionTracker.Keys.Contains(target))
                {
                    tracker.corruptionTracker[target] += 1;
                }
                else
                {
                    tracker.corruptionTracker.Add(target, 1);
                }

                Destroy(this);
            }
            return HasToReturn.canContinue;
        }

        public void Destroy()
        {
            Destroy(this);
        }
    }

    [DisallowMultipleComponent]
    class CorruptedAmmunition_Mono : MonoBehaviourPun, IPointStartHookHandler, IPointEndHookHandler, IPickStartHookHandler, IGameStartHookHandler
    {
        private int corruptedBullets = 0;
        private GameObject bulletMono = new GameObject("CorruptedBullet", typeof(CorruptionHitEffect));
        public Dictionary<Player, int> corruptionTracker = new Dictionary<Player, int>();
        private bool increased = false;
        private int hitsNeeded = 7;
        private float multiplier = 1f;

        private CharacterData data;
        private Player player;
        private Block block;
        private WeaponHandler weaponHandler;
        private Gun gun;

        private void Start()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            data = GetComponentInParent<CharacterData>();
        }

        private void Update()
        {
            if (!player)
            {
                if (!(data is null))
                {
                    player = data.player;
                    block = data.block;
                    weaponHandler = data.weaponHandler;
                    gun = weaponHandler.gun;

                    gun.ShootPojectileAction += OnShootProjectileAction;
                }

            }
        }

        private void CheckIfValid()
        {
            var haveCard = false;
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (player.data.currentCards[i].cardName == "Corrupted Ammunition")
                {
                    haveCard = true;
                    break;
                }
            }

            if (!haveCard)
            {
                Destroy(this);
            }
        }

        private void OnShootProjectileAction(GameObject obj)
        {
            if (corruptedBullets > 0)
            {
                corruptedBullets--;
                ProjectileHit bullet = obj.GetComponent<ProjectileHit>();
                var gameObject = Instantiate(bulletMono, bullet.transform.position, bullet.transform.rotation, bullet.transform);

                var hitEffect = gameObject.GetComponent<CorruptionHitEffect>();
                hitEffect.owner = player;
            }
            if (corruptedBullets == 0)
            {
                gun.projectileColor = new Color(199f / 255f, 199f / 255f, 0f / 255f, 1f);
            }
        }

        public void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        public void OnPointStart()
        {
            CheckIfValid();
            try
            {
                CheckCorruption();
            }
            catch
            {

            }

            gun.projectileColor = new Color(58f / 255f, 34f / 255f, 66f / 255f, 1f);
            var curses = CurseManager.instance.GetAllCursesOnPlayer(player);
            corruptedBullets = curses.Count();
            if (!increased)
            {
                increased = true;
                multiplier = 1f + corruptedBullets * 0.2f;
                gun.damage *= multiplier;
                
                UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Corrupted Ammunition][Debugging] Player {player.playerID} has a damage multiplier of {gun.damage} after an increase of {multiplier * 100f}% from curses.");
            }
        }

        public void OnPointEnd()
        {
            if (increased)
            {
                increased = false;
                gun.damage /= multiplier;
                
                UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Corrupted Ammunition][Debugging] Player {player.playerID} has a damage multiplier of {gun.damage} after an decrease of {multiplier * 100f}% from curses.");
            }
        }

        public void OnPickStart()
        {
            CheckCorruption();
        }

        private void CheckCorruption()
        {
            var tracked = corruptionTracker.Keys.ToArray();
            foreach (var person in tracked)
            {
                UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Corrupted Ammunition][Debugging] Player {person.playerID} has {corruptionTracker[person]} corruption from player {player.playerID}.");
                while (corruptionTracker[person] > hitsNeeded - 1)
                {
                    if (photonView.IsMine)
                    {
                        photonView.RPC(nameof(RPCA_CursePlayer), RpcTarget.All, person.playerID);
                    }
                    corruptionTracker[person] -= hitsNeeded;
                }
            }
        }

        [PunRPC]
        private void RPCA_CursePlayer(int id)
        {
            CurseManager.instance.CursePlayer(PlayerManager.instance.GetPlayerWithID(id));
        }

        private void OnDestroy()
        {
            if (increased)
            {
                increased = false;
                gun.damage /= 1f + corruptedBullets * 0.2f;
                UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Corrupted Ammunition][Debugging] Player {player.playerID} has a damage multiplier of {gun.damage} after an decrease of {corruptedBullets * 0.2f * 100f} from curses.");
            }
            gun.ShootPojectileAction -= OnShootProjectileAction;
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
        }
    }
}
