using System;
using Verse;
using RimWorld;
using Verse.AI;
using System.Collections.Generic;

namespace SuperiorCrafting.WorkGivers
{
  public class WorkGiver_MineHarvestSand : WorkGiver_ConstructAffectFloor
  {
    protected override DesignationDef DesDef
    {
      get
      {
        return SCDefOf.HarvestSand;
      }
    }

    public override Job JobOnCell(Pawn pawn, IntVec3 c)
    {
      return new Job(DefDatabase<JobDef>.GetNamed("JobDriver_HarvestSand", true), (LocalTargetInfo) c);
    }
    public override bool HasJobOnCell(Pawn pawn, IntVec3 c)
    {
      return !c.IsForbidden(pawn) && pawn.Map.designationManager.DesignationAt(c,SCDefOf.HarvestSand) != null && pawn.CanReserve((LocalTargetInfo) c, 1, -1, ReservationLayerDefOf.Floor, false);
    }
    public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
    {
    	IEnumerable<Designation> l = pawn.Map.designationManager.SpawnedDesignationsOfDef(SCDefOf.HarvestSand);
    	List<IntVec3> p_cells = new List<IntVec3>();
    	foreach(Designation d in l)
    	{p_cells.Add(d.target.Cell);}
    	return p_cells;
    }
  }
}
