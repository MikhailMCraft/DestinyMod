using System;
using Terraria;
using Terraria.ModLoader;
using DestinyMod.Common.Buffs;
using Terraria.DataStructures;

namespace DestinyMod.Common.ModPlayers
{
	public class BuffPlayer : ModPlayer
	{
		public DestinyModBuff IsBuffIndexDestinyModBuff(int index)
		{
			int buffType = Player.buffType[index];
			ModBuff modBuff = ModContent.GetModBuff(buffType);
			if (modBuff == null || modBuff is not DestinyModBuff destinyModBuff)
			{
				return null;
			}

			return destinyModBuff;
		}

		public void ImplementBuffIteration(Action<DestinyModBuff> onSuccessfulIteration)
		{
			for (int indexer = 0; indexer < Player.buffType.Length; indexer++)
			{
				if (IsBuffIndexDestinyModBuff(indexer) is DestinyModBuff destinyModBuff)
				{
					onSuccessfulIteration.Invoke(destinyModBuff);
				}
			}
		}

		public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			for (int indexer = 0; indexer < Player.buffType.Length; indexer++)
			{
				if (IsBuffIndexDestinyModBuff(indexer) is DestinyModBuff destinyModBuff)
				{
					destinyModBuff.ModifyHitNPC(Player, item, target, ref damage, ref knockback, ref crit);
				}
			}
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			for (int indexer = 0; indexer < Player.buffType.Length; indexer++)
			{
				if (IsBuffIndexDestinyModBuff(indexer) is DestinyModBuff destinyModBuff)
				{
					destinyModBuff.ModifyHitNPCWithProj(Player, proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
				}
			}
		}

		public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
		{
			for (int indexer = 0; indexer < Player.buffType.Length; indexer++)
			{
				if (IsBuffIndexDestinyModBuff(indexer) is DestinyModBuff destinyModBuff)
				{
					destinyModBuff.ModifyHitPvp(Player, item, target, ref damage, ref crit);
				}
			}
		}

		public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
		{
			for (int indexer = 0; indexer < Player.buffType.Length; indexer++)
			{
				if (IsBuffIndexDestinyModBuff(indexer) is DestinyModBuff destinyModBuff)
				{
					destinyModBuff.ModifyHitPvpWithProj(Player, proj, target, ref damage, ref crit);
				}
			}
		}

		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
			ImplementBuffIteration(destinyModBuff => destinyModBuff.OnHitNPC(Player, item, target, damage, knockback, crit));
		}

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			bool output = true;
			for (int indexer = 0; indexer < Player.buffType.Length; indexer++)
			{
				if (IsBuffIndexDestinyModBuff(indexer) is DestinyModBuff destinyModBuff 
					&& destinyModBuff.PreKill(Player, damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource) == false)
				{
					output = false;
				}
			}
			return output;
		}

		public override void UpdateBadLifeRegen()
        {
			ImplementBuffIteration(destinyModBuff => destinyModBuff.UpdateBadLifeRegen(Player));
		}

		public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			for (int indexer = 0; indexer < Player.buffType.Length; indexer++)
			{
				if (IsBuffIndexDestinyModBuff(indexer) is DestinyModBuff destinyModBuff)
				{
					destinyModBuff.DrawEffects(Player, drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
				}
			}
		}

		public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
		{
			for (int indexer = 0; indexer < Player.buffType.Length; indexer++)
			{
				if (IsBuffIndexDestinyModBuff(indexer) is DestinyModBuff destinyModBuff)
				{
					destinyModBuff.ModifyHitByNPC(Player, npc, ref damage, ref crit);
				}
			}
		}

		public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
		{
			for (int indexer = 0; indexer < Player.buffType.Length; indexer++)
			{
				if (IsBuffIndexDestinyModBuff(indexer) is DestinyModBuff destinyModBuff)
				{
					destinyModBuff.ModifyHitByProjectile(Player, proj, ref damage, ref crit);
				}
			}
		}

		public override float UseTimeMultiplier(Item item)
		{
			float outPut = 1f;
			for (int indexer = 0; indexer < Player.buffType.Length; indexer++)
			{
				if (IsBuffIndexDestinyModBuff(indexer) is DestinyModBuff destinyModBuff)
				{
					outPut *= destinyModBuff.UseTimeMultiplier(Player, item);
				}
			}
			return outPut;
		}
	}
}