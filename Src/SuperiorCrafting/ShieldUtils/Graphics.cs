// Decompiled with JetBrains decompiler
// Type: Enhanced_Defence.ShieldUtils.Graphics
// Assembly: Enhanced_Defence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8A381E51-C852-41B2-B251-E7AC4F04E532
// Assembly location: C:\Users\pesky\Desktop\SCA11Core\Assemblies\Enhanced_Defence.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Enhanced_Defence.ShieldUtils
{
  public static class Graphics
  {
    private static Mesh CircleMesh_cache;

    public static Mesh CircleMesh
    {
      get
      {
        if ((UnityEngine.Object) Graphics.CircleMesh_cache != (UnityEngine.Object) null)
          return Graphics.CircleMesh_cache;
        double num1 = 1.0;
        List<Vector2> vector2List = new List<Vector2>();
        vector2List.Add(new Vector2(0.0f, 0.0f));
        int num2 = 0;
        while (num2 <= 360)
        {
          double num3 = (double) num2 / 180.0 * Math.PI;
          vector2List.Add(new Vector2(0.0f, 0.0f)
          {
            x = (float) (num1 * Math.Cos(num3)),
            y = (float) (num1 * Math.Sin(num3))
          });
          num2 += 4;
        }
        Vector3[] vector3Array = new Vector3[vector2List.Count];
        for (int index = 0; index < vector3Array.Length; ++index)
          vector3Array[index] = new Vector3(vector2List[index].x, 0.0f, vector2List[index].y);
        int[] numArray = new Triangulator(vector2List.ToArray()).Triangulate();
        Graphics.CircleMesh_cache = new Mesh();
        Graphics.CircleMesh_cache.vertices = vector3Array;
        Graphics.CircleMesh_cache.uv = new Vector2[vector2List.Count];
        Graphics.CircleMesh_cache.triangles = numArray;
        Graphics.CircleMesh_cache.RecalculateNormals();
        Graphics.CircleMesh_cache.RecalculateBounds();
        return Graphics.CircleMesh_cache;
      }
    }
  }
}
