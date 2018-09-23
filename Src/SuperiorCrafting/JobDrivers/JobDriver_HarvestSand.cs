using System;
using Verse;
using RimWorld;
using Verse.AI;
using System.Collections.Generic;

namespace SuperiorCrafting.JobDrivers
{
  public class JobDriver_HarvestSand : JobDriver_AffectFloor
  {
    protected override int BaseWorkAmount
    {
      get
      {
        return 50;
      }
    }

    protected override DesignationDef DesDef
    {
      get
      {
        return SCDefOf.HarvestSand;
      }
    }

    protected override StatDef SpeedStat
    {
      get
      {
        return StatDefOf.MiningSpeed;
      }
    }

    protected override void DoEffect(IntVec3 c)
    {
    	GenSpawn.Spawn(ThingDef.Named("Sand"),c,pawn.Map).stackCount=20;
      	FilthMaker.RemoveAllFilth(c, this.Map);
    }
  }
}

