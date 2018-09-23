using RimWorld;
using System;
using System.Text;
using System.Linq;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SuperiorCrafting.Buildings
{

	public class Building_MultiTerraformingPump : Building
	{

		public TerrainDef TerrainSelected;
		public float terraformingRadius;
		private CompProperties_Terraforming Terracompp;
		
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if(!respawningAfterLoad)
			{
				TerrainSelected = DefDatabase<TerrainDef>.GetNamed("Soil");
				UpdateRadiusAndDays(TerrainSelected,1.0f);
				resetProgress();
			}
		}
		public void UpdateRadiusAndDays(TerrainDef T,float rad)
		{
			TerrainSelected = T;
			terraformingRadius = rad;
			Terracompp =(CompProperties_Terraforming) this.GetComp<CompTerraforming>().props;
			Terracompp.radius = this.terraformingRadius;
			Terracompp.daysToRadius =  this.terraformingRadius * 1.5f;
			
		}
		
		public void resetProgress()
		{
			GetComp<CompTerraforming>().reset();
		}
		
		public override void ExposeData()
		{
			base.ExposeData();
			 try
	      {
			 	Scribe_Defs.Look<TerrainDef>(ref TerrainSelected, "TerrainSelected");
			 	Scribe_Values.Look<float>(ref terraformingRadius, "terraformingRadius", 1, true);
	      }
	      catch (Exception ex)
	      {
	        Log.Error("Error TerrainSelected on building terraformingpump:" + (ex.Message + Environment.NewLine + ex.StackTrace));
	        //TerrainSelected = DefDatabase<TerrainDef>.GetNamed("Soil");
	        //terraformingRadius=0;
	      }
		}
		public override IEnumerable<Gizmo> GetGizmos()
		{
			  this.def.specialDisplayRadius = this.terraformingRadius;
			  IList<Gizmo> SDGizmo = (IList<Gizmo>) new List<Gizmo>();
		      Command_Action commandAction1 = new Command_Action();
		      commandAction1.defaultDesc = "Self Destroy";
		      commandAction1.icon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel", true);
		      commandAction1.activateSound = SoundDef.Named("Click");
		      commandAction1.action =new Action(this.selfDestroy);
		      SDGizmo.Add((Gizmo) commandAction1);
		      
		      return base.GetGizmos().Concat(SDGizmo);
		}
		
		public void selfDestroy()
		{
			this.Destroy(DestroyMode.Vanish);
		}
	}
}
