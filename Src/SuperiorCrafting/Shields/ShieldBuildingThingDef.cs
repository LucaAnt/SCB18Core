// Decompiled with JetBrains decompiler
// Type: Enhanced_Defence.Shields.ShieldBuildingThingDef
// Assembly: Enhanced_Defence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8A381E51-C852-41B2-B251-E7AC4F04E532
// Assembly location: C:\Users\pesky\Desktop\SCA11Core\Assemblies\Enhanced_Defence.dll

using System.Collections.Generic;
using Verse;

namespace Enhanced_Defence.Shields
{
  public class ShieldBuildingThingDef : ThingDef
  {
    public int shieldMaxShieldStrength;
    public int shieldInitialShieldStrength;
    public int shieldShieldRadius;
    public int shieldPowerRequiredCharging;
    public int shieldPowerRequiredSustaining;
    public int shieldRechargeTickDelay;
    public int shieldRecoverWarmup;
    public bool shieldBlockIndirect;
    public bool shieldBlockDirect;
    public bool shieldFireSupression;
    public bool shieldInterceptDropPod;
    public bool shieldStructuralIntegrityMode;
    public float colourRed;
    public float colourGreen;
    public float colourBlue;
    public List<string> SIFBuildings;
  }
}
