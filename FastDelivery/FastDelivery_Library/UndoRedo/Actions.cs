﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library.UndoRedo
{
    interface Actions
    {
        object Do();
        object Undo();
    }
}
