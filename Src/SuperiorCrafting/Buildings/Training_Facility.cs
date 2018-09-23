using RimWorld;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Verse;


namespace SuperiorCrafting
{
[StaticConstructorOnStartup]
  public class Building_Trainable : Building
  {
    public List<Pawn> MyAllowList = new List<Pawn>();
    public String TrainingType;
    public SkillDef TrainingSkillDef;
    public int counter;
	
    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
      base.SpawnSetup(map,respawningAfterLoad);
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
}

