using DestinyMod.Common.Items.Modifiers;
using DestinyMod.Common.ModPlayers;
using Terraria;

namespace DestinyMod.Content.Items.Perks.Weapon.Traits
{
    public class Frenzy : ItemPerk
    {
        private int _hitTimer;

        private int _hits;

        private int _prevLife;

        private bool _apply;

        public override bool IsInstanced => true;

        public override void SetDefaults()
        {
            DisplayName = "Frenzy";
            Description = "Being in combat for an extended time increases this weapon's damage, firing speed, and controls recoil until you are out of combat.";
        }

        public void Function(ref int damage)
        {
            _hits++;

            if (_apply)
            {
                damage = (int)(damage * 1.15f);
            }
        }

        public override void ModifyHitNPC(Player player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit) => Function(ref damage);

        public override void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) => Function(ref damage);

        public override void UseSpeedMultiplier(Player player, Item item, ref float multiplier)
        {
            if (_apply)
            {
                multiplier *= 1.2f;
            }
        }

        public override void Update(Player player)
        {
            ItemDataPlayer itemDataPlayer = player.GetModPlayer<ItemDataPlayer>();
            if (_apply && itemDataPlayer.Recoil >= 0)
            {
                itemDataPlayer.Recoil += 50;
            }

            if (_hitTimer > 0)
            {
                _hitTimer--;
                return;
            }

            _hitTimer = 300;
            if (_hits > 15 || player.statLife < _prevLife)
            {
                _apply = true;
                _hits = 0;
                return;
            }
            _apply = false;
            _prevLife = player.statLife;
        }
    }
}