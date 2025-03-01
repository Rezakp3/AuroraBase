using System;
using System.ComponentModel;

namespace Core.Enums
{
    public enum EChannel : short
    {
        [Description("کیف پول")]
        Wallet = 0,

        [Description("باشگاه")]
        Club = 1,
        
        [Description("پذیرنده")]
        Merchant= 2,
    }
}
