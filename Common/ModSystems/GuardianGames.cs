using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Net;
using System.IO;
using System;
using DestinyMod.Common.ModPlayers;
using Terraria.ModLoader.IO;
using DestinyMod.Content.Items.Misc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DestinyMod.Common.ModSystems
{
	public class GuardianGames : ModSystem
	{
		public static bool Active;

		public static int DepositCooldown;

		public static int WinningTeam;

		public static bool ClaimedItem;

		public static bool GameError;

		public static bool TryDeposit(Player player)
		{
			if (DepositCooldown > 0)
			{
				return false;
			}

			if (!DestinyClientConfig.Instance.GuardianGamesConfig)
            {
				Main.NewText("You must opt-in to the Guardian Games from the config menu.", Color.Red);
				return true;
            }

			try
			{
				int laurelCount = 0;
				int laurelType = ModContent.ItemType<Laurel>();
				foreach (Item item in player.inventory)
				{
					if (item.type == laurelType)
					{
						laurelCount += item.stack;
					}

					item.SetDefaults();
				}

				ClassPlayer classPlayer = player.GetModPlayer<ClassPlayer>();
				HttpClient client = new HttpClient();
				client.BaseAddress = new Uri("https://DestinyModServer.mikhailmcraft.repl.co");
				HttpRequestMessage request = new HttpRequestMessage();
				request.Method = HttpMethod.Post;
				request.Headers.Add("VERIFY-MOD", "a7rg53F435h4Ff2fhjWa33gH6j54ag2G");
				request.Headers.Add("CLASS", classPlayer.ClassType.ToString().ToUpper());
				request.Headers.Add("DUPLICATE-CHECK", Steamworks.SteamUser.GetSteamID().ToString());
				HttpResponseMessage response = client.Send(request);
				/*
				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";
				request.ContentLength = data.Length;*/

				string postData = Uri.EscapeDataString(laurelCount.ToString());
				byte[] data = System.Text.Encoding.ASCII.GetBytes(postData);

				using (var stream = request.Content.ReadAsStream())
				{
					stream.Write(data, 0, data.Length);
				}
				string responseString = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();

				if (responseString == "TOOMUCH")
				{
					Main.NewText("You've already deposited 30 Laurels today! Try again tomorrow.", new Color(255, 0, 0));
					player.QuickSpawnItem(laurelType, laurelCount);
				}
				else
				{
					string depositedLaurelsString = responseString.Remove(responseString.IndexOf("RECEIVED"));
					if (string.IsNullOrEmpty(depositedLaurelsString))
					{
						int depositedLaurels = int.Parse(depositedLaurelsString);
						player.QuickSpawnItem(laurelType, laurelCount - depositedLaurels);
						Main.NewText("Deposited " + depositedLaurels + " Laurels!", new Color(255, 255, 0));
						player.QuickSpawnItem(ItemID.GoldCoin, laurelCount - depositedLaurels);
					}
					else
					{
						Main.NewText("Deposited " + laurelCount + "Laurels!", new Color(255, 255, 0));
						player.QuickSpawnItem(ItemID.GoldCoin, laurelCount);
					}

					int firedRocket = 0;
					switch (classPlayer.ClassType)
					{
						case DestinyClassType.None:
							goto finalise;

						case DestinyClassType.Titan:
							firedRocket = ProjectileID.RocketFireworkRed;
							break;

						case DestinyClassType.Hunter:
							firedRocket = ProjectileID.RocketFireworkBlue;
							break;

						case DestinyClassType.Warlock:
							firedRocket = ProjectileID.RocketFireworkYellow;
							break;
					}

					Projectile.NewProjectile(player.GetProjectileSource_Misc(0), player.Center, new Vector2(0, -Main.rand.Next(5, 8)), firedRocket, 0, 0, player.whoAmI);
				}

			finalise:
				DepositCooldown = 6000;
				return true;
			}
			catch
			{
				return false;
			}
		}


		public override void SaveWorldData(TagCompound tag) => tag.Add("Cooldown", DepositCooldown);

		public override void LoadWorldData(TagCompound tag) => DepositCooldown = tag.Get<int>("Cooldown");
	}
}