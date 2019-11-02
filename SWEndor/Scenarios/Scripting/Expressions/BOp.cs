namespace SWEndor.Scenarios.Scripting.Expressions
{
  public enum BOp : byte
  {
    LOGICAL_OR, // a || b
    LOGICAL_AND, // a && b
    ADD, // a + b
    SUBTRACT, // a - b
    MULTIPLY, // a * b
    MODULUS, // a % b
    DIVIDE, // a / b
    EQUAL_TO, // a == b
    NOT_EQUAL_TO, // a != b
    MORE_THAN, // a > b
    MORE_THAN_OR_EQUAL_TO, // a >= b
    LESS_THAN, // a < b
    LESS_THAN_OR_EQUAL_TO, // a <= b
  }
}
