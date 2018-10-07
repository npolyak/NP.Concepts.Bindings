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
    public class CompositePathGetter<PropertyType> : IObjWithPropGetter<PropertyType>
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
                return _propGetters[_propGetters.Count - 1];
            }
        }

        IEnumerable<IBindingPathLink<object>> _pathLinks;

        List<IObjWithPropGetter<object>> _propGetters = new List<IObjWithPropGetter<object>>();
        public List<IObjWithPropGetter<object>> PropGetters
        {
            get
            {
                return _propGetters;
            }
        }

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
                _propGetters.Add(new SimplePropGetter<object>());
                return;
            }

            IObjWithPropGetter<object> previousPropGetter = null;
            foreach (var pathLink in _pathLinks)
            {
                IObjWithPropGetter<object> propGetter = pathLink.CreateGetter();

                _propGetters.Add(propGetter);

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
                this._propGetters[0].TheObj = _obj;

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

    public class CompositePathSetter<PropertyType> : IObjWithPropSetter<PropertyType>
    {
        IObjWithPropSetter<object> _theSetter = null;
        public IObjWithPropSetter<object> TheSetter
        {
            get
            {
                return _theSetter;
            }
        }

        List<IObjWithPropGetter<object>> _propGetters;
        public List<IObjWithPropGetter<object>> PropGetters
        {
            get
            {
                return _propGetters;
            }
        }

        public CompositePathSetter(IEnumerable<IBindingPathLink<object>> pathLinks)
        {
            IBindingPathLink<object> theSetterPathLink = pathLinks.Last();

            _theSetter = theSetterPathLink.CreateSetter();

            _propGetters =
                pathLinks
                    .TakeWhile((pathLink) => (!Object.ReferenceEquals(pathLink, theSetterPathLink)))
                    .Select((pathLink) => pathLink.CreateGetter())
                    .ToList();

            IObjWithPropGetter<object> previousPropGetter = null;
            foreach (var propGetter in _propGetters)
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
                    _theSetter.TheObj = thePreviousPropGetter.GetPropValue();
                };
            }
        }

        public void Set(PropertyType propertyValue)
        {
            _theSetter.Set(propertyValue);
        }

        public object TheObj
        {
            get
            {
                if (_propGetters.Count > 0)
                {
                    return _propGetters[0].TheObj;
                }

                return _theSetter.TheObj;
            }

            set 
            {
                if (_propGetters.Count > 0)
                {
                    _propGetters[0].TheObj = value;
                }
                else
                {
                    _theSetter.TheObj = value;
                }
            }
        }
    }
}
