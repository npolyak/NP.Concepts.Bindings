using NP.Concepts.Binding.GettersAndSetters;
using NP.Utilities;
using System;
using System.Collections.Generic;

namespace NP.Concepts.Binding.PathLinks
{
    public static class PathLinkHelpers
    {
        public static IObjWithPropGetter<PropertyType> 
            CreateGetter<PropertyType>(this IEnumerable<IBindingPathLink<object>> pathLinks)
            //where PropertyType : class
        {
            if (pathLinks.IsNullOrEmpty())
                return null;

            //if (pathLinks.Count() == 1)
            //    return pathLinks.Cast<IBindingPathLink<PropertyType>>().First().CreateGetter();

            return new CompositePathGetter<PropertyType>(pathLinks, default(PropertyType));
        }

        public static IObjWithPropSetter<PropertyType>
            CreateSetter<PropertyType>(this IEnumerable<IBindingPathLink<object>> pathLinks)
        {
            if (pathLinks.IsNullOrEmpty())
                return null;

            //if (pathLinks.Count() == 1)
            //    return pathLinks.Cast<IBindingPathLink<PropertyType>>().First().CreateSetter();

            return new CompositePathSetter<PropertyType>(pathLinks);
        }

        public static PlainPropBindingPathLink<object> ToPlainPathLink(this string pathLinkName, Type objType = null)
        {
            return new PlainPropBindingPathLink<object>(pathLinkName, objType);
        }
    }
}
