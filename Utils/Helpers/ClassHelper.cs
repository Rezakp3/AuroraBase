using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Utils.Helpers;

public static class ClassHelper
{
    public static T GetAttribute<TType, T>()
        where T : Attribute 
        => typeof(TType).GetCustomAttribute<T>();
}