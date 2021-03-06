﻿// (c) Nick Polyak 2013 - http://awebpros.com/
// License: Code Project Open License (CPOL) 1.92(http://www.codeproject.com/info/cpol10.aspx)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author(s) of this software if something goes wrong. 
// 
// Also as a courtesy, please, mention this software in any documentation for the 
// products that use it.

using NP.Utilities;

namespace NP.Concepts.Binding.PathLinks
{
    public class PathLinkInfo
    {
        public string LinkPath { get; set; }

        public PropertyKind ThePropertyKind { get; set; }

        public override bool Equals(object obj)
        {
            PathLinkInfo target = obj as PathLinkInfo;

            if (target == null)
                return false;

            return this.LinkPath.ObjEquals(target.LinkPath) &&
                this.ThePropertyKind == target.ThePropertyKind;
        }

        public override int GetHashCode()
        {
            return
                this.LinkPath.GetHashCodeExtension() ^ 
                this.ThePropertyKind.GetHashCode();
        }
    }
}
