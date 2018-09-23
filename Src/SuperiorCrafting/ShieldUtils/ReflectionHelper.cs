// Decompiled with JetBrains decompiler
// Type: Enhanced_Defence.ShieldUtils.ReflectionHelper
// Assembly: Enhanced_Defence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8A381E51-C852-41B2-B251-E7AC4F04E532
// Assembly location: C:\Users\pesky\Desktop\SCA11Core\Assemblies\Enhanced_Defence.dll

using System;
using System.Reflection;

namespace Enhanced_Defence.ShieldUtils
{
  internal class ReflectionHelper
  {
    internal static object GetInstanceField(Type type, object instance, string fieldName)
    {
      BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
      return type.GetField(fieldName, bindingAttr).GetValue(instance);
    }
  }
}
