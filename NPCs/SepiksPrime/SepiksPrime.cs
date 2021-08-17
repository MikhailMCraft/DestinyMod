using System;
using System.IO;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;

namespace TheDestinyMod.NPCs.SepiksPrime
{
    [AutoloadBossHead]
    public class SepiksPrime : ModNPC
    {
        private int timesFiredThisCycle;

        private int phase = 1;
        //phases:
        //1 - initial
        //2 - 75% health shield
        //3 - 75%-50
        //4 - 50-25%
        //5 - 25% health shield
        //6 - last

        private bool shielded = false;

        private Vector2 NewCenter;

        private List<Dust> velocityChanger = new List<Dust>();

        private int rad = 120;

        public override void SetStaticDefaults() {
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults() {
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.boss = true;
            npc.npcSlots = 40f;
            npc.width = 198;
            npc.height = 186;
            npc.knockBackResist = 0f;
            npc.damage = 10;
            npc.defense = 10;
            npc.lifeMax = 7500;
            npc.value = Item.buyPrice(0, 6, 0, 0);
            npc.DeathSound = null;
            npc.HitSound = SoundID.NPCHit4;
            npc.lavaImmune = true;
            npc.noTileCollide = true;
            for (int k = 0; k < npc.buffImmune.Length; k++) {
                npc.buffImmune[k] = true;
            }
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/SepiksPrime");
            bossBag = ModContent.ItemType<Items.BossBags.SepiksPrimeBag>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale) {
            npc.lifeMax = (int)(npc.lifeMax / Main.expertLife * 1.2f * bossLifeScale);
            npc.defense = 25;
            npc.damage = 20;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile) {
            if (projectile.damage > npc.life && DestinyConfig.Instance.SepiksDeathAnimation) {
                npc.life = 49;
                return false;
            }
            return base.CanBeHitByProjectile(projectile);
        }

        public override void AI() {
            npc.ai[0]++;
            Player target = Main.player[npc.target];
            if (npc.life > 50 && DestinyConfig.Instance.SepiksDeathAnimation || !DestinyConfig.Instance.SepiksDeathAnimation) {
                npc.rotation = (float)Math.Atan2(npc.position.Y + npc.height - 80f - target.position.Y - (target.height / 2), npc.position.X + (npc.width / 2) - target.position.X - (target.width / 2)) + (float)Math.PI / 2f;
            }
            else if (npc.life < 50 && !npc.dontTakeDamage && DestinyConfig.Instance.SepiksDeathAnimation) { //doesnt work with others
                npc.dontTakeDamage = true;
                npc.rotation += 1;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPC/SepiksDie"), npc.Center);
                npc.ai[0] = 0;
                npc.alpha = 0;
                return;
            }
            else if (npc.life < 50 && npc.dontTakeDamage && DestinyConfig.Instance.SepiksDeathAnimation) {
                npc.rotation += 1;
                if (npc.ai[0] > 192) {
                    npc.StrikeNPC(9999, 0, 0); //has to be 9999 damage because...terraria
                }
                return;
            }
            if (!target.active || target.dead) {
                npc.TargetClosest(true);
                target = Main.player[npc.target];
                if (!target.active || target.dead) {
                    Main.PlaySound(SoundID.Item78, npc.Center);
                    npc.position = new Vector2(npc.position.X + 1000, npc.position.Y);
                    if (npc.timeLeft > 10) {
                        npc.timeLeft = 10;
                    }
                }
            }
            if (NewCenter != Vector2.Zero && npc.ai[3] < 40) {
                if (npc.ai[3] < 20) {
                    npc.alpha += 13;
                    float num = NewCenter.X + (npc.width / 2) - (npc.Center.X + npc.width / 2);
                    float num2 = NewCenter.Y + (npc.height / 2) - (npc.Center.Y + npc.height / 2);
                    float numFinal = (float)Math.Sqrt(num * num + num2 * num2);
                    num *= 5f / numFinal;
                    num2 *= 5f / numFinal;
                    for (int i = 0; i < 50; i++) {
                        double radius = Math.Sqrt(Main.rand.NextDouble());
                        double rand = Main.rand.NextDouble() * (Math.PI * 2);
                        Vector2 vector = npc.Center + new Vector2((float)(radius * Math.Cos(rand)), (float)(radius * Math.Sin(rand))) * ((npc.width - rad) / 2);
                        Dust dust = Dust.NewDustDirect(vector, 1, 1, DustID.WhiteTorch, Scale: 1.4f);
                        velocityChanger.Add(dust);
                        dust.noGravity = true;
                        dust.velocity.X = num;
                        dust.velocity.Y = num2;
                    }
                    rad -= 4;
                    //npc.scale -= 0.01f;
                }
                foreach (Dust dust in velocityChanger) {
                    dust.velocity *= 1.05f;
                }
                npc.ai[3]++;
                return;
            }
            if (npc.ai[3] >= 40) {
                if (NewCenter != Vector2.Zero) {
                    npc.Center = NewCenter;
                    velocityChanger.Clear();
                    rad = 120;
                }
                NewCenter = Vector2.Zero;
                if (npc.alpha > 0) {
                    npc.alpha -= 13;
                    //npc.scale += 0.01f;
                }
                else {
                    npc.ai[0] = 0;
                    npc.alpha = 0;
                    npc.ai[3] = 0;
                    timesFiredThisCycle = 0;
                    npc.defense /= 3;
                    if (npc.dontTakeDamage && npc.life > 50) {
                        SummonServitors();
                    }
                }
                return;
            }
            if (npc.timeLeft <= 10) {
                return;
            }
            if (npc.dontTakeDamage && npc.life > 50) {
                if ((phase == 2 || phase == 5) && !CheckShieldedPhase()) {
                    phase = phase == 2 ? 3 : 6;
                    npc.damage = Main.expertMode ? 20 : 10;
                    npc.dontTakeDamage = false;
                    shielded = false;
                    npc.ai[0] = 0f;
                    TeleportNearTarget();
                    if (Main.netMode == NetmodeID.Server) {
                        ModPacket netMessage = GetPacket(SepiksBossMessageType.DontTakeDamage);
                        netMessage.Write(false);
                        netMessage.Send();
                    }
                }
            }
            if (phase == 1 && npc.life <= npc.lifeMax - npc.lifeMax / 4 || phase == 4 && npc.life <= npc.lifeMax / 4) {
                phase = phase == 1 ? 2 : 5;
                shielded = true;
                npc.damage = Main.expertMode ? 40 : 20;
                npc.dontTakeDamage = true;
                TeleportNearTarget(target.Center.X + Main.rand.Next(-50, 50), target.Center.Y - 350);
                Main.PlaySound(SoundID.Item78, npc.Center);
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, $"Sounds/NPC/SepiksGroan{(Main.rand.NextBool() ? "1" : "2")}"), npc.Center);
                if (Main.netMode == NetmodeID.Server) {
                    ModPacket netMessage = GetPacket(SepiksBossMessageType.DontTakeDamage);
                    netMessage.Write(true);
                    netMessage.Send();
                }
            }
            else if (phase == 3 && npc.life <= npc.lifeMax / 2) {
                phase = 4;
                npc.ai[0] = 0f;
                timesFiredThisCycle = 0;
            }
            if (phase == 1) {
                if ((target.Center - npc.Center).Length() > 1000) {
                    TeleportNearTarget();
                    npc.ai[0] = 0f;
                    timesFiredThisCycle = 0;
                }
                if (npc.ai[0] > 50f && timesFiredThisCycle < 3) {
                    FireBlastAtTarget();
                    npc.ai[0] = 0f;
                    timesFiredThisCycle++;
                }
                if (npc.ai[0] > 120f && timesFiredThisCycle >= 3) {
                    TeleportNearTarget();
                    timesFiredThisCycle = 0;
                    npc.ai[0] = 0f;
                    Main.LocalPlayer.AddBuff(195, 3);
                }
            }
            else if (phase == 3) {
                if (npc.ai[0] == 40f || npc.ai[0] == 55f || npc.ai[0] == 70f) {
                    if (timesFiredThisCycle >= 3)
                        return;
                    timesFiredThisCycle++;
                    FireBlastAtTarget();
                }
                else if (timesFiredThisCycle >= 3 && npc.ai[0] > 130f) {
                    npc.ai[0] = 0f;
                    timesFiredThisCycle = 0;
                    TeleportNearTarget();
                }
            }
            else if (phase == 4) {
                if (npc.ai[0] == 100f && timesFiredThisCycle < 3 || npc.ai[0] == 80f && timesFiredThisCycle < 3 && Main.expertMode) {
                    FireHomingAtTarget();
                    npc.ai[0] = 0f;
                    timesFiredThisCycle++;
                }
                else if (npc.ai[0] > 90f && timesFiredThisCycle >= 3 || npc.ai[0] > 70f && timesFiredThisCycle >= 3 && Main.expertMode) {
                    TeleportNearTarget(target.Center.X + Main.rand.Next(-50, 50), target.Center.Y - 250);
                    Main.PlaySound(SoundID.Item78, npc.position);
                    npc.ai[0] = 0f;
                    timesFiredThisCycle = 0;
                }
            }
            else if (phase == 6) {
                if (npc.ai[0] == 30f) {
                    FireBlastAtTarget();
                }
                else if (npc.ai[0] == 65f) {
                    npc.ai[0] = 0f;
                    TeleportNearTarget();
                }
            }
            if (shielded) {
                if (npc.ai[0] > 80f || npc.ai[0] > 70f && Main.expertMode) {
                    FireBlastAtTarget();
                    npc.ai[0] = 0f;
                }
            }
            if ((target.Center - npc.Center).Length() < 100 && !shielded) {
                TeleportNearTarget();
                npc.ai[0] = 0f;
            }
        }

