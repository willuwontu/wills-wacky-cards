using System;
using ModdingUtils.RoundsEffects;
using UnityEngine;

namespace WillsWackyCards.MonoBehaviours
{
	public class SavageWounds_Mono : HitEffect
	{
		public float duration = 0f;
		public override void DealtDamage(Vector2 damage, bool selfDamage, Player damagedPlayer = null)
		{
			var wounds = damagedPlayer.gameObject.AddComponent<SavageWoundsDamage_Mono>();
			wounds.duration = duration;
			wounds.start = true;
		}
	}
}
