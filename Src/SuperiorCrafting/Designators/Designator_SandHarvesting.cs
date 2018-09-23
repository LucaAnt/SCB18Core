using RimWorld;
using UnityEngine;
using Verse;


namespace SuperiorCrafting.Designators
{

	public class Designator_SandHarvesting : Designator
  {
		
    
    public Designator_SandHarvesting()
    {
      
      this.defaultLabel = "Harvest Sand";
      this.defaultDesc = "Harvest sand from the desert ground biome";
      this.icon = ContentFinder<Texture2D>.Get("Designators/HS", true);
      this.useMouseIcon = true;
      this.soundDragSustain = SoundDefOf.DesignateDragStandard;
      this.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
      this.soundSucceeded = SoundDefOf.DesignateSmoothFloor;
    }

    public override int DraggableDimensions
    {
      get
      {
        return 2;
      }
    }

    public override bool DragDrawMeasurements
    {
      get
      {
        return true;
      }
    }

    public override AcceptanceReport CanDesignateCell(IntVec3 c)
    {
      if (!c.InBounds(this.Map) || c.Fogged(this.Map))
        return (AcceptanceReport) false;
      if (this.Map.designationManager.DesignationAt(c, SCDefOf.HarvestSand) != null)
        return (AcceptanceReport) false;
      Building edifice = c.GetEdifice(this.Map);
      if (edifice != null && edifice.def.Fillage == FillCategory.Full && edifice.def.passability == Traversability.Impassable)
        return (AcceptanceReport) false;
      if (!c.GetTerrain(Find.VisibleMap).defName.Equals(TerrainDefOf.Sand.defName))
        return (AcceptanceReport) "Must target sand ground";
      return AcceptanceReport.WasAccepted;
    }

    public override void DesignateSingleCell(IntVec3 c)
    {
      if (DebugSettings.godMode)
        this.Map.terrainGrid.RemoveTopLayer(c, true);
      else
        this.Map.designationManager.AddDesignation(new Designation((LocalTargetInfo) c, SCDefOf.HarvestSand));
    }

    public override void SelectedUpdate()
    {
      GenUI.RenderMouseoverBracket();
    }
  }
 }


