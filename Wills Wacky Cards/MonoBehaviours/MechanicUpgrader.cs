using System;
using Sonigon;
using Sonigon.Internal;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using UnboundLib;
using WWC.Interfaces;
using WWC.Extensions;
using ModdingUtils.Extensions;

namespace WWC.MonoBehaviours
{
	public class MechanicUpgrader : MonoBehaviour, IPointEndHookHandler, IPointStartHookHandler
	{
		public void Start()
		{
			this.soundCounterLast = this.counter;
			this.data = base.GetComponentInParent<CharacterData>();
			HealthHandler healthHandler = this.data.healthHandler;
			base.GetComponentInParent<ChildRPC>().childRPCs.Add("MechanicUpgrade", new Action(this.RPCA_Upgrade));
			InterfaceGameModeHooksManager.instance.RegisterHooks(this);
		}

		public void OnDestroy()
		{
			HealthHandler healthHandler = this.data.healthHandler;
			base.GetComponentInParent<ChildRPC>().childRPCs.Remove("MechanicUpgrade");
			this.SoundStop();
			InterfaceGameModeHooksManager.instance.RemoveHooks(this);

			if (cloneActionAttached)
            {
				this.data.healthHandler.reviveAction -= OnCloneAction;
            }
		}

		public void OnDisable()
		{
			this.SoundStop();
		}

		private void SoundPlay()
		{
			if (!this.soundChargeIsPlaying)
			{
				this.soundChargeIsPlaying = true;
				SoundManager.Instance.Play(this.soundUpgradeChargeLoop, base.transform, new SoundParameterBase[]
				{
				this.soundParameterIntensity
				});
			}
		}

		private void SoundStop()
		{
			if (this.soundChargeIsPlaying)
			{
				this.soundChargeIsPlaying = false;
				SoundManager.Instance.Stop(this.soundUpgradeChargeLoop, base.transform, true);
			}
		}

		public void OnPointEnd()
        {
			this.ResetStuff();
        }

		private void ResetStuff()
		{
			this.SoundStop();
			this.remainingDuration = 0f;
			this.counter = 0f;
			this.upgradeLevel = 0;
			this.currentUpgradeLevel = 0;
			if (this.isUpgrading)
			{
                for (int i = 0; i < this.upgradeObjects.Length; i++)
                {
                    this.upgradeObjects[i].gameObject.SetActive(false);
                }
                this.rotator.gameObject.SetActive(false);
                this.still.gameObject.SetActive(false);
				this.isUpgrading = false;
			}
			this.SoundStop();
		}

		private void RPCA_Upgrade()
		{
			var upgrade = this.data.player.gameObject.AddComponent<MechanicUpgrade>();
			this.remainingDuration = this.upgradeCooldown;
		}

		public void OnPointStart()
        {
			this.remainingDuration = this.upgradeCooldown;
			this.isUpgrading = true;
		}

		private void Update()
		{
			if (this.soundCounterLast < this.counter)
			{
				this.SoundPlay();
			}
			else
			{
				this.SoundStop();
			}
			this.soundCounterLast = this.counter;
			this.soundParameterIntensity.intensity = this.counter;
			this.outerRing.fillAmount = this.counter;
			this.fill.fillAmount = this.counter;
			this.rotator.transform.localEulerAngles = new Vector3(0f, 0f, -Mathf.Lerp(0f, 360f, this.counter));
			if (!((bool)this.data.playerVel.GetFieldValue("simulated")))
			{
				this.startCounter = 1f;
				return;
			}
			this.startCounter -= TimeHandler.deltaTime;
			if (this.startCounter > 0f)
			{
				return;
			}
			if (this.remainingDuration > 0f)
			{
				if (!this.isUpgrading)
				{
					this.isUpgrading = true;
					//for (int i = 0; i < this.abyssalObjects.Length; i++)
					//{
					//	this.abyssalObjects[i].gameObject.SetActive(true);
					//}
					//this.data.maxHealth *= this.hpMultiplier;
					//this.data.health *= this.hpMultiplier;
					//this.data.stats.ConfigureMassAndSize();
					//this.isAbyssalForm = true;
				}
				this.remainingDuration -= TimeHandler.deltaTime;
				this.counter = this.remainingDuration / this.upgradeCooldown;
				return;
			}
			if (this.isUpgrading)
			{
				this.isUpgrading = false;
				//for (int j = 0; j < this.abyssalObjects.Length; j++)
				//{
				//	this.abyssalObjects[j].gameObject.SetActive(false);
				//}
				//this.data.maxHealth /= this.hpMultiplier;
				//this.data.health /= this.hpMultiplier;
				//this.data.stats.ConfigureMassAndSize();
				//this.isAbyssalForm = false;
			}
            try
            {
                if (this.data.input.direction == Vector3.zero || this.data.input.direction == Vector3.down)
                {
                    this.counter += TimeHandler.deltaTime / this.timeToFill;
                }
                else
                {
                    this.counter -= TimeHandler.deltaTime / this.timeToEmpty;
                }
            }
			catch (Exception e)
			{
				UnityEngine.Debug.Log("First Catch");
				UnityEngine.Debug.LogException(e);
			}
            try
            {
                this.counter = Mathf.Clamp(this.counter, -0.1f / this.timeToFill, 1f);
                if (this.counter >= 1f && this.data.view.IsMine)
                {
                    this.remainingDuration = this.upgradeCooldown;
                    base.GetComponentInParent<ChildRPC>().CallFunction("MechanicUpgrade");
                }
            }
			catch (Exception e)
			{
				UnityEngine.Debug.Log("Second Catch");
				UnityEngine.Debug.LogException(e);
			}
			try
            {
                if (this.counter <= 0f)
                {
                    this.rotator.gameObject.SetActive(false);
                    this.still.gameObject.SetActive(false);
                    return;
                }
                this.rotator.gameObject.SetActive(true);
                this.still.gameObject.SetActive(true);
            }
			catch (Exception e)
			{
				UnityEngine.Debug.Log("Last Catch");
				UnityEngine.Debug.LogException(e);
			}
		}

