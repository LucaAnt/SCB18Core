using System;
using System.Linq;
using Verse;


namespace SuperiorCrafting
{

	public class CompProperties_SC_Comp_ImmuneSystemBoost_Light : CompProperties
	{
		public CompProperties_SC_Comp_ImmuneSystemBoost_Light()
    	{
					this.compClass = typeof (CompImmuneBoostLight);
    	}
	}
	
	public class CompProperties_SC_Comp_ImmuneSystemBoost_Medium : CompProperties
	{
		public CompProperties_SC_Comp_ImmuneSystemBoost_Medium()
    	{
					this.compClass = typeof (CompImmuneBoostMedium);
    	}
	}
	
	public class CompImmuneBoostLight : ThingComp
  {

    public override void PostIngested(Pawn ingester)
    {	
    	ingester.health.AddHediff(HediffMaker.MakeHediff(HediffDef.Named("LightImmuneBoost"), ingester, (BodyPartRecord) null), (BodyPartRecord) null, new DamageInfo?());
    }
  }
	public class CompImmuneBoostMedium : ThingComp
  {

    public override void PostIngested(Pawn ingester)
    {	
    	ingester.health.AddHediff(HediffMaker.MakeHediff(HediffDef.Named("MediumImmuneBoost"), ingester, (BodyPartRecord) null), (BodyPartRecord) null, new DamageInfo?());
    }
  }
}
