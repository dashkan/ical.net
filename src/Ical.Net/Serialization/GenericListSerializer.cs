﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Ical.Net.Serialization
{
    public class GenericListSerializer : SerializerBase
    {
        private readonly Type _innerType;
        private readonly Type _objectType;

        public GenericListSerializer(Type objectType)
        {
            _innerType = objectType.GetGenericArguments()[0];

            var listDef = typeof (List<>);
            _objectType = listDef.MakeGenericType(typeof (object));
        }

        public override Type TargetType => _objectType;

        public override string SerializeToString(object obj) => throw new NotImplementedException();

        public override object Deserialize(TextReader tr)
        {
            var p = SerializationContext.Peek() as ICalendarProperty;
            if (p == null)
            {
                return null;
            }

            // Get a serializer factory to deserialize the contents of this list
            var listObj = (IList)Activator.CreateInstance(_objectType);
            if (listObj == null)
            {
                return null;
            }

            // Get a serializer for the inner type
            var sf = GetService<ISerializerFactory>();
            var stringSerializer = sf.Build(_innerType, SerializationContext) as IStringSerializer;
            if (stringSerializer == null)
            {
                return null;
            }
            // Deserialize the inner object
            var value = tr.ReadToEnd();

            // If deserialization failed, pass the string value into the list.
            var objToAdd = stringSerializer.Deserialize(new StringReader(value)) ?? value;

            // Determine if the returned object is an IList<ObjectType>, rather than just an ObjectType.
            if (objToAdd is IList add)
            {
                //Deserialization returned an IList<ObjectType>, instead of an ObjectType.  So enumerate through the items in the list and add
                //them individually to our list.
                foreach (var innerObj in add)
                {
                    listObj.Add(new[] {innerObj});
                }
            }
            else
            {
                // Add the object to the list
                listObj.Add(new[] {objToAdd});
            }
            return listObj;
        }
    }
}