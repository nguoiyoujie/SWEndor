using Primrose.Primitives.Extensions;
using System;

namespace SWEndor.Game.Scenarios.Scripting
{
  public class InsufficientParametersException : Exception
  {
    public InsufficientParametersException(string subject, int expected, int num)
      : base("Insufficient parameters for action '{0}': required {1}, has {2}".F(subject, expected.ToString(), num.ToString()))
    { }
  }
}
