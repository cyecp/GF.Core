﻿using System;
using System.Collections.Generic;
using Eb;

public class EtSampleAutoPatcher : EntityDef
{
    //-------------------------------------------------------------------------
    public override void declareAllComponent(byte node_type)
    {
        declareComponent<DefSampleAutoPatcher>();
    }
}
