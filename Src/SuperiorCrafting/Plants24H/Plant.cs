// Decompiled with JetBrains decompiler
// Type: Enhanced_Defence.Plants24H.Plant
// Assembly: Enhanced_Defence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8A381E51-C852-41B2-B251-E7AC4F04E532
// Assembly location: C:\Users\pesky\Desktop\SCA11Core\Assemblies\Enhanced_Defence.dll

using RimWorld;
using UnityEngine;
using Verse;

namespace Enhanced_Defence.Plants24H
{
  public class Plant : RimWorld.Plant
  {
    private new int unlitTicks;

    private new bool Resting
    {
      get
      {
        return false;
      }
    }

    public override void TickLong()
    {
      this.CheckTemperatureMakeLeafless();
      if (!GenPlant.GrowthSeasonNow(this.Position,Find.VisibleMap))
        return;
      if (!this.HasEnoughLightToGrow)
        this.unlitTicks += 2000;
      else
        this.unlitTicks = 0;
      bool flag = this.LifeStage == PlantLifeStage.Mature;
      this.growth += this.GrowthPerTick * 2000f;
      if (!flag && this.LifeStage == PlantLifeStage.Mature)
        this.NewlyMatured();
      if (this.def.plant.LimitedLifespan)
      {
        this.age += 2000;
        if (this.get_Rotting())
        {
          int num = Mathf.CeilToInt(10f);
          this.TakeDamage(new DamageInfo(DamageDefOf.Rotting, num, (Thing) null, new BodyPartDamageInfo?(), (ThingDef) null));
        }
      }
      if (this.Destroyed || !this.def.plant.shootsSeeds || ((double) this.growth < 0.600000023841858 || !Rand.MTBEventOccurs(this.def.plant.seedEmitMTBDays, 30000f, 2000f)) || (!GenPlant.SnowAllowsPlanting(this.Position) || GridsUtility.Roofed(this.Position)))
        return;
      GenPlantReproduction.TrySpawnSeed(this.Position, this.def, SeedTargFindMode.MapGenCluster, (Thing) this);
    }

    public override void ExposeData()
    {
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      Scribe_Values.LookValue<int>((M0&) @this.unlitTicks, "unlitTicks", (M0) 0, false);
      base.ExposeData();
    }

    private new void CheckTemperatureMakeLeafless()
    {
      float num = 8f;
      if ((double) GridsUtility.GetTemperature(this.Position) >= (double) this.HashOffset() * 0.00999999977648258 % (double) num - (double) num - 2.0)
        return;
      this.MakeLeafless();
    }

    private new bool HasEnoughLightToGrow
    {
      get
      {
        return (double) this.GrowthRateFactor_Light > 0.001;
      }
    }

    private new float GrowthPerTick
    {
      get
      {
        if (this.LifeStage != PlantLifeStage.Growing || this.Resting)
          return 0.0f;
        return (float) (1.0 / (30000.0 * (double) this.def.plant.growDays)) * this.GrowthRate;
      }
    }

    private void NewlyMatured()
    {
      if (!this.CurrentlyCultivated())
        return;
      Find.VisibleMap.mapDrawer().MapMeshDirty(this.Position, MapMeshFlag.Things);
    }

    private new bool CurrentlyCultivated()
    {
      if (!this.def.plant.Sowable)
        return false;
      Zone zone = Find.get_ZoneManager().ZoneAt(this.Position);
      if (zone != null && zone is Zone_Growing)
        return true;
      Building edifice = GridsUtility.GetEdifice(this.Position);
      if (edifice != null)
        return edifice.def.building.SupportsPlants;
      return false;
    }
  }
}
