﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorTreeEditor
{
    public class ColorFieldDesigner : BaseFieldDesigner
    {
        [Category("常规"), DisplayName("R值"), Description("R值")]
        public int R { get; set; }

        [Category("常规"), DisplayName("G值"), Description("G值")]
        public int G { get; set; }

        [Category("常规"), DisplayName("B值"), Description("B值")]
        public int B { get; set; }

        public override string FieldContent()
        {
            return string.Format("{0}:{1},{2},{3}", FieldName, R, G, B);
        }

        public override string ToString()
        {
            return "color";
        }
    }
}