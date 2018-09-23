using System;
using RimWorld;
using UnityEngine;
using Verse;
using SuperiorCrafting.Buildings;

namespace SuperiorCrafting.Comps
{

	[StaticConstructorOnStartup]
	public class SC_Comp_FireOverlay : CompFireOverlay
	{
		public override void PostDraw()
	    {
			bool fueled= ((Building_CampFire) this.parent).hasFuel;
			if(fueled)
	      		base.PostDraw();
			else
				return;
	    }
	}
	
	public class SC_CompProperties_FireOverlay : CompProperties_FireOverlay
  {
    public float fireSize = 1f;
    public Vector3 offset;

    public SC_CompProperties_FireOverlay()
    {
      this.compClass = typeof (SC_Comp_FireOverlay);
    }
  }
}
