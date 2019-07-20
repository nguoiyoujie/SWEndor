namespace SWEndor.Primitives
{
  public delegate void DoFunc<T>(ref T data);
  public delegate void DoFunc<T1, T2>(ref T1 d1, ref T2 d2);
  public delegate void DoFunc<T1, T2, T3>(ref T1 d1, ref T2 d2, ref T3 d3);
  public delegate void DoFunc<T1, T2, T3, T4>(ref T1 d1, ref T2 d2, ref T3 d3, ref T4 d4);
  public delegate V GetFunc<T, out V>(ref T data);
  public delegate V GetFunc<T1, T2, out V>(ref T1 data, ref T2 param);
  public delegate void SetFunc<T, in V>(ref T data, V value);
}
