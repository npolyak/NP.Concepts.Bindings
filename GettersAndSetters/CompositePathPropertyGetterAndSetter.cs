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

using NP.Concepts.Binding.PathLinks;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NP.Concepts.Binding.GettersAndSetters
{
    public class CompositePathGetter<PropertyType> :
        IObjWithPropGetter<PropertyType>
    {
        PropertyType _defaultValue;

        ValueHolder<PropertyType> _cachedValue = new ValueHolder<PropertyType>();

        public event Action PropertyChangedEvent;

        public void TriggerPropertyChanged()
        {
            if (PropertyChangedEvent == null)
                return;

            LastPropGetter.TriggerPropertyChanged();
        }

        IObjWithPropGetter<object> LastPropGetter
        {
            get
            {
                return PropGetters[PropGetters.Count - 1];
            }
        }

        IEnumerable<IBindingPathLink<object>> _pathLinks;

        public List<IObjWithPropGetter<object>> PropGetters { get; } =
            new List<IObjWithPropGetter<object>>();

        public CompositePathGetter
        (
            IEnumerable<IBindingPathLink<object>> pathLinks, 
            PropertyType defaultValue
        )
        {
            _pathLinks = pathLinks;

            _defaultValue = defaultValue;

            if (pathLinks.IsNullOrEmpty())
            {
                PropGetters.Add(new SimplePropGetter<object>());
                return;
            }

            IObjWithPropGetter<object> previousPropGetter = null;
            foreach (var pathLink in _pathLinks)
            {
                IObjWithPropGetter<object> propGetter = pathLink.CreateGetter();

                PropGetters.Add(propGetter);

                if (previousPropGetter != null)
                {
                    IObjWithPropGetter<object> thePreviousPropGetter = previousPropGetter;
                    previousPropGetter.PropertyChangedEvent += () =>
                        {
                            propGetter.TheObj = thePreviousPropGetter.GetPropValue();
                        };
                }

                previousPropGetter = propGetter;
            }

            previousPropGetter.PropertyChangedEvent += () =>
                {
                    if (this.PropertyChangedEvent == null)
                        return;

                    if (!LastPropGetter.HasObj)
                    {
                        _cachedValue.TheValue = _defaultValue;
                    }
                    else
                    {
                        _cachedValue.TheValue = (PropertyType) LastPropGetter.GetPropValue();
                    }

                    PropertyChangedEvent?.Invoke();
                };
        }

        void SetValueFromLastPropGetterValue()
        {
            if (!LastPropGetter.HasObj)
            {
                _cachedValue.TheValue = _defaultValue;
            }
            else
            {
                _cachedValue.TheValue = (PropertyType)LastPropGetter.GetPropValue();
            }
        }

        public PropertyType GetPropValue()
        {
            if (!_cachedValue.HasBeenSet)
            {
                SetValueFromLastPropGetterValue();
            }

            return _cachedValue.TheValue;
        }


        object _obj = null;
        public object TheObj
        {
            get
            {
                return _obj;
            }

            set
            {
                if (_obj == value)
                    return;

                if (HasObj)
                {
                    // disconnect old object
                }

                _obj = value;
                this.PropGetters[0].TheObj = _obj;

                if (HasObj)
                {
                    // connect new object
                }
            }
        }

        public bool HasObj
        {
            get
            {
                return _obj != null;
            }
        }
    }

    public class CompositePathSetter<PropertyType> : 
        IObjWithPropSetter<PropertyType>
    {
        public IObjWithPropSetter<object> TheSetter { get; } = null;
        public List<IObjWithPropGetter<object>> PropGetters { get; }

        public CompositePathSetter(IEnumerable<IBindingPathLink<object>> pathLinks)
        {
            IBindingPathLink<object> theSetterPathLink = pathLinks.Last();

            TheSetter = theSetterPathLink.CreateSetter();

            PropGetters =
                pathLinks
                    .TakeWhile((pathLink) => (!Object.ReferenceEquals(pathLink, theSetterPathLink)))
                    .Select((pathLink) => pathLink.CreateGetter())
                    .ToList();

            IObjWithPropGetter<object> previousPropGetter = null;
            foreach (var propGetter in PropGetters)
            {
                IObjWithPropGetter<object> thePreviousPropGetter = previousPropGetter;
                if (previousPropGetter != null)
                {
                    thePreviousPropGetter.PropertyChangedEvent += () =>
                    {
                        propGetter.TheObj = thePreviousPropGetter.GetPropValue();
                    };
                }

                previousPropGetter = propGetter;
            }

            if (previousPropGetter != null)
            {
                IObjWithPropGetter<object> thePreviousPropGetter = previousPropGetter;
                // set the last property getter to the set the setter
                previousPropGetter.PropertyChangedEvent += () =>
                {
                    TheSetter.TheObj = thePreviousPropGetter.GetPropValue();
                };
            }
        }

        public void Set(PropertyType propertyValue)
        {
            TheSetter.Set(propertyValue);
        }

        public object TheObj
        {
            get
            {
                if (PropGetters.Count > 0)
                {
                    return PropGetters[0].TheObj;
                }

                return TheSetter.TheObj;
            }

            set 
            {
                if (PropGetters.Count > 0)
                {
                    PropGetters[0].TheObj = value;
                }
                else
                {
                    TheSetter.TheObj = value;
                }
            }
        }
    }
}
