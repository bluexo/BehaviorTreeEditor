﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorTreeEditor
{
    public enum FieldType
    {
        None,
        IntField,
        LongField,
        FloatField,
        DoubleField,
        ColorField,
        Vector2,
        Vector3,
        EnumField,
        BooleanField,
        RepeatIntField,//int数组
        RepeatLongField,//Long数组
        RepeatFloatField,//浮点数组
        RepeatVector2Field,//Vector2数组
        RepeatVector3Field,//Vector3数组
    }
}
