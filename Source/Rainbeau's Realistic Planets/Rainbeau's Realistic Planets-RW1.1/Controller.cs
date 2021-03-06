﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Planets_Code
{
	public class Controller : Mod {
		public static Dictionary<Faction, int> factionCenters = new Dictionary<Faction, int>();
		public static Dictionary<Faction, int> failureCount = new Dictionary<Faction, int>();
		public static double maxFactionSprawl = 0;
		public static double minFactionSeparation = 0;
		public static Settings Settings;
		public static MethodInfo FactionControlSettingsMI = null;

		public override string SettingsCategory() { return "Planets.ModName".Translate(); }
		public override void DoSettingsWindowContents(Rect canvas) { Settings.DoWindowContents(canvas); }

		public Controller(ModContentPack content) : base(content) {
			const bool Debug = false;
			const string Id = "net.rainbeau.rimworld.mod.realisticplanets";

			Log.Warning($"Realistic Planets - Fan Update will be using Harmony as an external dependency when it next updates. To avoid wiping your mod list, please download and use Harmony now.");

			if (Debug)
			{
				Log.Warning($"{Id} is in debug mode. Contact the mod author if you see this.");
				Harmony.DEBUG = Debug;
			}

			var harmony = new Harmony(Id);
			harmony.PatchAll( Assembly.GetExecutingAssembly() );
			Settings = GetSettings<Settings>();
			LongEventHandler.QueueLongEvent(new Action(Init), "LibraryStartup", false, null);
		}

		private void Init()
		{
			// Get FactionControl's settings button for use on the Create World page
			foreach (ModContentPack pack in LoadedModManager.RunningMods)
			{
				if (pack.PackageId == "factioncontrol.kv.rw")
				{
					foreach (Assembly assembly in pack.assemblies.loadedAssemblies)
					{
						var dialog = assembly.GetType("FactionControl.Patch_Page_CreateWorldParams_DoWindowContents");
						if (dialog != null)
						{
							FactionControlSettingsMI = dialog.GetMethod("OpenSettingsWindow", BindingFlags.Public | BindingFlags.Static);
							break;
						}
					}
					if (FactionControlSettingsMI == null)
						Log.Warning("unable to find SettingsDialogWindow in FactionControl mod");
					break;
				}
			}
		}
	}
}
