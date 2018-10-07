// (c) Nick Polyak 2013 - http://awebpros.com/
// License: Code Project Open License (CPOL) 1.92(http://www.codeproject.com/info/cpol10.aspx)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author(s) of this software if something goes wrong. 
// 
// Also as a courtesy, please, mention this software in any documentation for the 
// products that use it.

namespace NP.Concepts.Binding.PathLinks
{
    public enum PropertyKind
    {
        Unknown,
        Plain, // plain C# property (in order to detect property change it has to be notifiable)
        Map, // a property that is retrieved from or set on a dictionary object by using string indexer (theObj[propName])
        AProperty, // AProperty
        Attached // WPF attached property
    }

    public static class PropertyKindExtensions
    {
        public static bool IsNameBased(this PropertyKind propKind)
        {
            switch(propKind)
            {
                case PropertyKind.Map:
                case PropertyKind.Plain:
                    return true;
                default:
                    return false;
            }
        }
    }
}
