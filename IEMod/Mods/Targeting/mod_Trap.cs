using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Targeting {
	
	[ModifiesType]
	public class mod_Trap : Trap
	{
		[NewMember]
		[DuplicatesBody(nameof(CanActivate))]
		private bool orig_CanActivate(GameObject victim) {
			return false;
		}

		[ModifiesMember(nameof(CanActivate))]
		public bool mod_CanActivate(GameObject victim) {
			if (!this.m_trap_initialized)
			{
				Debug.LogError("Cannot activate uninitialized trap!");
				return false;
			}
			if (IEModOptions.DisableFriendlyFire) {
				Faction victimFaction = victim?.GetComponent<Faction>();
				if (victimFaction?.isPartyMember == true && this.IsPlayerOwnedTrap)
				{
					return false;
				}
			}
			return orig_CanActivate(victim);
		}
	

		//[ModifiesMember("CanActivate")]
		private bool CanActivateNew(GameObject victim) {
			bool disableFriendlyFire = IEModOptions.DisableFriendlyFire;

			if (!this.m_trap_initialized)
			{
				Debug.LogError("Cannot activate uninitialized trap!");
				return false;
			}

			if (disableFriendlyFire)
			{
				Faction victimFaction = victim.GetComponent<Faction>();

				if (victimFaction.isPartyMember && this.IsPlayerOwnedTrap)
				{
					return false;
				}
			}

			if (this.Disarmed || this.m_triggerVictim != null)
			{
				return false;
			}
			if (this.MaxHitCount > 0 && this.m_numHits >= this.MaxHitCount)
			{
				return false;
			}
			if (GameUtilities.IsAnimalCompanion(victim))
			{
				return false;
			}
			if (this.m_ocl != null)
			{
				return true;
			}
			if (this.ActivatesForAnyone)
			{
				return true;
			}
			Faction component = victim.GetComponent<Faction>();
			return component != null && (component.IsHostile(base.gameObject) || this.m_trap_faction.IsHostile(victim));
		}
	}
}