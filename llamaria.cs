using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace llamaria
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class llamaria : Mod
	{

		public class AskLlamaCommand : ModCommand
		{
			// Define trigger
			public override string Command => "ask";
			public override string Description => "Sends a message to interact with Ollama service (must have a locally running service first)";

			// Type of command (Chat or Console)
			public override CommandType Type => CommandType.Chat;

			// Happens when the command is triggered
			public override void Action(CommandCaller caller, string input, string[] args)
            {
                string message = string.Join(" ", args);
                Main.NewText(message, Color.LightBlue);
				string items = GetItemsString();
                Task.Run(async () =>
                {
                    string response = await OllamaApiHandler.GenerateResponse("llama3.1", items + "User: " + message);
                    if (!string.IsNullOrEmpty(response))
                    {
                        Main.NewTextMultiline(response, false, Color.LightGreen);
                    }
                    else
                    {
                        Main.NewText("No response received from the API.", new Color(255, 255, 255));
                    }
                });
            }
			private string GetItemsString()
			{
				StringBuilder itemsBuilder = new StringBuilder();
				itemsBuilder.AppendLine("Current inventory: \n");
				foreach (var player in Main.ActivePlayers)
				{
					foreach (var item in player.inventory)
					{
						if (string.IsNullOrEmpty(item.Name))
						{
							continue;
						}
						string itemName = item.Name;
						int quantity = item.stack;
						itemsBuilder.AppendLine($"{quantity} {itemName}{(quantity > 1 ? "s" : "")}");
					}
				}
				return itemsBuilder.ToString();
			}
		}
	}
}