﻿using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheDestinyMod
{
    public static class DestinyHelper
    {
        /// <summary>
        /// Homes in on an NPC
        /// </summary>
        /// <param name="distance">The current distance from the NPC</param>
        /// <param name="projectile">The projectile that is being operated on</param>
        /// <param name="move">The projectile's current move position</param>
        /// <returns>True if the projectile found a target to lock onto. Otherwise returns false.</returns>
        public static bool HomeInOnNPC(float distance, Projectile projectile, ref Vector2 move) {
            bool target = false;
            for (int k = 0; k < 200; k++) {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && Main.npc[k].damage > 0 && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, Main.npc[k].position, Main.npc[k].width, Main.npc[k].height)) {
                    Vector2 newMove = Main.npc[k].Center - projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance) {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
            }
            return target;
        }

        ///<summary>
        ///Used to generate a structure using StructureHelper. You must check that the StructureHelper mod is installed or the game will crash!
        ///</summary>
        ///<param name="structure">The structure file name</param>
        ///<example>
        ///<code>
        ///if (ModLoader.GetMod("StructureHelper") != null) {
        ///    StructureHelperGenerateStructure("Example");
        ///}
        ///</code>
        ///</example>
        public static void StructureHelperGenerateStructure(string structure) {
            StructureHelper.Generator.GenerateStructure($"Structures/{structure}", new Vector2(Main.spawnTileX, Main.spawnTileY).ToPoint16(), TheDestinyMod.Instance);
        }
    }
}