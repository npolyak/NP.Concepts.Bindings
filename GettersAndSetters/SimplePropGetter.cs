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


namespace NP.Concepts.Binding.GettersAndSetters
{
    public class SimplePropGetter<PropertyType> : GenericSimplePropWithDefaultGetter<PropertyType>
    {
        protected override void OnObjUnset()
        {
        }

        protected override void OnObjSet()
        {
        }

        public override PropertyType GetPropValue()
        {
            return (PropertyType) this.TheObj;
        }
    }
}
