using RimWorld;
using System;
using System.Text;
using System.Linq;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SuperiorCrafting.ITabs
{

	public class ITab_Training_Dummy : ITab
  {
	private Vector2 ScrollPosition;
    public ITab_Training_Dummy()
    {
      this.labelKey = "TabTraining";
      this.tutorTag = "Training";
      this.size = new Vector2(820f, 700f);
    }

    public override bool IsVisible
    {
      get
      {
      	return true;
      }
    }


    protected override void FillTab()
    {
    	Building_Trainable currentBuilding  =(Building_Trainable) this.SelThing;
    	List<Pawn> MyAllowList = currentBuilding.MyAllowList;
    	List<Pawn> pawnList = new List<Pawn>(currentBuilding.Map.mapPawns.FreeColonists.Where<Pawn>((Func<Pawn, bool>) (p =>
      {
        if (p != null && !p.Destroyed && !p.Downed)
          return p.Spawned;
        return false;
      })));
      if (pawnList == null || pawnList.Count == 0)
      {
       CloseTab();
      }
      else
      {
      	Rect MainContainer = new Rect(new Vector2(0.0f,0.0f),size);
      	
        Verse.Text.Font = GameFont.Medium;
        String labelString = currentBuilding.LabelCap + " Colonist Assigment";
        
        Vector2 labelDimensions = Text.CalcSize(labelString);
        Rect LabelContainer = new Rect(0.0f, 0.0f,labelDimensions.x, labelDimensions.y);
        LabelContainer  =  LabelContainer.CenteredOnXIn(MainContainer);
        Widgets.Label(LabelContainer,labelString);
        
        
        
        float nextPawnEntryYPosition = LabelContainer.height;
        float ScrollviewHeight  = (float) (pawnList.Count * 35.0 + 100.0);
        Rect ScrollExternalContainer = MainContainer.ContractedBy(20);
        ScrollExternalContainer.y = nextPawnEntryYPosition;
        ScrollExternalContainer.height = ScrollExternalContainer.height - nextPawnEntryYPosition;
        
        GUI.BeginGroup(new Rect(5.0f,nextPawnEntryYPosition,MainContainer.width,MainContainer.height - nextPawnEntryYPosition));
        Rect ScrollContainer = new Rect(0.0f, 0.0f, ScrollExternalContainer.width , ScrollviewHeight); 
        Widgets.BeginScrollView(ScrollExternalContainer, ref ScrollPosition, ScrollContainer);
        int counter = 0;
        foreach(Pawn pawn in pawnList)
        {
       
        	if(counter%2!=0){
         	 Rect Row = new Rect(5.0f, nextPawnEntryYPosition, ScrollContainer.width, 32f);
         	 Widgets.DrawBoxSolid(Row,SimpleColor.Red.ToUnityColor());}
        	counter++;
        	
          Rect PawnEntryContainer = new Rect(5.0f, nextPawnEntryYPosition, ScrollContainer.width * 0.6f, 32f);
          Widgets.Label(PawnEntryContainer, pawn.LabelCap +"    "+pawn.skills.GetSkill(currentBuilding.TrainingSkillDef));
          PawnEntryContainer.x = PawnEntryContainer.xMax;
          PawnEntryContainer.width = ScrollContainer.width * 0.4f;
          if (!MyAllowList.Contains(pawn))
          {
            if (!Widgets.ButtonText(PawnEntryContainer, "Begin Training", true, false, true))
              nextPawnEntryYPosition += 35f;
            else if (MyAllowList.Count >= 5)
            {
              Messages.Message("Cant assign more than " + (int) MyAllowList.Count + " colonists",(GlobalTargetInfo) ((Thing) currentBuilding), MessageTypeDefOf.NegativeEvent);
 
            }
            else
            {
              MyAllowList.Add(pawn);
            }
          }
          else if (!Widgets.ButtonText(PawnEntryContainer, "Stop Training", true, false, true))
          {
            nextPawnEntryYPosition += 35f;
          }
          else
          {
            MyAllowList.Remove(pawn);

          }
        }
        Widgets.EndScrollView();
        GUI.EndGroup();
   	 	}
	}
    
	}
}
