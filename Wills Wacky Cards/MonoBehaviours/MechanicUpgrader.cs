using System;
using Sonigon;
using Sonigon.Internal;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using UnboundLib;

namespace WWC.MonoBehaviours
{
	public class MechanicUpgrader : MonoBehaviour
	{
		public void Start()
		{
			this.soundCounterLast = this.counter;
			this.data = base.GetComponentInParent<CharacterData>();
			HealthHandler healthHandler = this.data.healthHandler;
			healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(this.ResetStuff));
			base.GetComponentInParent<ChildRPC>().childRPCs.Add("MechanicUpgrade", new Action(this.RPCA_Activate));
		}

		public void OnDestroy()
		{
			HealthHandler healthHandler = this.data.healthHandler;
			healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(this.ResetStuff));
			base.GetComponentInParent<ChildRPC>().childRPCs.Remove("MechanicUpgrade");
			this.SoundStop();
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

		private void ResetStuff()
		{
			this.SoundStop();
			this.remainingDuration = 0f;
			this.counter = 0f;
			this.upgradeLevel = 0;
			this.currentUpgradeLevel = 0;
			if (this.isUpgrading)
			{
				this.isUpgrading = false;
				//for (int i = 0; i < this.abyssalObjects.Length; i++)
				//{
				//	this.abyssalObjects[i].gameObject.SetActive(false);
				//}
				//this.data.maxHealth /= this.hpMultiplier;
				//this.data.health /= this.hpMultiplier;
				//this.data.stats.ConfigureMassAndSize();
				//this.isAbyssalForm = false;
				//this.rotator.gameObject.SetActive(false);
				//this.still.gameObject.SetActive(false);
			}
			this.SoundStop();
		}

		private void RPCA_Activate()
		{
			this.upgradeLevel += 1;
			this.remainingDuration = this.duration;
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
				this.counter = this.remainingDuration / this.duration;
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
                    this.remainingDuration = this.duration;
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

		public SoundEvent soundUpgradeChargeLoop;

		private bool soundChargeIsPlaying;

		private float soundCounterLast;

		private SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(0f, UpdateMode.Continuous);

		[Range(0f, 1f)]
		public float counter;

		public float timeToFill = 5f;

		public float timeToEmpty = 1f;

		public float duration = 1;

		public float hpMultiplier = 2f;

		public int upgradeLevel = 0;

		public int currentUpgradeLevel = 0;

		public ProceduralImage outerRing;

		public ProceduralImage fill;

		public Transform rotator;

		public Transform still;

		private CharacterData data;

		public GameObject[] upgradeObjects;

		private float remainingDuration;

		private bool isUpgrading;

		private float startCounter;
	}
}
