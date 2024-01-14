using System;
using Sonigon;
using Sonigon.Internal;
using UnityEngine;
using TMPro;
using UnityEngine.UI.ProceduralImage;
using UnboundLib;
using WWC.Interfaces;
using WWC.Extensions;
using ModdingUtils.Extensions;

namespace WWC.MonoBehaviours
{
	public class MechanicUpgrader : MonoBehaviour, IRoundEndHookHandler, IPointStartHookHandler
	{
		public void Start()
		{
			this.soundCounterLast = this.counter;
			this.data = base.GetComponentInParent<CharacterData>();
			HealthHandler healthHandler = this.data.healthHandler;
			healthHandler.reviveAction += OnRevive;
			base.GetComponentInParent<ChildRPC>().childRPCs.Add("MechanicUpgrade", new Action(this.RPCA_Upgrade));
			InterfaceGameModeHooksManager.instance.RegisterHooks(this);
		}

		public void OnDestroy()
		{
			HealthHandler healthHandler = this.data.healthHandler;
			healthHandler.reviveAction -= OnRevive;
			base.GetComponentInParent<ChildRPC>().childRPCs.Remove("MechanicUpgrade");
			this.SoundStop();
			InterfaceGameModeHooksManager.instance.RemoveHooks(this);

			if (cloneActionAttached)
            {
				this.data.healthHandler.reviveAction -= OnCloneAction;
            }

			if (levelFrame)
            {
				UnityEngine.GameObject.Destroy(levelFrame);
            }
		}

