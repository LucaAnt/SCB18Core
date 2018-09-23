using RimWorld;
using System;
using System.Text;
using System.Linq;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using SuperiorCrafting.Buildings;


namespace SuperiorCrafting
{
	[StaticConstructorOnStartup]
	public class CompProperties_Terraforming : CompProperties_TerrainPump
  {

    public CompProperties_Terraforming()
    {
      this.compClass = typeof (CompTerraforming);
    }
  }
	
public class CompTerraforming : CompTerrainPump
  {
	
    protected override void AffectCell(IntVec3 c)
    {
      AffectCell(this.parent.Map, c);
    }

    public void AffectCell(Map map, IntVec3 c)
    {
    	map.terrainGrid.SetTerrain(c,(parent as Building_MultiTerraformingPump).TerrainSelected);
    }
    
    public void reset()
    {
    	this.PostDeSpawn(this.parent.Map);
    }

  }
}
