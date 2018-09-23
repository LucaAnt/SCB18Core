// Decompiled with JetBrains decompiler
// Type: SuperiorCrafting.Building_CPRdummy
// Assembly: SuperiorCrafting, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2C368F18-E1F7-43CC-A052-34F288B1502D
// Assembly location: C:\Users\pesky\Desktop\SCA11Core\Assemblies\SuperiorCrafting.dll

using RimWorld;
using System;
using System.Text;
using System.Linq;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SuperiorCrafting
{
	[StaticConstructorOnStartup]
	
 //////////////////////////////////////////////public interface IAssignableBuilding
  public class Building_CPRdummy : Building
  {
    public List<Pawn> MyAllowList = new List<Pawn>();
    private int ShotDelay = 60;
    private int fireDelay = 6;
    private string skillDefName = "Medicine";
    private int skillIncrease = 1;
    private static Texture2D Ui_Change;
    private IntVec3 ShotShield;
    private Job MyCustomJob;
    private Pawn PawnSelected;
    public int counter;
    private bool countint;
    private bool Busy=false;

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
      base.SpawnSetup(map,respawningAfterLoad);
      Building_CPRdummy.Ui_Change = ContentFinder<Texture2D>.Get("UI/UI_Change Button", true);
      if (this.MyCustomJob == null)
      	this.MyCustomJob = new Job(DefDatabase<JobDef>.GetNamed("GoToAndWaitForeverCPR", true), (LocalTargetInfo) ((Thing) this), (LocalTargetInfo) InteractionCell);
      this.ShotShield = this.Position + IntVec3Utility.RotatedBy(new Vector3(0.0f, 0.0f, 3f).ToIntVec3(), this.Rotation);	
      
    }

    	
    unsafe public override void ExposeData()
    {
      base.ExposeData();
      try
      {
      	Scribe_Collections.Look<Pawn>(ref this.MyAllowList,"MyAllowList",LookMode.Reference,new object[0]);
      	
      }
      catch (Exception ex)
      {
        Log.Error("Error MyAllowList:" + (ex.Message + Environment.NewLine + ex.StackTrace));
        this.MyAllowList = new List<Pawn>();
      }
      Scribe_References.Look<Pawn>(ref PawnSelected, "PawnSelected", false);
      
    }

    private void AllowedPawnScanner()
    {

      if(!MyAllowList.NullOrEmpty() && !Busy)
	      foreach(Pawn p in MyAllowList)
	      {
      		if (p.Dead || p.DestroyedOrNull())
	          this.MyAllowList.Remove(p);
	        else if (p.mindState.IsIdle && !p.Dead && (!p.Downed && p.Spawned) && (!p.Drafted &&  ReservationUtility.CanReserveAndReach(p, (LocalTargetInfo) ((Thing) this), PathEndMode.InteractionCell, Danger.None, 1)))
	        {
	        	
	        	Busy=true;
	        	PawnSelected = p;
	          	p.jobs.TryTakeOrderedJob(MyCustomJob,JobTag.MiscWork);
	          	Log.Message("DENTRO");
	          break;
	        }
	      }
	      return;
    }

    private bool TrainingSafety()
    {
      return (double) this.PawnSelected.needs.food.CurLevel > 0.25 && (double) this.PawnSelected.needs.rest.CurLevel > 0.25 && this.PawnSelected.CurJob == this.MyCustomJob && !PawnSelected.Drafted;
    }

    private void PersonalTrainer()
    {
      if (!(PawnSelected.Position == InteractionCell))
      {
	      if (this.counter <= 0 && this.countint)
	      	this.Reset();
	      --this.counter;
	      if (Math.IEEERemainder((double) this.counter, (double) this.fireDelay) == 1.0)
	      {
	        foreach (SkillRecord skill in this.PawnSelected.skills.skills)
	        {
	          if (skill.def.defName == this.skillDefName)
	            skill.Learn((float) this.skillIncrease);
	        }
	      }
	      if (Math.IEEERemainder((double) this.counter, (double) this.ShotDelay) != 1.0)
	        return;
	      
	      
      }
    }
    
    private void Train()
    {
    	if ((PawnSelected.Position == InteractionCell))
    	{
	        foreach (SkillRecord skill in this.PawnSelected.skills.skills)
	        {
	          if (skill.def.defName == this.skillDefName)
	            skill.Learn((float) this.skillIncrease);
	        }
	      MoteMaker.ThrowMetaIcon(Position,this.Map,ThingDef.Named("Mote_HealingCross"));
    	}
    }

    public override void Tick()
    {
      base.Tick();
      
//      if (this.PawnSelected != null)
//      {
//        if (this.TrainingSafety())
//          this.Train();
//       else
//     	Reset();
//
//      }
//      if(!Busy)
//      	AllowedPawnScanner();
    }
    
    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
    {
      List<FloatMenuOption> floatMenuOptionList = new List<FloatMenuOption>();
      if (!ReservationUtility.CanReserve(myPawn, (LocalTargetInfo) ((Thing) this), 1))
        return (IEnumerable<FloatMenuOption>) new List<FloatMenuOption>()
        {
          new FloatMenuOption("CannotUseReserved", (Action) null, MenuOptionPriority.VeryLow, (Action) null, (Thing) null)
        };
      if (!myPawn.CanReach((LocalTargetInfo) ((Thing) this),PathEndMode.Touch,Danger.None,false,TraverseMode.ByPawn))
        return (IEnumerable<FloatMenuOption>) new List<FloatMenuOption>()
        {
          new FloatMenuOption("CannotUseNoPath", (Action) null, MenuOptionPriority.VeryLow, (Action) null, (Thing) null)
        };
      if (!this.MyAllowList.Contains(myPawn))
        return (IEnumerable<FloatMenuOption>) new List<FloatMenuOption>()
        {
          new FloatMenuOption("Not on training list", (Action) null, MenuOptionPriority.VeryLow, (Action) null, (Thing) null)
        };
     
      Action action = (Action) (() =>
      {
        Busy=true;
        myPawn.jobs.StopAll();
        myPawn.jobs.TryTakeOrderedJob(MyCustomJob,JobTag.MiscWork);
        this.PawnSelected = myPawn;
        //myPawn.Reserve((LocalTargetInfo) ((Thing) this),this.MyCustomJob,1,-1,null);
        
      });
      floatMenuOptionList.Add(new FloatMenuOption("Go train medicine", action, MenuOptionPriority.VeryLow, (Action) null, (Thing) null));
      return (IEnumerable<FloatMenuOption>) floatMenuOptionList;
    }

    public void StartDesignation()
    {
      Find.UIRoot.windows.Add( new DialogTrainingUserMenuCPR(this));
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
      IList<Gizmo> source = (IList<Gizmo>) new List<Gizmo>();
      IList<Gizmo> gizmoList = source;
      Command_Action commandAction1 = new Command_Action();
      commandAction1.defaultDesc = "User List";
      commandAction1.icon = Building_CPRdummy.Ui_Change;
      commandAction1.hotKey = KeyBindingDefOf.Misc3;
      commandAction1.activateSound = SoundDef.Named("Click");
      commandAction1.action =new Action(this.StartDesignation);
      commandAction1.groupKey = 88776001;
      Command_Action commandAction2 = commandAction1;
      gizmoList.Add((Gizmo) commandAction2);
      IEnumerable<Gizmo> gizmos = base.GetGizmos();
      return gizmos == null ? source.AsEnumerable<Gizmo>() : source.AsEnumerable<Gizmo>().Concat<Gizmo>(gizmos);
    }

    public void Reset()
    {
      if (this.PawnSelected != null && this.PawnSelected.CurJob == this.MyCustomJob)
        this.PawnSelected.jobs.StopAll(false);
      this.PawnSelected = (Pawn) null;
      this.counter = 0;
      this.countint = false;
      Busy= false;
    }

    
    public override string GetInspectString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < this.MyAllowList.Count; ++index)
      {
        stringBuilder.AppendLine();
        stringBuilder.Append((index + 1).ToString() + ". " + this.MyAllowList.ElementAt<Pawn>(index).LabelCap);
      }
      return stringBuilder.ToString().Trim();
    }
  }

  