		private void OnRevive()
        {
			if (this.isUpgrading)
            {
				this.remainingDuration = Mathf.Clamp(remainingDuration + (UpgradeCooldown / 4f), 0, UpgradeCooldown);
				return;
            }
			this.counter = this.counter / 2f;
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

		public void OnRoundEnd()
        {
			this.ResetStuff();
        }

        public void OnPointStart()
        {
			for (int i = 0; i < this.upgradeLevel; i++)
			{
                this.data.player.gameObject.AddComponent<MechanicUpgrade>();

                if (upgradeAction != null)
                {
                    upgradeAction(upgradeLevel);
                }
            }

            this.remainingDuration = 0;
            this.isUpgrading = true;
            this.counter = 0.1f;
        }

        private void ResetStuff()
		{
			this.remainingDuration = 0f;
			this.counter = 0.1f;
			this.upgradeLevel = 0;
			this.currentUpgradeLevel = 0;
			this.levelText.text = $"{upgradeLevel}";
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
			this.remainingDuration = this.UpgradeCooldown;
			this.upgradeLevel++;
			this.levelText.text = $"{upgradeLevel}";

			if (upgradeAction != null)
            {
				upgradeAction(upgradeLevel);
            }
		}

		private void Upgrade()
        {
			base.GetComponentInParent<ChildRPC>().CallFunction("MechanicUpgrade");
			this.counter = 1f;
		}

		private void Update()
		{
			AdjustColors(isUpgrading);

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
					currentUpgradeLevel = upgradeLevel;
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
				this.counter = this.remainingDuration / this.UpgradeCooldown;
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
                if (this.data.input.direction == Vector3.zero || this.data.input.direction == Vector3.down || this.data.input.direction == Vector3.up)
                {
                    this.counter += TimeHandler.deltaTime / this.UpgradeTime;
                }
                else
                {
                    this.counter -= TimeHandler.deltaTime / this.UpgradeTimeWhileMoving;
                }
            }
			catch (Exception e)
			{
				UnityEngine.Debug.Log("First Catch");
				UnityEngine.Debug.LogException(e);
			}
            try
            {
                this.counter = Mathf.Clamp(this.counter, -0.1f / this.UpgradeTime, 1f);
                if (this.counter >= 1f && this.data.view.IsMine)
                {
                    this.remainingDuration = this.UpgradeCooldown;
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
		}

		private void AdjustColors(bool onCooldown)
		{
			int index = onCooldown ? 1 : 0;

			this.backRing.color = backRingColors[index];
			this.rotateImage.color = ringColors[index];
			this.topImage.color = ringColors[index];
			this.fill.color = backgroundColors[index];
			this.outerRing.color = ringColors[index];
		}

		private Color[] backRingColors = new Color[]
{
			new Color32(255, 167, 0, 255),
			new Color32(161, 56, 52, 29)
};

		private Color[] ringColors = new Color[]
		{
			new Color32(255, 167, 0, 255),
			new Color32(255, 69, 0, 255)
		};

		private Color[] backgroundColors = new Color[]
		{
			new Color32(255, 196, 0, 10),
			new Color32(255, 41, 0, 5)
		};

		private ProceduralImage backRing
        {
			get
            {
				return this.gameObject.transform.Find("Canvas/Size/BackRing").GetComponent<ProceduralImage>();
			}
        }
        private ProceduralImage rotateImage
        {
			get
            {
				return rotator.gameObject.GetComponentInChildren<ProceduralImage>();
			}
        }
        private ProceduralImage topImage
        {
			get
            {
				return still.gameObject.GetComponentInChildren<ProceduralImage>();
			}
        }
        public SoundEvent soundUpgradeChargeLoop;

		private bool soundChargeIsPlaying;

		private float soundCounterLast;

		private SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(0f, UpdateMode.Continuous);

		[Range(0f, 1f)]
		public float counter;

		private float upgradeTime = 5f;

		public float upgradeTimeAdd;

		public float upgradeTimeMult = 1f;

		public float UpgradeTime
        {
			get
            {
				return ((upgradeTime + upgradeTimeAdd) * upgradeTimeMult);
            }
        }

		public float UpgradeTimeWhileMoving
		{
			get
			{
				return (timeToEmpty >= 0 ? timeToEmpty : ((timeToEmpty - upgradeTimeAdd) * upgradeTimeMult));
			}
		}

		public float timeToEmpty = 0.75f;

		private float upgradeCooldown = 8f;

		public float upgradeCooldownAdd;

		public float upgradeCooldownMult = 1f;

		public float UpgradeCooldown
        {
			get
            {
				return ((upgradeCooldown + upgradeCooldownAdd) * upgradeCooldownMult);
            }
        }
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

		public GameObject levelFrame = null;

		public TextMeshProUGUI levelText = null;

		public float extraBlockTime = 0f;
		public float regenAdd = 0f;
		public int extraJumps = 0;
		public GunStatModifier gunStatModifier = new GunStatModifier();
		public GunAmmoStatModifier gunAmmoStatModifier = new GunAmmoStatModifier();
		public CharacterDataModifier characterDataModifier = new CharacterDataModifier();
		public CharacterStatModifiersModifier characterStatModifiersModifier = new CharacterStatModifiersModifier();
		public GravityModifier gravityModifier = new GravityModifier();
		public BlockModifier blockModifier = new BlockModifier();

		public Action<int> upgradeAction;
	}

	public class MechanicUpgrade : ReversibleEffect, IPointEndHookHandler
    {
		private float extraBlockTime = 0f;
		private float extraRegen = 0f;
		private int extraJumps = 0;

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
			health.regeneration += extraRegen = upgrader.regenAdd;
			data.jumps += extraJumps = upgrader.extraJumps;
			data.currentJumps += extraJumps;
		}

		public void OnPointEnd()
        {
			UnityEngine.GameObject.Destroy(this);
        }

        public override void OnOnDestroy()
        {
			InterfaceGameModeHooksManager.instance.RemoveHooks(this);
			WWC.Extensions.CharacterStatModifiersExtension.GetAdditionalData(stats).extraBlockTime -= extraBlockTime;
			health.regeneration -= extraRegen;
			data.jumps -= extraJumps;
			data.currentJumps -= extraJumps;
		}
    }

	public class ClonedWeakness : ReversibleEffect, IPointStartHookHandler, IGameStartHookHandler, IBattleStartHookHandler, IPointEndHookHandler
    {
		public override void OnStart()
		{
			InterfaceGameModeHooksManager.instance.RegisterHooks(this);
			applyImmediately = true;
			this.SetLivesToEffect(int.MaxValue);

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
        public void OnPointEnd()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        public override void OnOnDestroy()
		{
			InterfaceGameModeHooksManager.instance.RemoveHooks(this);
		}
	}
}
