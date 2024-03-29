﻿using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITEMS_SignPlacement.ITEM.Elements
{
    internal class SignatureLine
    {
        public string Name {  get; private set; }
        public string Surname { get; set; }
        public XYZ Offset { get; set; }
        public string SignatureFamilySymbolName { get; set; }
        public SignatureLine(string name)
        {
            Name = name;
        }
    }
}
