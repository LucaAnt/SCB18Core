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
  public class Building_Trainable : Building
  {
    public List<Pawn> MyAllowList = new List<Pawn>();
    public String TrainingType;
    public SkillDef TrainingSkillDef;
    private static Texture2D Ui_Change;
    private Pawn PawnSelected;
    public int counter;
	
    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
      base.SpawnSetup(map,respawningAfterLoad);
      Building_Trainable.Ui_Change = ContentFinder<Texture2D>.Get("UI/UI_Change Button", true);
      switch(this.def.defName)
			{	
				case "ShootingRange":
		      			TrainingType = "Shooting";
		      			TrainingSkillDef =SkillDefOf.Shooting;
						break;
				case "CPRdummy":
						TrainingType = "Medicine";
						TrainingSkillDef =SkillDefOf.Medicine;
						break;
					case "Holodeck":
						TrainingType = "Social";
						TrainingSkillDef =SkillDefOf.Social;
						break;
					case "PunchingBag":
						TrainingType = "Melee";
						TrainingSkillDef =SkillDefOf.Melee;
						break;					
				default:return;
			}
      
  
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
      commandAction1.icon = Building_Trainable.Ui_Change;
      commandAction1.hotKey = KeyBindingDefOf.Misc3;
      commandAction1.activateSound = SoundDef.Named("Click");
      commandAction1.action =new Action(this.StartDesignation);
      commandAction1.groupKey = 88776001;
      Command_Action commandAction2 = commandAction1;
      gizmoList.Add((Gizmo) commandAction2);
      IEnumerable<Gizmo> gizmos = base.GetGizmos();
      return gizmos == null ? source.AsEnumerable<Gizmo>() : source.AsEnumerable<Gizmo>().Concat<Gizmo>(gizmos);
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

 

  
  internal class DialogTrainingUserMenuCPR : MainTabWindow
  {
    private const float EntryHeight = 35f;
    private Building_Trainable building;
    private Vector2 scrollPosition;

    public DialogTrainingUserMenuCPR(Building_Trainable building)
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
            }
          }
          else if (!Widgets.ButtonText(rect3, "Training UnAssign Colonist", true, false, true))
          {
            y += 35f;
          }
          else
          {
            this.building.MyAllowList.Remove(pawn);

          }
        }
        Widgets.EndScrollView();
      }
    }
  }

  

  }

