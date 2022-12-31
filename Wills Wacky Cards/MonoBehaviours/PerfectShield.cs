using System;
using System.Reflection;
using ModdingUtils.MonoBehaviours;
using UnboundLib;
using UnityEngine;
using HarmonyLib;
using GunChargePatch.Extensions;

namespace WWC.MonoBehaviours
{
    [DisallowMultipleComponent]
    public class PerfectShield : ProjectileHitSurface
    {
        public override HasToStop HitSurface(HitInfo hit, GameObject projectile)
        {
            Photon.Pun.PhotonNetwork.Destroy(projectile.transform.root.gameObject);
            return HasToStop.HasToStop;
        }
    }
}