        public override void FindFrame(int frameHeight) {
            if (phase == 2 || phase == 3) {
                npc.frame.Y = frameHeight;
            }
            else if (phase == 4) {
                npc.frame.Y = frameHeight * 2;
            }
            else if (phase >= 5) {
                npc.frame.Y = frameHeight * 3;
            }
        }

        public override void DrawEffects(ref Color drawColor) {
            if (NewCenter != Vector2.Zero) {
                drawColor = Color.White;
            }
        }

        /// <summary>
        /// Teleports Sepiks near a target
        /// </summary>
        /// <param name="x">The X value of where to force Sepiks in world coordinates</param>
        /// <param name="y">The Y value of where to force Sepiks in world coordinates</param>
        private void TeleportNearTarget(float x = 0, float y = 0) {
            Vector2 global = Vector2.Zero;
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                Player target = Main.player[npc.target];
                bool teleportSuccess = false;
                int attempts = 0;
                while (!teleportSuccess) {
                    attempts++;
                    Vector2 teleportTo = new Vector2(target.Center.X + Main.rand.Next(-200, 200), target.Center.Y - Main.rand.Next(50, 300));
                    if (phase == 3 || phase == 6) { //because it's so spontaneous this gives the player more breathing room so sepiks isn't so close as he is normally
                        teleportTo = new Vector2(target.Center.X + Main.rand.Next(-300, 300), target.Center.Y - Main.rand.Next(50, 400));
                    }
                    if (x > 0 && y > 0) {
                        teleportTo = new Vector2(x, y);
                    }
                    Point tileToGoTo = teleportTo.ToTileCoordinates();
                    //ensures that sepiks won't spawn in tiles, and that he'll spawn a moderate distance from the ground
                    if (WorldGen.EmptyTileCheck((int)(tileToGoTo.X - 3.125), (int)(tileToGoTo.X + 3.125), (int)(tileToGoTo.Y - 3.125), (int)(tileToGoTo.Y + 3.125)) && WorldGen.EmptyTileCheck(tileToGoTo.X, tileToGoTo.X, tileToGoTo.Y, tileToGoTo.Y + 20) && WorldGen.InWorld(tileToGoTo.X, tileToGoTo.Y) || x > 0 && y > 0) { // && (target.Center - teleportTo).Length() > 200
                        global = teleportTo;
                        Main.PlaySound(SoundID.Item78, npc.Center);
                        npc.defense *= 3;
                        teleportSuccess = true;
                        npc.netUpdate = true;
                    }
                    if (attempts >= 30000) {
                        return;
                    }
                }
            }
            NewCenter = global;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor) {
            if (NewCenter == Vector2.Zero) {
                spriteBatch.Draw(mod.GetTexture("NPCs/SepiksPrime/SepiksPrime_Glow"), npc.Center - Main.screenPosition + new Vector2(0, 4), npc.frame, Color.LightYellow, npc.rotation, npc.frame.Size() / 2, npc.scale, SpriteEffects.None, 0);
            }
        }

        /// <summary>
        /// Checks if Sepiks should have his shield up
        /// </summary>
        /// <returns>True if a Sepiks Servitor was found in the world. Otherwise returns false.</returns>
        private bool CheckShieldedPhase() {
            for (int k = 0; k < 200; k++) {
                if (Main.npc[k].active && Main.npc[k].type == ModContent.NPCType<SepiksServitor>()) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Summons Sepiks' servitors
        /// </summary>
        private void SummonServitors() {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                NPC.NewNPC((int)npc.Center.X + 150, (int)npc.Center.Y + 20, ModContent.NPCType<SepiksServitor>());
                NPC.NewNPC((int)npc.Center.X - 150, (int)npc.Center.Y + 20, ModContent.NPCType<SepiksServitor>());
                NPC.NewNPC((int)npc.Center.X + 130, (int)npc.Center.Y + 120, ModContent.NPCType<SepiksServitor>());
                NPC.NewNPC((int)npc.Center.X - 130, (int)npc.Center.Y + 120, ModContent.NPCType<SepiksServitor>());
                NPC.NewNPC((int)npc.Center.X + 500, (int)npc.Center.Y - 100, ModContent.NPCType<Fallen.Skiff>(), 0, npc.whoAmI);
            }
        }

        /// <summary>
        /// Fires an Eye Blast at the current target
        /// </summary>
        private void FireBlastAtTarget() {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                Player target = Main.player[npc.target];
                Vector2 delta = target.Center - npc.Center;
                float magnitude = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
                if (magnitude > 0) {
                    delta *= 10f / magnitude;
                }
                else {
                    delta = new Vector2(0f, 5f);
                }
                Projectile.NewProjectile(npc.Center, delta.RotatedByRandom(MathHelper.ToRadians(10)), ModContent.ProjectileType<SepiksBlast>(), 20, 5, Main.myPlayer, npc.whoAmI);
                Main.PlaySound(SoundID.Item8, npc.Center);
                npc.netUpdate = true;
            }
        }

        /// <summary>
        /// Fires a Homing Eye Blast at the current target
        /// </summary>
        private void FireHomingAtTarget() {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<SepiksHoming>(), ai0: npc.whoAmI);
                Main.PlaySound(SoundID.Item8, npc.Center);
                npc.netUpdate = true;
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) {
            scale = 1.5f;
            if (NewCenter != Vector2.Zero) {
                scale = 0f;
            }
            return null;
        }

        public override void NPCLoot() {
            if (Main.expertMode) {
                npc.DropBossBags();
            }
            if (Main.rand.NextBool(7) && !Main.expertMode) {
                Item.NewItem(npc.Hitbox, ModContent.ItemType<Items.Vanity.SepiksPrimeMask>());
            }
            if (Main.rand.NextBool(10)) {
                Item.NewItem(npc.Hitbox, ModContent.ItemType<Items.Placeables.SepiksPrimeTrophy>());
            }
            if (!DestinyWorld.downedPrime) {
                DestinyWorld.downedPrime = true;
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<Items.ExoticCipher>());
                if (Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage) {
            Dust.NewDust(npc.position, npc.width, npc.height, DustID.PurpleCrystalShard, hitDirection);
        }

        public void HandlePacket(BinaryReader reader) {
            SepiksBossMessageType type = (SepiksBossMessageType)reader.ReadByte();
            switch (type) {
                case SepiksBossMessageType.DontTakeDamage:
                    npc.dontTakeDamage = reader.ReadBoolean();
                    break;
                default:
                    TheDestinyMod.Logger.Error($"Sepiks Prime Packet Handler: Encountered unknown packet of type {type}");
                    break;
            }
        }
        
        private ModPacket GetPacket(SepiksBossMessageType type) {
            ModPacket packet = mod.GetPacket();
			packet.Write((byte)DestinyModMessageType.SepiksPrime);
			packet.Write(npc.whoAmI);
			packet.Write((byte)type);
			return packet;
        }
    }

    internal enum SepiksBossMessageType : byte
    {
        DontTakeDamage
    }
}