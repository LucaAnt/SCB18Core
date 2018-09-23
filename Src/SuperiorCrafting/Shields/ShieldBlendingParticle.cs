// Decompiled with JetBrains decompiler
// Type: Enhanced_Defence.Shields.ShieldBlendingParticle
// Assembly: Enhanced_Defence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8A381E51-C852-41B2-B251-E7AC4F04E532
// Assembly location: C:\Users\pesky\Desktop\SCA11Core\Assemblies\Enhanced_Defence.dll

using System;
using UnityEngine;
using Verse;

namespace Enhanced_Defence.Shields
{
[StaticConstructorOnStartup]
  internal class ShieldBlendingParticle
  {
    private static readonly Material ShieldSparksMat = MaterialPool.MatFrom("Things/ShieldSparks", (bool) ((UnityEngine.Object) MatBases.LightOverlay));
    private float currentAngle = UnityEngine.Random.Range(0.0f, 360f);
    private int transitionDirection = 1;
    private int transitionStep = UnityEngine.Random.Range(1, 1);
    public const int transitionMax = 80;
    private int transitionStatus;
    private Vector3 drawPosition;

    public int currentStep
    {
      get
      {
        return this.transitionStatus;
      }
    }

    public int currentDir
    {
      get
      {
        return this.transitionDirection;
      }
    }

    public ShieldBlendingParticle(Vector3 pos)
    {
      this.drawPosition = pos;
    }

    public ShieldBlendingParticle(Vector3 pos, int step)
    {
      this.drawPosition = pos;
      this.transitionStatus = Math.Max(Math.Min(80, step), 0);
    }

    public void DrawMe()
    {
      this.DrawMe(this.drawPosition);
    }

    public void DrawMe(Vector3 location)
    {
      this.doTransitionStep();
      Matrix4x4 matrix = new Matrix4x4();
      matrix.SetTRS(location + Altitudes.AltIncVect, Quaternion.Euler(0.0f, this.currentAngle, 0.0f), Vector3.one);
      Graphics.DrawMesh(MeshPool.plane20, matrix, FadedMaterialPool.FadedVersionOf(ShieldBlendingParticle.ShieldSparksMat, (float) (0.200000002980232 + (double) this.transitionStatus / 80.0 * 0.699999988079071)), 0);
    }

    private void doTransitionStep()
    {
      this.transitionStatus += this.transitionStep * this.transitionDirection;
      if (this.transitionStatus >= 80)
      {
        this.transitionDirection = -1;
        this.transitionStatus = 80;
      }
      else
      {
        if (this.transitionStatus > 0)
          return;
        this.currentAngle = UnityEngine.Random.Range(0.0f, 360f);
        this.transitionDirection = 1;
        this.transitionStatus = 0;
      }
    }
  }
}
