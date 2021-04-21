using System;
using System.Collections.Generic;
using System.Text;

namespace TasqR
{
    public enum TasqProcess : byte
    {
        Start = 0,
        BeforeRun = 1,
        Initialize = 2,
        SelectionCriteria = 3,
        Run = 4,
        AfterRun = 5
    }
}
