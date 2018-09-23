using RimWorld;
using System;
using System.Linq;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using System.Reflection;

//<researchMods>
//            <li Class="ResearchableStatUpgrades.ResearchMod_Repeatable">
//				<def>ProductivityRepeatable</def> //PARAMETRO 1 con riferimento def
//			</li>
// </researchMods>
//	
namespace SuperiorCrafting
{

	public class SC_ReasearchUpgrades:ResearchMod
	{
			
		private String ResearchIndex = string.Empty;
		
		public override void Apply()
		{
			
			switch(ResearchIndex)
			{	
				case "CostructionI":
					CostructionI();
					break;
				case "CostructionII":
					CostructionII();
					break;
				case "CostructionIII":
					CostructionIII();
					break;
				case "CostructionIV":
					CostructionIV();
					break;
				case "PowerII":
					PowerII();
					break;
				case "PowerIII":
					PowerIII();
					break;
				case "PowerIV":
					PowerIV();
					break;
				case "NutrientResynthesisII":
					NutrientResynthesisII();
					break;
				case "ProteinReplication":
					ProteinReplication();
					break;	
				default:return;
			}
		}

		 public static void patch()
	    {
	      Messages.Message("References patched!",(GlobalTargetInfo) Find.VisibleMap.mapPawns.FreeColonistsSpawned.First(),MessageTypeDefOf.TaskCompletion);
	      Log.Message("References patched!");
	    }
		 
		 public static void ConstructionNotifyRepair()
		 {
		 		      
	      foreach (Map M in Find.Maps)
	      {
	      	
	      	foreach(Building B in M.listerBuildings.AllBuildingsColonistOfDef(DefDatabase<ThingDef>.GetNamed("SCWall", true)))
	      		M.listerBuildingsRepairable.Notify_BuildingTookDamage(B);
	      	foreach(Building B in M.listerBuildings.AllBuildingsColonistOfDef(DefDatabase<ThingDef>.GetNamed("WallLighted", true)))
	      		M.listerBuildingsRepairable.Notify_BuildingTookDamage(B);
	      	foreach(Building B in M.listerBuildings.AllBuildingsColonistOfDef(DefDatabase<ThingDef>.GetNamed("WallConduit", true)))
	      		M.listerBuildingsRepairable.Notify_BuildingTookDamage(B);
	      	
	      }
		 }
		 public static void CostructionI()
	    {
	      DefDatabase<ThingDef>.GetNamed("SCWall", true).stuffCategories.Add(StuffCategoryDefOf.Stony);
	      DefDatabase<ThingDef>.GetNamed("WallLighted", true).stuffCategories.Add(StuffCategoryDefOf.Stony);
	      DefDatabase<ThingDef>.GetNamed("WallConduit", true).stuffCategories.Add(StuffCategoryDefOf.Stony);
	      DefDatabase<ThingDef>.GetNamed("SCDoor", true).stuffCategories.Add(StuffCategoryDefOf.Stony);
	    } 
		 
		public static void CostructionII()
	    {
	      DefDatabase<ThingDef>.GetNamed("SCWall", true).stuffCategories.Add(StuffCategoryDefOf.Metallic);
	      DefDatabase<ThingDef>.GetNamed("WallLighted", true).stuffCategories.Add(StuffCategoryDefOf.Metallic);
	      DefDatabase<ThingDef>.GetNamed("WallConduit", true).stuffCategories.Add(StuffCategoryDefOf.Metallic);
	      DefDatabase<ThingDef>.GetNamed("SCDoor", true).stuffCategories.Add(StuffCategoryDefOf.Metallic);
	      
	      DefDatabase<ThingDef>.GetNamed("SCWall", true).SetStatBaseValue(StatDef.Named("MaxHitPoints"), 400f);
	      DefDatabase<ThingDef>.GetNamed("WallConduit", true).SetStatBaseValue(StatDef.Named("MaxHitPoints"), 400f);
	      DefDatabase<ThingDef>.GetNamed("WallLighted", true).SetStatBaseValue(StatDef.Named("MaxHitPoints"), 400f);
	      ConstructionNotifyRepair();

	    } 
		public static void CostructionIII()
	    {
	      DefDatabase<ThingDef>.GetNamed("SCWall", true).SetStatBaseValue(StatDef.Named("MaxHitPoints"), 450f);
	      DefDatabase<ThingDef>.GetNamed("WallConduit", true).SetStatBaseValue(StatDef.Named("MaxHitPoints"), 450f);
	      DefDatabase<ThingDef>.GetNamed("WallLighted", true).SetStatBaseValue(StatDef.Named("MaxHitPoints"), 450f);
	      ConstructionNotifyRepair();
	    }
		public static void CostructionIV()
	    {
	      DefDatabase<ThingDef>.GetNamed("SCWall", true).SetStatBaseValue(StatDef.Named("MaxHitPoints"), 500f);
	      DefDatabase<ThingDef>.GetNamed("WallConduit", true).SetStatBaseValue(StatDef.Named("MaxHitPoints"), 500f);
	      DefDatabase<ThingDef>.GetNamed("WallLighted", true).SetStatBaseValue(StatDef.Named("MaxHitPoints"), 500f);
	      ConstructionNotifyRepair();
	    }
	
	public static void PowerII()
    {
		CompProperties_Battery new_cap = DefDatabase<ThingDef>.GetNamed("Battery", true).GetCompProperties<CompProperties_Battery>();

		new_cap.storedEnergyMax = 1150f;
		new_cap.efficiency = 0.75f;
    }

    public static void PowerIII()
    {
		CompProperties_Battery new_cap = DefDatabase<ThingDef>.GetNamed("Battery", true).GetCompProperties<CompProperties_Battery>();
		new_cap.storedEnergyMax = 1300f;
		new_cap.efficiency = 1.00f;
    }

    public static void PowerIV()
    {
		CompProperties_Battery new_cap = DefDatabase<ThingDef>.GetNamed("Battery", true).GetCompProperties<CompProperties_Battery>();
		new_cap.storedEnergyMax = 1500f;
		new_cap.efficiency = 1.25f;
    }

    public static void NutrientResynthesisII()
    {
    	DefDatabase<ThingDef>.GetNamed("NutrientPasteDispenser").building.nutritionCostPerDispense = 0.15f;
    }

    public static void ProteinReplication()
    {
		DefDatabase<ThingDef>.GetNamed("NutrientPasteDispenser").building.nutritionCostPerDispense = 0.1f;
    }
    
	}
}
