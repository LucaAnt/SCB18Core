using System;
using Verse;
using Harmony;
using System.Reflection;
using RimWorld;
using System.Collections.Generic;

namespace SuperiorCrafting.Harmony
{

	internal static class Technology_Level_Upgrade
	{
		public static void UpgradeLevel()
		{
			Faction player_faction = null;
			foreach(Faction fa in Find.FactionManager.AllFactions)
			{
				if(fa.IsPlayer)
				{
					player_faction = fa;
					break;
				}
			}
				
			if ((player_faction.def.techLevel<TechLevel.Spacer)&&
				(DefDatabase<ResearchProjectDef>.GetNamed("MoisturePump",true).IsFinished) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("PowerIV",true).IsFinished) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("SecurityIV",true).IsFinished) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("ConstructionIV",true).IsFinished) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("CraftingIV",true).IsFinished) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("MedicineIV",true).IsFinished))
				{
					player_faction.def.techLevel = TechLevel.Spacer;
					Messages.Message("Colony technology level is now Spacer",MessageTypeDefOf.PositiveEvent);
					//Log.Message(player_faction.def.techLevel.ToString());
					return;
			}else if ((player_faction.def.techLevel<TechLevel.Industrial) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("Devilstrand",true).IsFinished) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("SolarPanels",true).IsFinished) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("Mortars",true).IsFinished) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("Autodoors",true).IsFinished) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("ElectricSmelting",true).IsFinished) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("DrugProduction",true).IsFinished))
				{
					player_faction.def.techLevel = TechLevel.Industrial;
					Messages.Message("Colony technology level is now Industrial",MessageTypeDefOf.PositiveEvent);
					//Log.Message(player_faction.def.techLevel.ToString());
					return;
			}else if ((player_faction.def.techLevel<TechLevel.Medieval) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("AgricultureI",true).IsFinished) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("Electricity",true).IsFinished) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("SecurityI",true).IsFinished) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("Stonecutting",true).IsFinished) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("Smithing",true).IsFinished) &&
			    (DefDatabase<ResearchProjectDef>.GetNamed("HospitalBed",true).IsFinished))
				{
					player_faction.def.techLevel = TechLevel.Medieval;
					Messages.Message("Colony technology level is now Medieval",MessageTypeDefOf.PositiveEvent);
					//Log.Message(player_faction.def.techLevel.ToString());
					return;
			}
				
		}
	}
}
