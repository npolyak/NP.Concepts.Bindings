// (c) Nick Polyak 2014 - http://awebpros.com/
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
using System;
using System.Reflection;

namespace NP.Concepts.Binding.Extensions
{
    public class EventManager<EventObjectType> where EventObjectType : class
    {
        public event Action BeforeEventNameSetEvent = null;
        public event Action AfterEventNameSetEvent = null;

        MethodInfo _handlerMethodInfo = null;
        public MethodInfo HandlerMethodInfo
        {
            protected get
            {
                return _handlerMethodInfo;
            }
            set
            {
                _handlerMethodInfo = value;
            }
        }

        EventInfo _eventInfo = null;

        Type EventHandlerType
        {
            get
            {
                if (_eventInfo == null)
                    return null;

                return _eventInfo.EventHandlerType;
            }
        }

        Delegate _handlerDelegate = null;

        string _eventName = null;
        public string EventName
        {
            get
            {
                return _eventName;
            }

            set
            {
                if (_eventName.ObjEquals(value))
                    return;

                if (BeforeEventNameSetEvent != null)
                    BeforeEventNameSetEvent();

                _eventName = value;

                if (_eventName != null)
                {
                    _eventInfo = typeof(EventObjectType).GetEvent(EventName);
                    _handlerDelegate = Delegate.CreateDelegate(EventHandlerType, this, _handlerMethodInfo);
                }

                if (AfterEventNameSetEvent != null)
                    AfterEventNameSetEvent();
            }
        }


        bool CannotSetEventOnObj(EventObjectType eventObj)
        {
            return ((eventObj == null) || (EventHandlerType == null));
        }

        protected void RemoveEventFromObject(EventObjectType eventObj)
        {
            if (CannotSetEventOnObj(eventObj))
                return;

            _eventInfo.RemoveEventHandler(eventObj, _handlerDelegate);
        }

        protected void AddEventHandlerOnObject(EventObjectType eventObj)
        {
            if (CannotSetEventOnObj(eventObj))
                return;

            _eventInfo.RemoveEventHandler(eventObj, _handlerDelegate);
            _eventInfo.AddEventHandler(eventObj, _handlerDelegate);
        }
    }
}