//    private Toil Toils_WaitWithSoundAndEffect(TargetIndex targetIndex)
//    {
//      Toil toil = new Toil();
//      toil.initAction = (Action)(toil.actor.pather.StopDead);
//      toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
//      MoteMaker.ThrowMetaIcon(this.TargetA.Cell,this.Map,ThingDef.Named("Mote_HealingCross"));
//      return toil;
//    }


  
  internal class DialogTrainingUserMenuCPR : MainTabWindow
  {
    private const float EntryHeight = 35f;
    private Building_CPRdummy building;
    private Vector2 scrollPosition;

    public DialogTrainingUserMenuCPR(Building_CPRdummy building)
    {
      this.building = building;
      layer = WindowLayer.GameUI;
      this.closeOnEscapeKey = true;
      this.doCloseButton = true;
      this.doCloseX = true;
      this.soundAppear = (SoundDef) null;
      this.soundClose = (SoundDef) null;
      this.preventCameraMotion = false;
      this.forcePause = true;
    }
    
    public override Vector2 RequestedTabSize
    {
      get
      {
        return new Vector2(820f, 700f);
      }
    }

    public override MainTabWindowAnchor Anchor
    {
      get
      {
        return MainTabWindowAnchor.Left;
      }
    }
    public override void DoWindowContents(Rect inRect)
    {
    	
      List<Pawn> pawnList = new List<Pawn>(Find.VisibleMap.mapPawns.FreeColonists.Where<Pawn>((Func<Pawn, bool>) (p =>
      {
        if (p != null && !p.Destroyed && !p.Downed)
          return p.Spawned;
        return false;
      })));
      if (pawnList == null || pawnList.Count == 0)
      {
        this.Close(true);
      }
      else
      {
        ((IEnumerable<GUIStyle>) Text.fontStyles).ElementAt<GUIStyle>(1);
        Rect rect1 = new Rect(inRect)
        {
          yMin = inRect.yMin + 50f,
          yMax = inRect.yMax - 45f
        };
        float y = 32f;
        Widgets.Label(new Rect(200f, 0.0f, inRect.width * 0.8f, 60f), this.building.LabelCap + " Colonist Assigment");
        Rect rect2 = new Rect(0.0f, y, inRect.width - 16f, (float) ((double) pawnList.Count * 35.0 + 100.0));
        Widgets.BeginScrollView(rect1, ref this.scrollPosition, rect2);
        for (int index = 0; index < pawnList.Count; ++index)
        {
          Pawn pawn = pawnList[index];
          Rect rect3 = new Rect(0.0f, y, rect2.width * 0.6f, 32f);
          Widgets.Label(rect3, pawn.LabelCap);
          rect3.x = rect3.xMax;
          rect3.width = rect2.width * 0.4f;
          if (!this.building.MyAllowList.Contains(pawn))
          {
            if (!Widgets.ButtonText(rect3, "Training Assign Colonist", true, false, true))
              y += 35f;
            else if (this.building.MyAllowList.Count >= 5)
            {
              Messages.Message("Cant assign more than" + (int) this.building.MyAllowList.Count + " colonists",(GlobalTargetInfo) ((Thing) building), MessageTypeDefOf.NegativeEvent);
 
            }
            else
            {
              this.building.MyAllowList.Add(pawn);
              this.building.Reset();
            }
          }
          else if (!Widgets.ButtonText(rect3, "Training UnAssign Colonist", true, false, true))
          {
            y += 35f;
          }
          else
          {
            this.building.MyAllowList.Remove(pawn);
            this.building.Reset();
          }
        }
        Widgets.EndScrollView();
      }
    }
  }
}
