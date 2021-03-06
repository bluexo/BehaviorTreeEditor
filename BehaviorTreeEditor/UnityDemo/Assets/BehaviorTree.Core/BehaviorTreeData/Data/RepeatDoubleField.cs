﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BTData
{
    public partial class RepeatDoubleField : BaseField
    {
        public List<double> Value;

        public override void Read(ref Reader reader)
        {
            reader.Read(ref FieldName).Read(ref Value);
        }

        public override void Write(ref Writer writer)
        {
            writer.Write(FieldName).Write(Value);
        }
    }
}
