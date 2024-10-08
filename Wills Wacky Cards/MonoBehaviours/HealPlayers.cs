using ModdingUtils.RoundsEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WWC.Extensions;

namespace WWC.MonoBehaviours
{
    public class HealPlayers : MonoBehaviour
    {
        CharacterData data;
        LineEffect lineffect;

        private void Start()
        {
            this.data = this.GetComponentInParent<CharacterData>();
            this.lineffect = GetComponentInChildren<LineEffect>();
        }

        public void Go()
        {
            float range = lineffect.GetRadius();

            switch (targetTeam)
            {
                case TargetTeam.Own:
                    foreach (Player player in PlayerManager.instance.players.Where(p => p.teamID == this.data.player.teamID && (this.ignoreSelf ? this.data.player.playerID != p.playerID : true) && Vector3.Distance(p.transform.position, this.data.transform.position) <= range))
                    {
                        CanSeeInfo info = PlayerManager.instance.CanSeePlayer(this.transform.position, player);

                        if (info.canSee)
                        {
                            player.data.healthHandler.Heal(amount);
                        }
                    }
                    break;
                case TargetTeam.Other:
                    foreach (Player player in PlayerManager.instance.players.Where(p => p.teamID != this.data.player.teamID && Vector3.Distance(p.transform.position, this.data.transform.position) <= range))
                    {
                        CanSeeInfo info = PlayerManager.instance.CanSeePlayer(this.transform.position, player);

                        if (info.canSee)
                        {
                            player.data.healthHandler.Heal(amount);
                        }
                    }
                    break;
                case TargetTeam.Any:
                    foreach (Player player in PlayerManager.instance.players.Where(p => (this.ignoreSelf ? this.data.player.playerID != p.playerID : true) && Vector3.Distance(p.transform.position, this.data.transform.position) <= range))
                    {
                        CanSeeInfo info = PlayerManager.instance.CanSeePlayer(this.transform.position, player);

                        if (info.canSee)
                        {
                            player.data.healthHandler.Heal(amount);
                        }
                    }
                    break;
            }
        }

        public bool ignoreSelf = true;
        public float amount = 10f;
        public TargetTeam targetTeam = TargetTeam.Own;

        public enum TargetTeam
        {
            Own,
            Other,
            Any
        }
    }
}