		public void AttachCloneAction()
        {
			this.data.healthHandler.reviveAction += OnCloneAction;
			cloneActionAttached = true;
        }

		private void OnCloneAction()
        {
			this.player.gameObject.AddComponent<ClonedWeakness>();
			this.remainingDuration = upgradeCooldown;
			this.isUpgrading = true;
		}

		public SoundEvent soundUpgradeChargeLoop;

		private bool soundChargeIsPlaying;

		private float soundCounterLast;

		private SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(0f, UpdateMode.Continuous);

		[Range(0f, 1f)]
		public float counter;

		public float timeToFill = 5f;

		public float timeToEmpty = 1f;

		public float upgradeCooldown = 1;

		public float hpMultiplier = 2f;

		public int upgradeLevel = 0;

		public int currentUpgradeLevel = 0;

		public ProceduralImage outerRing;

		public ProceduralImage fill;

		public Transform rotator;

		public Transform still;

		private CharacterData data;

		public Player player
        {
			get
            {
				return data.player;
            }
        }
        public GameObject[] upgradeObjects = new GameObject[] { };

		private float remainingDuration;

		private bool isUpgrading;

		private float startCounter;

		private bool cloneActionAttached = false;

		public float extraBlockTime = 0f;
		public GunStatModifier gunStatModifier = new GunStatModifier();
		public GunAmmoStatModifier gunAmmoStatModifier = new GunAmmoStatModifier();
		public CharacterDataModifier characterDataModifier = new CharacterDataModifier();
		public CharacterStatModifiersModifier characterStatModifiersModifier = new CharacterStatModifiersModifier();
		public GravityModifier gravityModifier = new GravityModifier();
		public BlockModifier blockModifier = new BlockModifier();
	}

	public class MechanicUpgrade : ReversibleEffect, IPointEndHookHandler
    {
		private float extraBlockTime = 0f;

        public override void OnStart()
        {
			InterfaceGameModeHooksManager.instance.RegisterHooks(this);
			applyImmediately = true;
			this.SetLivesToEffect(int.MaxValue);

			var upgrader = player.GetComponentInChildren<MechanicUpgrader>();
			this.gunStatModifier = upgrader.gunStatModifier.Copy();
			this.characterDataModifier = upgrader.characterDataModifier.Copy();
			this.blockModifier = upgrader.blockModifier.Copy();
			this.characterStatModifiersModifier = upgrader.characterStatModifiersModifier.Copy();
			this.gunAmmoStatModifier = upgrader.gunAmmoStatModifier.Copy();
			this.gravityModifier = upgrader.gravityModifier.Copy();
			extraBlockTime = upgrader.extraBlockTime;
			WWC.Extensions.CharacterStatModifiersExtension.GetAdditionalData(stats).extraBlockTime += extraBlockTime;
			block.UpdateParticleDuration();
		}

		public void OnPointEnd()
        {
			UnityEngine.GameObject.Destroy(this);
        }

        public override void OnOnDestroy()
        {
			InterfaceGameModeHooksManager.instance.RemoveHooks(this);
			WWC.Extensions.CharacterStatModifiersExtension.GetAdditionalData(stats).extraBlockTime -= extraBlockTime;
		}
    }

	public class ClonedWeakness : ReversibleEffect, IPointStartHookHandler, IGameStartHookHandler, IBattleStartHookHandler
    {
		public override void OnStart()
		{
			InterfaceGameModeHooksManager.instance.RegisterHooks(this);
			applyImmediately = true;
			this.SetLivesToEffect(int.MaxValue);

			characterDataModifier.health_mult = 0.7f;
			characterDataModifier.maxHealth_mult = 0.7f;
		}

		public void OnPointStart()
        {
			UnityEngine.GameObject.Destroy(this);
        }
		public void OnGameStart()
		{
			UnityEngine.GameObject.Destroy(this);
		}
		public void OnBattleStart()
		{
			UnityEngine.GameObject.Destroy(this);
		}

		public override void OnOnDestroy()
		{
			InterfaceGameModeHooksManager.instance.RemoveHooks(this);
		}
	}
}
