﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Types
{
    public delegate object MemberGetter(object instance);
    public delegate T MemberGetter<T>(object instance);
}
