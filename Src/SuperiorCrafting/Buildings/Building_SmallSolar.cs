using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace SuperiorCrafting
{
	[StaticConstructorOnStartup]
public class Building_SmallSolar : Building
{
  private static readonly Material BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(127,255,0));
  private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(245,255,250));
  public const float FullSunPower = 500f;
  public const float NightPower = 0.0f;

  public override void Tick()
  {
    if (Find.VisibleMap.roofGrid.Roofed(((Thing) this).Position))
      ((CompPowerTrader) this.PowerComp).PowerOutput = 0.0f;
    else
    	((CompPowerTrader) this.PowerComp).PowerOutput = Mathf.Lerp(0.0f, 2000f, Find.VisibleMap.skyManager.CurSkyGlow);
  }

  public override void Draw()
  {
    base.Draw();
    if (((CompPowerTrader) this.PowerComp).PowerOutput == 0f)
        return;
      GenDraw.DrawFillableBar(new GenDraw.FillableBarRequest()
      {
        //center = ((Thing) this).DrawPos + Vector3.left * 0.36f,
        center = ((Thing) this).DrawPos,
        size = new Vector2(0.20f, 0.10f),
        fillPercent = ((CompPowerTrader) this.PowerComp).PowerOutput / 500f,
        filledMat = Building_SmallSolar.BarFilledMat,
        unfilledMat = Building_SmallSolar.BarUnfilledMat,
        margin = 0.08f,
        rotation = Rot4.West
      });
  }
  public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
      base.SpawnSetup(map, respawningAfterLoad);
    }
	}
}
