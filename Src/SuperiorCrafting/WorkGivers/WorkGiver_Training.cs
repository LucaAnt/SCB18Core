using RimWorld;
using System;
using System.Text;
using System.Linq;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace SuperiorCrafting
{

	public class WorkGiver_Training : WorkGiver_Scanner
	{
	
	public override PathEndMode PathEndMode
    {
      get
      {
        return PathEndMode.InteractionCell;
      }
    }
	
	public override bool Prioritized
    {
      get
      {
        return true;
      }
    } 
 
    public override bool ShouldSkip(Pawn pawn)
    {
    	if (pawn.skills.GetSkill(SkillDefOf.Medicine).levelInt < 20)
    		return false;
    	return true;
    }

    public override bool HasJobOnThing(Pawn p, Thing t, bool forced = false)
    {
    	if (p.mindState.IsIdle && !p.Dead && (!p.Downed && p.Spawned) && (!p.Drafted &&  ReservationUtility.CanReserveAndReach(p, (LocalTargetInfo) ((Thing) t), PathEndMode.InteractionCell, Danger.None, 1)))
    	{
    		Building_Trainable Y=(Building_Trainable) t;
    		if((t.def.defName.Equals("ShootingRange") || t.def.defName.Equals("CPRdummy") || t.def.defName.Equals("Holodeck") || t.def.defName.Equals("PunchingBag")) && Y.MyAllowList.Contains(p))
	    		{return true;}
    		
    	}
    	
    	return false;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
    	Building dummy = t as Building;
      return new Job(DefDatabase<JobDef>.GetNamed("JobDriver_Training_Dummy", true), (LocalTargetInfo) (dummy), (LocalTargetInfo)dummy.InteractionCell);
    }

    
    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
    	IEnumerable<Thing> L;
    	
    	L = (IEnumerable<Thing>) pawn.Map.listerBuildings.AllBuildingsColonistOfDef(DefDatabase<ThingDef>.GetNamed("ShootingRange", true)).ToArray();
    	if(L.Any())
    		foreach(Thing X in L)
    		{
    			Building_Trainable Y=(Building_Trainable)X;
    			if (Y.MyAllowList.Contains(pawn)) return L;
    		}
    		
    	
    	L = (IEnumerable<Thing>) pawn.Map.listerBuildings.AllBuildingsColonistOfDef(DefDatabase<ThingDef>.GetNamed("CPRdummy", true)).ToArray();
    	if(L.Any())
    		foreach(Thing X in L)
    		{
    			Building_Trainable Y=(Building_Trainable)X;
    			if (Y.MyAllowList.Contains(pawn)) return L;
    		}
    	
    	L = (IEnumerable<Thing>) pawn.Map.listerBuildings.AllBuildingsColonistOfDef(DefDatabase<ThingDef>.GetNamed("Holodeck", true)).ToArray();
    	if(L.Any())
    		foreach(Thing X in L)
    		{
    			Building_Trainable Y=(Building_Trainable)X;
    			if (Y.MyAllowList.Contains(pawn)) return L;
    		}
    	
    	L = (IEnumerable<Thing>) pawn.Map.listerBuildings.AllBuildingsColonistOfDef(DefDatabase<ThingDef>.GetNamed("PunchingBag", true)).ToArray();
    	if(L.Any())
    		foreach(Thing X in L)
    		{
    			Building_Trainable Y=(Building_Trainable)X;
    			if (Y.MyAllowList.Contains(pawn)) return L;
    		}
    	return L;
    }
	}
	
	
	/// <summary>
	/// //////////////////////////////////////////JOB_DRIVER
	/// </summary>
	public class JobDriver_Training : JobDriver
  {
		const float XPgain=5.00f;

    public override bool TryMakePreToilReservations()
    {

    	return this.pawn.Reserve((LocalTargetInfo) ((Thing) this.TargetA), this.job, 1, -1, (ReservationLayerDef) null);
    }
  
    protected override IEnumerable<Toil> MakeNewToils()
    {
   	//FAIL CONDITIONS
      ToilFailConditions.FailOnDestroyedOrNull<JobDriver_Training>(this,TargetIndex.A);
      ToilFailConditions.FailOnDespawnedNullOrForbidden<JobDriver_Training>(this, TargetIndex.A);
      this.FailOn( ()=>
		{
                  	Pawn p = this.pawn;
                  	if(this.IsContinuation(this.job) && this.CurToilIndex>1)
                  	if((double) p.needs.food.CurLevel < 0.25)
                  	{Messages.Message(p.LabelCap+" has stopped training due hunger.",(GlobalTargetInfo) p,MessageTypeDefOf.NeutralEvent);}else if ((double) p.needs.rest.CurLevel < 0.25 ){Messages.Message(p.LabelCap+" has stopped training due lack of sleep.",(GlobalTargetInfo) p,MessageTypeDefOf.NeutralEvent);}		
                  	return ((double) p.needs.food.CurLevel < 0.25 || (double) p.needs.rest.CurLevel < 0.25 || p.Drafted || p.jobs.jobQueue.Any());
		});
      //SEQUENZA AZIONI PAWN
      yield return Toils_Reserve.Reserve(TargetIndex.A, 1);
      yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
      Toil Train = Toils_Training(TargetIndex.B);
      yield return Train;
      yield return Toils_General.Wait(100);
      yield return Toils_Jump.Jump(Train);
    }
    
    private Toil Toils_Training(TargetIndex targetIndex)
    {
      Toil toil = new Toil();
      toil.initAction = (Action) (() =>
      {
        toil.actor.pather.StopDead();
        Building_Trainable X =(Building_Trainable) this.TargetA;
        
        switch(X.def.defName)
			{	
				case "ShootingRange":
      					this.pawn.skills.Learn(DefDatabase<SkillDef>.GetNamed(X.TrainingType,true),XPgain);
      					
      					IntVec3 shooter = TargetA.Cell +IntVec3Utility.RotatedBy(new Vector3(0.0f, 0.0f, 3f).ToIntVec3(), X.Rotation);
      					Projectile projectile = (Projectile) ThingMaker.MakeThing(ThingDef.Named("TrainingBullet"), (ThingDef) null);
      					GenSpawn.Spawn((Thing) projectile, pawn.Position,Map);
      					projectile.Launch((Thing) pawn, pawn.DrawPos, shooter, (Thing) null);
      					
      					SoundDef Sound_shoot = SoundDef.Named("ShotRevolver");
      					Sound_shoot.PlayOneShot(SoundInfo.InMap(X,MaintenanceType.None));
						break;
						
				case "CPRdummy":
						this.pawn.skills.Learn(DefDatabase<SkillDef>.GetNamed(X.TrainingType,true),XPgain);
						
						MoteMaker.ThrowMetaIcon(this.TargetA.Cell,this.Map,ThingDef.Named("Mote_HealingCross"));
						break;
						
					case "Holodeck":
						this.pawn.skills.Learn(DefDatabase<SkillDef>.GetNamed(X.TrainingType,true),XPgain);
 
						IntVec3 bubble_pos=new IntVec3(TargetA.Cell.x+1,TargetA.Cell.y,TargetA.Cell.z);
						
						MoteMaker.MakeStaticMote(bubble_pos	,Map,DefDatabase<ThingDef>.GetNamed("Mote_Speech"),1f);
						break;
						
					case "PunchingBag":
						this.pawn.skills.Learn(DefDatabase<SkillDef>.GetNamed(X.TrainingType,true),XPgain);
						
						MoteMaker.ThrowDustPuff(TargetA.Cell,Map,1.0f);
						
						SoundDef Sound_PBAG = SoundDef.Named("Pawn_Melee_Punch_HitPawn");
						Sound_PBAG.PlayOneShot(SoundInfo.InMap(X,MaintenanceType.None));
					break;		
						
				default:return;
				
			}
        
      });
      toil.defaultCompleteMode = ToilCompleteMode.Instant;
      return toil;
    }
   }
}
