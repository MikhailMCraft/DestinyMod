using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.UI;

namespace DestinyMod.Core.UI
{
	public class UIImplementer : ModSystem
	{
		public override void Load()
		{
			foreach (Type type in DestinyMod.Instance.Code.GetTypes().Where(t => t.IsSubclassOf(typeof(DestinyModUIState)) 
			&& !t.IsAbstract 
			&& t.GetConstructor(Type.EmptyTypes) != null))
			{
				DestinyModUIState uiState = Activator.CreateInstance(type) as DestinyModUIState;
				string uiStateName = type.Name;
				uiState.PreLoad(ref uiStateName);
				uiState.Name = uiStateName;
				uiState.DefaultSetUpInterface();
				uiState.UIHandler = uiState.Load();

				if (uiState.AutoAddHandler)
				{
					UIHandler.ProcessedUIs.Add(uiState.UIHandler);
				}
			}
		}

		public override void Unload() => UIHandler.Unload();

		public override void UpdateUI(GameTime gameTime) => UIHandler.HandleUpdate(gameTime);

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) => UIHandler.HandleModifyInterfaceLayers(layers);
	}
}