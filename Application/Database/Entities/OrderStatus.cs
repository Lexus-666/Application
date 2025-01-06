using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace kursah_5semestr;

public enum OrderStatus
{
    [Description("new")]
    New = 0,

    [Description("paid")]
    Paid
}