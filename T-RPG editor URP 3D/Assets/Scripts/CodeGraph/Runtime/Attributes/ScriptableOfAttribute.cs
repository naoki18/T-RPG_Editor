using System;

public class ScriptableOfAttribute : Attribute
{
    private Type type;

    public Type Type => type;

    public ScriptableOfAttribute(Type _type)
    {
        type = _type;
    }
}
