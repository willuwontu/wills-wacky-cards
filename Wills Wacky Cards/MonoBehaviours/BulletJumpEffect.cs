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
    public class BulletJumpEffect : ReversibleEffect
    {
        private float interval;
        private bool ContinuousTrigger => (!(gun.attackSpeed / data.stats.attackSpeedMultiplier >= 0.3f || gun.useCharge || gun.dontAllowAutoFire));
        private Gun jumpGun;

        private readonly float minTimeFromGround = 0.1f; // minimum amount of time off the ground before this will engage

        public override void OnAwake()
        {
            jumpGun = player.gameObject.GetOrAddComponent<BulletJumpGun>();
        }

        public override void OnStart()
        {
            SetLivesToEffect(int.MaxValue);
			jumpGun = player.gameObject.GetOrAddComponent<BulletJumpGun>();
			CopyGunStats(gun, jumpGun);
			jumpGun.bursts = 1;
			jumpGun.numberOfProjectiles = 1;
		}
        public override void OnUpdate()
        {
			if (!data)
            {
				return;
            }

			if (data.dead)
            {
				return;
            }

			// If we still have normal jumps
			if (data.currentJumps > 0)
            {
				return;
            }
			// If we have no ammo
			if ((int)gunAmmo.GetFieldValue("currentAmmo") <= 0)
            {
				return;
            }

			if (!data.view.IsMine)
            {
				return;
            }

			// If we've jumped recently
			if (data.sinceJump < interval)
            {
				return;
            }
			// If we just left the ground
			if (data.sinceGrounded <= minTimeFromGround)
            {
				return;
            }

			// If we haven't pressed jump or aren't allowed to jump while holding it down
			if (!(data.playerActions.Jump.WasPressed || (ContinuousTrigger && data.playerActions.Jump.IsPressed)))
            {
				return;
            }

			if (!((bool)(typeof(PlayerVelocity).GetField("simulated", BindingFlags.Instance | BindingFlags.GetField |
                        BindingFlags.NonPublic).GetValue(data.playerVel))))
            {
				return;
            }

            // If the battle has started or we're in sandbox
            if (true)
            {
				CopyGunStats(gun, jumpGun);
				jumpGun.bursts = 1;
				jumpGun.numberOfProjectiles = 1;
				jumpGun.chargeNumberOfProjectilesTo = 0;
				jumpGun.GetAdditionalData().attacksAtFullCharge = 1;
				data.jump.Jump(true, 0.1f * gun.damage * gun.bulletDamageMultiplier * gun.projectileSpeed * gun.projectielSimulatonSpeed * (gun.useCharge ? gun.currentCharge * gun.chargeDamageMultiplier * gun.chargeSpeedTo : 1f));

                jumpGun.SetFieldValue("forceShootDir",(Vector3) Vector3.down);
                jumpGun.Attack((gun.useCharge ? gun.currentCharge: 0f), true);
				gun.currentCharge = 0f;
				this.photonView.RPC(nameof(this.RPCA_ReduceAmmo), Photon.Pun.RpcTarget.Others);
				gunAmmo.SetFieldValue("currentAmmo", (int) gunAmmo.GetFieldValue("currentAmmo") - 1);
				gunAmmo.SetFieldValue("freeReloadCounter", 0f);
				gunAmmo.InvokeMethod("SetActiveBullets", false);
				if ((int)gunAmmo.GetFieldValue("currentAmmo") <= 0)
				{
					gunAmmo.SetFieldValue("reloadCounter", (float) gunAmmo.InvokeMethod("ReloadTime"));
					gun.isReloading = true;
					gun.player.data.stats.InvokeMethod("OnOutOfAmmp", gunAmmo.maxAmmo);
				}
			}
        }

        [Photon.Pun.PunRPC]
        private void RPCA_ReduceAmmo()
        {
			gunAmmo.SetFieldValue("currentAmmo", (int)gunAmmo.GetFieldValue("currentAmmo") - 1);
			gunAmmo.SetFieldValue("freeReloadCounter", 0f);
			gunAmmo.InvokeMethod("SetActiveBullets", false);
			if ((int)gunAmmo.GetFieldValue("currentAmmo") <= 0)
			{
				gunAmmo.SetFieldValue("reloadCounter", (float)gunAmmo.InvokeMethod("ReloadTime"));
				gun.isReloading = true;
				gun.player.data.stats.InvokeMethod("OnOutOfAmmp", gunAmmo.maxAmmo);
			}
		}

        public override void OnOnDestroy()
        {
			UnityEngine.GameObject.Destroy(jumpGun);
        }
        public void SetInterval(float interval)
        {
            this.interval = interval;
        }
        public void SetContinuousTrigger(bool enabled)
        {
            continuous_trigger = enabled;
        }
        public bool GetContinuousTrigger()
        {
            return ContinuousTrigger;
        }

		public static void CopyGunStats(Gun copyFromGun, Gun copyToGun)
		{
			copyToGun.ammo = copyFromGun.ammo;
			copyToGun.ammoReg = copyFromGun.ammoReg;
			copyToGun.attackID = copyFromGun.attackID;
			copyToGun.attackSpeed = copyFromGun.attackSpeed;
			copyToGun.attackSpeedMultiplier = copyFromGun.attackSpeedMultiplier;
			copyToGun.bodyRecoil = copyFromGun.bodyRecoil;
			copyToGun.bulletDamageMultiplier = copyFromGun.bulletDamageMultiplier;
			copyToGun.bulletPortal = copyFromGun.bulletPortal;
			copyToGun.bursts = copyFromGun.bursts;
			copyToGun.chargeDamageMultiplier = copyFromGun.chargeDamageMultiplier;
			copyToGun.chargeEvenSpreadTo = copyFromGun.chargeEvenSpreadTo;
			copyToGun.chargeNumberOfProjectilesTo = copyFromGun.chargeNumberOfProjectilesTo;
			copyToGun.chargeRecoilTo = copyFromGun.chargeRecoilTo;
			copyToGun.chargeSpeedTo = copyFromGun.chargeSpeedTo;
			copyToGun.chargeSpreadTo = copyFromGun.chargeSpreadTo;
			copyToGun.cos = copyFromGun.cos;
			copyToGun.currentCharge = copyFromGun.currentCharge;
			copyToGun.damage = copyFromGun.damage;
			copyToGun.damageAfterDistanceMultiplier = copyFromGun.damageAfterDistanceMultiplier;
			copyToGun.defaultCooldown = copyFromGun.defaultCooldown;
			copyToGun.dmgMOnBounce = copyFromGun.dmgMOnBounce;
			copyToGun.dontAllowAutoFire = copyFromGun.dontAllowAutoFire;
			copyToGun.drag = copyFromGun.drag;
			copyToGun.dragMinSpeed = copyFromGun.dragMinSpeed;
			copyToGun.evenSpread = copyFromGun.evenSpread;
			copyToGun.explodeNearEnemyDamage = copyFromGun.explodeNearEnemyDamage;
			copyToGun.explodeNearEnemyRange = copyFromGun.explodeNearEnemyRange;
			copyToGun.forceSpecificAttackSpeed = copyFromGun.forceSpecificAttackSpeed;
			copyToGun.forceSpecificShake = copyFromGun.forceSpecificShake;
			copyToGun.gravity = copyFromGun.gravity;
			copyToGun.hitMovementMultiplier = copyFromGun.hitMovementMultiplier;
			//copyToGun.holdable = copyFromGun.holdable;
			copyToGun.ignoreWalls = copyFromGun.ignoreWalls;
			copyToGun.isProjectileGun = copyFromGun.isProjectileGun;
			copyToGun.isReloading = copyFromGun.isReloading;
			copyToGun.knockback = copyFromGun.knockback;
			copyToGun.lockGunToDefault = copyFromGun.lockGunToDefault;
			copyToGun.multiplySpread = copyFromGun.multiplySpread;
			copyToGun.numberOfProjectiles = copyFromGun.numberOfProjectiles;
			copyToGun.objectsToSpawn = copyFromGun.objectsToSpawn;
			copyToGun.overheatMultiplier = copyFromGun.overheatMultiplier;
			copyToGun.percentageDamage = copyFromGun.percentageDamage;
			copyToGun.player = copyFromGun.player;
			copyToGun.projectielSimulatonSpeed = copyFromGun.projectielSimulatonSpeed;
			copyToGun.projectileColor = copyFromGun.projectileColor;
			copyToGun.projectiles = copyFromGun.projectiles;
			copyToGun.projectileSize = copyFromGun.projectileSize;
			copyToGun.projectileSpeed = copyFromGun.projectileSpeed;
			copyToGun.randomBounces = copyFromGun.randomBounces;
			copyToGun.recoil = copyFromGun.recoil;
			copyToGun.recoilMuiltiplier = copyFromGun.recoilMuiltiplier;
			copyToGun.reflects = copyFromGun.reflects;
			copyToGun.reloadTime = copyFromGun.reloadTime;
			copyToGun.reloadTimeAdd = copyFromGun.reloadTimeAdd;
			copyToGun.shake = copyFromGun.shake;
			copyToGun.shakeM = copyFromGun.shakeM;
			copyToGun.ShootPojectileAction = copyFromGun.ShootPojectileAction;
			//copyToGun.shootPosition = copyFromGun.shootPosition;
			copyToGun.sinceAttack = copyFromGun.sinceAttack;
			copyToGun.size = copyFromGun.size;
			copyToGun.slow = copyFromGun.slow;
			copyToGun.smartBounce = copyFromGun.smartBounce;
			//copyToGun.soundDisableRayHitBulletSound = copyFromGun.soundDisableRayHitBulletSound;
			//copyToGun.soundGun = copyFromGun.soundGun;
			//copyToGun.soundImpactModifier = copyFromGun.soundImpactModifier;
			//copyToGun.soundShotModifier = copyFromGun.soundShotModifier;
			copyToGun.spawnSkelletonSquare = copyFromGun.spawnSkelletonSquare;
			copyToGun.speedMOnBounce = copyFromGun.speedMOnBounce;
			copyToGun.spread = copyFromGun.spread;
			copyToGun.teleport = copyFromGun.teleport;
			copyToGun.timeBetweenBullets = copyFromGun.timeBetweenBullets;
			copyToGun.timeToReachFullMovementMultiplier = copyFromGun.timeToReachFullMovementMultiplier;
			copyToGun.unblockable = copyFromGun.unblockable;
			copyToGun.useCharge = copyFromGun.useCharge;
			copyToGun.waveMovement = copyFromGun.waveMovement;

			Traverse.Create(copyToGun).Field("attackAction").SetValue((Action)Traverse.Create(copyFromGun).Field("attackAction").GetValue());
			//Traverse.Create(copyToGun).Field("gunAmmo").SetValue((GunAmmo)Traverse.Create(copyFromGun).Field("gunAmmo").GetValue());
			Traverse.Create(copyToGun).Field("gunID").SetValue((int)Traverse.Create(copyFromGun).Field("gunID").GetValue());
			Traverse.Create(copyToGun).Field("spreadOfLastBullet").SetValue((float)Traverse.Create(copyFromGun).Field("spreadOfLastBullet").GetValue());

			Traverse.Create(copyToGun).Field("forceShootDir").SetValue((Vector3)Traverse.Create(copyFromGun).Field("forceShootDir").GetValue());
		}
	}

    class BulletJumpGun : Gun
    {

    }
}
