using System;
using RimWorld;
using UnityEngine;
using Verse;


namespace SuperiorCrafting.Buildings
{

	public class Building_CampFire : Building_WorkTable
	{
		public bool hasFuel;
		 private Graphic cachedGraphicFull;
		 
		 		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			this.GetComp<CompRefuelable>().Refuel(this.GetComp<CompRefuelable>().TargetFuelLevel);
		}
		public override Graphic Graphic
    {
      get
      {
      	if (hasFuel)
      		return base.Graphic;
      	if (this.def.building.fullGraveGraphicData == null)
          return base.Graphic;
        if (this.cachedGraphicFull == null)
          this.cachedGraphicFull = this.def.building.fullGraveGraphicData.GraphicColoredFor((Thing) this);
       
        return this.cachedGraphicFull;
				
      }
    }

		
		public override void Tick()
		{
			base.Tick();
			CompRefuelable current_fuel=this.GetComp<CompRefuelable>();
			this.hasFuel = current_fuel.HasFuel;
			if(hasFuel)
				MoteMaker.ThrowDustPuff(this.Position,Map,1.0f);
		}
	}
}
