using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
using Primrose.Primitives.ValueTypes;
using System;
using System.Text;

namespace SWEndor.FileFormat.INI
{
  [AttributeUsage(AttributeTargets.Field)]
  public class INIValueAttribute : Attribute
  {
    private static Registry<Type, Func<INIFile, string, string, object, object>> _getter = new Registry<Type, Func<INIFile, string, string, object, object>>();
    private static Registry<Type, Action<INIFile, string, string, object>> _setter = new Registry<Type, Action<INIFile, string, string, object>>();


    static INIValueAttribute()
    {
      _getter.Add(typeof(bool), (f, s, k, d) => f.GetBool(s, k, (bool)d));
      _getter.Add(typeof(bool[]), (f, s, k, d) => f.GetBoolArray(s, k, (bool[])d));
      _getter.Add(typeof(double), (f, s, k, d) => f.GetDouble(s, k, (double)d));
      _getter.Add(typeof(double[]), (f, s, k, d) => f.GetDoubleArray(s, k, (double[])d));
      _getter.Add(typeof(float), (f, s, k, d) => f.GetFloat(s, k, (float)d));
      _getter.Add(typeof(float2), (f, s, k, d) => f.GetFloat2(s, k, (float2)d));
      _getter.Add(typeof(float3), (f, s, k, d) => f.GetFloat3(s, k, (float3)d));
      _getter.Add(typeof(float4), (f, s, k, d) => f.GetFloat4(s, k, (float4)d));
      _getter.Add(typeof(float[]), (f, s, k, d) => f.GetFloatArray(s, k, (float[])d));
      _getter.Add(typeof(int), (f, s, k, d) => f.GetInt(s, k, (int)d));
      _getter.Add(typeof(int[]), (f, s, k, d) => f.GetIntArray(s, k, (int[])d));
      _getter.Add(typeof(string), (f, s, k, d) => f.GetString(s, k, (string)d));
      _getter.Add(typeof(string[]), (f, s, k, d) => f.GetStringArray(s, k, (string[])d));
      _getter.Add(typeof(StringBuilder[]), (f, s, k, d) => f.GetStringBuilderArray(s, k, (StringBuilder[])d));
      _getter.Add(typeof(uint), (f, s, k, d) => f.GetUInt(s, k, (uint)d));
      _getter.Add(typeof(uint[]), (f, s, k, d) => f.GetUIntArray(s, k, (uint[])d));

      _setter.Add(typeof(bool), (f, s, k, d) => f.SetBool(s, k, (bool)d));
      _setter.Add(typeof(bool[]), (f, s, k, d) => f.SetBoolArray(s, k, (bool[])d));
      _setter.Add(typeof(double), (f, s, k, d) => f.SetDouble(s, k, (double)d));
      _setter.Add(typeof(double[]), (f, s, k, d) => f.SetDoubleArray(s, k, (double[])d));
      _setter.Add(typeof(float), (f, s, k, d) => f.SetFloat(s, k, (float)d));
      _setter.Add(typeof(float2), (f, s, k, d) => f.SetFloat2(s, k, (float2)d));
      _setter.Add(typeof(float3), (f, s, k, d) => f.SetFloat3(s, k, (float3)d));
      _setter.Add(typeof(float4), (f, s, k, d) => f.SetFloat4(s, k, (float4)d));
      _setter.Add(typeof(float[]), (f, s, k, d) => f.SetFloatArray(s, k, (float[])d));
      _setter.Add(typeof(int), (f, s, k, d) => f.SetInt(s, k, (int)d));
      _setter.Add(typeof(int[]), (f, s, k, d) => f.SetIntArray(s, k, (int[])d));
      _setter.Add(typeof(string), (f, s, k, d) => f.SetString(s, k, (string)d));
      _setter.Add(typeof(string[]), (f, s, k, d) => f.SetStringArray(s, k, (string[])d));
      _setter.Add(typeof(StringBuilder[]), (f, s, k, d) => f.SetStringBuilderArray(s, k, (StringBuilder[])d));
      _setter.Add(typeof(uint), (f, s, k, d) => f.SetUInt(s, k, (uint)d));
      _setter.Add(typeof(uint[]), (f, s, k, d) => f.SetUIntArray(s, k, (uint[])d));
    }

    public string Section;
    public string Key;
    public INIValueAttribute(string section, string key)
    {
      Section = section;
      Key = key;
    }

    public object Read(Type t, INIFile f, object defaultValue)
    {
      if (_getter.Contains(t))
      {
        return _getter.Get(t).Invoke(f, Section, Key, defaultValue);
      }
      else
      {
        if (t.IsEnum)
          return f.GetEnum(t, Section, Key, defaultValue);
        else if (t.IsArray && t.GetElementType().IsEnum)
          return f.GetEnumArray(t, Section, Key, defaultValue);
      }
      throw new InvalidOperationException("Attempted to parse an INIKey value into an unsupported type '{0}'".F(t.Name));
    }

    public void Write(Type t, INIFile f, object value)
    {
      if (_setter.Contains(t))
      {
        _setter.Get(t).Invoke(f, Section, Key, value);
      }
      else
      {
        if (t.IsEnum)
          f.SetEnum(Section, Key, value);
        else if (t.IsArray && t.GetElementType().IsEnum)
          f.SetEnumArray(Section, Key, value);
      }
      throw new InvalidOperationException("Attempted to set an INIKey value as a value of an unsupported type '{0}'".F(t.Name));
    }
  }
}
