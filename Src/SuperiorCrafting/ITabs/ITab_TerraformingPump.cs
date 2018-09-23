using RimWorld;
using System;
using System.Text;
using System.Linq;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using SuperiorCrafting.Buildings;

namespace SuperiorCrafting.ITabs
{

	public class ITab_TerraformingPump : ITab
	{

		private Vector2 ScrollPosition;
		static List<TerrainDef> TerrainTypes;
		
	    public ITab_TerraformingPump()
	    {
	    
	      this.labelKey = "Options";
	      this.tutorTag = "TabTerraforming";
	      this.size = new Vector2(400f, 400f);
	      
	      TerrainTypes =new List<TerrainDef>();
	      TerrainTypes.Add(DefDatabase<TerrainDef>.GetNamed("Soil"));
	      TerrainTypes.Add(DefDatabase<TerrainDef>.GetNamed("SoilRich"));
	      TerrainTypes.Add(DefDatabase<TerrainDef>.GetNamed("MarshyTerrain"));
	      TerrainTypes.Add(DefDatabase<TerrainDef>.GetNamed("Sand"));
	      TerrainTypes.Add(DefDatabase<TerrainDef>.GetNamed("Mud"));
	      TerrainTypes.Add(DefDatabase<TerrainDef>.GetNamed("Marsh"));
	      TerrainTypes.Add(DefDatabase<TerrainDef>.GetNamed("Ice"));
	      TerrainTypes.Add(DefDatabase<TerrainDef>.GetNamed("WaterShallow"));
	      TerrainTypes.Add(DefDatabase<TerrainDef>.GetNamed("WaterDeep"));

	      
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
	    	Verse.Text.Font = GameFont.Small;
	    	Building_MultiTerraformingPump currentTerraformingPump =(Building_MultiTerraformingPump) this.SelThing;
	    	
	    	//LEFT PANE
	        Rect TerrainTypesPane = new Rect(0.0f,0.0f,size.x/2,size.y);
	        Rect TerrainTypesLabel = new Rect(0.0f,0.0f,size.x/2,Text.LineHeight);
	        TerrainTypesLabel = TerrainTypesLabel.CenteredOnXIn(TerrainTypesPane);
	        Widgets.Label(TerrainTypesLabel,"Terrain type selection:");
	        float NextTerrainentrybasey= Text.LineHeight * 2;
	        Rect TerrainTypesPaneScrollview = new Rect(0.0f,NextTerrainentrybasey,size.x/2,Text.LineHeight * TerrainTypes.Count);
	        GUI.BeginGroup(TerrainTypesPane);
	        Widgets.BeginScrollView(TerrainTypesPane,ref ScrollPosition,TerrainTypesPaneScrollview);
	        foreach(TerrainDef T in TerrainTypes)
	        {
	        	NextTerrainentrybasey+=Text.LineHeight;
	        	Rect terrainEntryRect = new Rect(0.0f,NextTerrainentrybasey,TerrainTypesPaneScrollview.width,Text.LineHeight);
	        	Widgets.DrawHighlightIfMouseover(terrainEntryRect);
	        	
	        	if(Widgets.RadioButton(terrainEntryRect.position,false))
	        	{
	        		Widgets.RadioButton(terrainEntryRect.position,true);
	        		currentTerraformingPump.UpdateRadiusAndDays(T,currentTerraformingPump.terraformingRadius);
	        		currentTerraformingPump.resetProgress();
	        		Messages.Message("Terraforming progress has been reset",MessageTypeDefOf.CautionInput);
	        	}else if( currentTerraformingPump.TerrainSelected.defName.Equals(T.defName)){
	        		Widgets.RadioButton(terrainEntryRect.position,true);
	        	}
	        	
	        	Rect EntryTexture =new Rect(terrainEntryRect.position,new Vector2(Widgets.RadioButtonSize,Widgets.RadioButtonSize));
	        	EntryTexture.x+=Widgets.RadioButtonSize+3;
	        	Widgets.DrawTextureFitted(EntryTexture,T.uiIcon,1.0f);
	        		
	        	Rect EntryLabel =terrainEntryRect.CenteredOnXIn(terrainEntryRect);
	        	EntryLabel.x+=Widgets.RadioButtonSize+EntryTexture.x+3;
	        	String entryString = T.defName.ToString();
	        	EntryLabel.width =Text.CalcSize(entryString).x;
	        	Widgets.Label(EntryLabel,entryString);
	        	
	        	
	        }
	        Widgets.EndScrollView();
	        GUI.EndGroup();
	        
	        //RIGHT PANE
	        Rect RightSliderPane = new Rect(size.x/2,0.0f,size.x/2,size.y).ContractedBy(10);

	        currentTerraformingPump.terraformingRadius = Widgets.HorizontalSlider(RightSliderPane,currentTerraformingPump.terraformingRadius,1.0f,50.0f,true,"Terraforming radius :"+((int)currentTerraformingPump.terraformingRadius).ToString());
	        
	        currentTerraformingPump.UpdateRadiusAndDays(currentTerraformingPump.TerrainSelected,currentTerraformingPump.terraformingRadius);
	        
		}
	}
	
}
