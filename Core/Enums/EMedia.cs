using System;
using System.ComponentModel;

namespace Core.Enums
{
    public enum EMedia : short
    {
        [Description("اندروید")]
        Android = 0,

        [Description("وب")]
        Web = 1,
        
        [Description("انگولار")]
        Angular= 2,
    }
}
