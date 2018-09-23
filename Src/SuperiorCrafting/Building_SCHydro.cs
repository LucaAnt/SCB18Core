using System;
using RimWorld;
using UnityEngine;
using Verse;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace SuperiorCrafting
{

	public class Building_SCHydro : Building_PlantGrower
	{
		public IEnumerable<IntVec3> GrowableCells
    {
      get
      {
        return GenRadial.RadialCellsAround(this.Position,1.0f, true);
      }
    }
	
	
	private void MakeMatchingGrowZone()
    {
		GrowableCells
      //Building_SunLamp.\u003CMakeMatchingGrowZone\u003Ec__AnonStorey1 zoneCAnonStorey1 = new Building_SunLamp.\u003CMakeMatchingGrowZone\u003Ec__AnonStorey1();
      

      //zoneCAnonStorey1.designator = DesignatorUtility.FindAllowedDesignator<Designator_ZoneAdd_Growing>();

      //zoneCAnonStorey1.designator.DesignateMultiCell(this.GrowableCells.Where<IntVec3>(new Func<IntVec3, bool>(zoneCAnonStorey1.\u003C\u003Em__0)));
    }
	}
}
