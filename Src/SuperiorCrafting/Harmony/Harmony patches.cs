using System;
using Verse;
using Harmony;
using System.Reflection;
using RimWorld;
using System.Collections.Generic;

namespace SuperiorCrafting.Harmony
{

	[StaticConstructorOnStartup]
	class Main
	{
			
		static Main()
		{
			var harmony = HarmonyInstance.Create("com.github.harmony.rimworld.mod.SuperiorCrafting");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			
			
		}
	}


//	[HarmonyPatch(typeof(Zone_Growing))]
//	public static class Patch_Zone_Growing_Default
//	{
//
//		static MethodInfo TargetMethod()
//		{
//			
//			Log.Message("ENTRA");
//			//return AccessTools.Method(typeof(Zone_Growing), ".ctor", new Type[] { typeof(ZoneManager)},  null);
//			return AccessTools.Method(typeof(Zone_Growing), "GetInspectStringr", new Type[] {},  null);
//		}  
//		
//		
//		static void Prefix(Zone_Growing __instance)
//		{
//			Log.Message("hooked");
//			//__instance.SetPlantDefToGrow(ThingDefOf.PlantGrass);
//		
//		}
//		static void Postfix()
//		{
//			Log.Message("hooked");
//			//__instance.SetPlantDefToGrow(ThingDefOf.PlantGrass);
//		
//		}
//	}
	
//	[HarmonyPatch(typeof(Zone_Growing))]
//	[HarmonyPatch(".ctor")]
//	[HarmonyPatch(new Type[] { typeof(ZoneManager) })]
//	[HarmonyPatch(typeof(Zone_Growing), ".ctor", new Type[] { typeof(ZoneManager)})]
//	internal static class Patch_Zone_Growing_Default
//	{
//		   
//		static void Postfix(Zone_Growing __instance,ZoneManager z)
//		{
//			Log.Message("hooked");
//			//__instance.SetPlantDefToGrow(ThingDefOf.PlantGrass);
//		
//		}
//	}
//	
	//Got to hook here because harmony dosen't hook any constructors :(
	[HarmonyPatch(typeof(Zone_Growing))]
	[HarmonyPatch("NextZoneColor", PropertyMethod.Getter)]
	internal class Patch_Zone_Growing
	{
		static void Postfix(Zone_Growing __instance)
		{
			//Log.Message("hooked");
			__instance.SetPlantDefToGrow(ThingDefOf.PlantGrass);
		}
	}
	
	[HarmonyPatch(typeof(ResearchManager), "ResearchPerformed", new Type[] { typeof(float),typeof(Pawn)})]
	internal class Patch_Research_Manager
	{
		static void Postfix(ResearchManager __instance)
		{
			Technology_Level_Upgrade.UpgradeLevel();
		}
	}	
	
}