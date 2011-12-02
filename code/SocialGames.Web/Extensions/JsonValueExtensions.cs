namespace Microsoft.Samples.SocialGames.GamePlay.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Json;

    public static class JsonValueExtensions 
    {
        public static IDictionary<string, object> ToDictionary(this JsonValue jsonValue)
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            if (jsonValue.JsonType != JsonType.Object)
            {
                throw new NotSupportedException("The json value is not an object.");
            }

            return ToClrCollection<Dictionary<string, object>>(jsonValue);
        }

        public static object[] ToObjectArray(this JsonValue jsonValue)
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            if (jsonValue.JsonType != JsonType.Array)
            {
                throw new NotSupportedException("The json value is not an array.");
            }

            return ToClrCollection<object[]>(jsonValue);
        }

        private static T ToClrCollection<T>(JsonValue jsonValue)
        {
            return (T)ToClrCollection(jsonValue, typeof(T));
        }

        private static object ToClrCollection(JsonValue jsonValue, Type type)
        {
            object collection = null;
            if (CanConvertToClrCollection(jsonValue, type))
            {
                JsonValue parentJsonValue = jsonValue;
                Queue<KeyValuePair<string, JsonValue>> iterator = null;
                Stack<ToClrCollectionStackInfo> stack = new Stack<ToClrCollectionStackInfo>();
                int index = 0;
                collection = CreateClrCollection(parentJsonValue);
                do
                {
                    if (iterator == null)
                    {
                        iterator = new Queue<KeyValuePair<string, JsonValue>>(parentJsonValue);
                    }

                    while ((iterator != null) && (iterator.Count > 0))
                    {
                        KeyValuePair<string, JsonValue> pair = iterator.Dequeue();
                        JsonValue value3 = pair.Value;
                        switch (value3.JsonType)
                        {
                            case JsonType.Object:
                            case JsonType.Array:
                                {
                                    object obj3 = CreateClrCollection(value3);
                                    InsertClrItem(collection, ref index, pair.Key, obj3);
                                    stack.Push(new ToClrCollectionStackInfo(parentJsonValue, collection, index, iterator));
                                    parentJsonValue = pair.Value;
                                    iterator = null;
                                    collection = obj3;
                                    index = 0;
                                    break;
                                }

                            default:
                                InsertClrItem(collection, ref index, pair.Key, ((JsonPrimitive)pair.Value).Value);
                                break;
                        }
                    }

                    if ((iterator != null) && (stack.Count > 0))
                    {
                        ToClrCollectionStackInfo info = stack.Pop();
                        collection = info.Collection;
                        iterator = info.JsonValueChildren;
                        parentJsonValue = info.ParentJsonValue;
                        index = info.CurrentIndex;
                    }
                }
                while (((stack.Count > 0) || (iterator == null)) || (iterator.Count > 0));
            }

            return collection;
        }

        private static bool CanConvertToClrCollection(JsonValue jsonValue, Type collectionType)
        {
            return jsonValue != null && ((jsonValue.JsonType == JsonType.Object && collectionType == typeof(Dictionary<string, object>)) || (jsonValue.JsonType == JsonType.Array && collectionType == typeof(object[])));
        }

        private static void InsertClrItem(object collection, ref int index, string key, object value)
        {
            Dictionary<string, object> dictionary = collection as Dictionary<string, object>;
            if (dictionary != null)
            {
                dictionary.Add(key, value);
            }
            else
            {
                object[] objArray = collection as object[];
                objArray[index] = value;
                index++;
            }
        }

        private static object CreateClrCollection(JsonValue jsonValue)
        {
            if (jsonValue.JsonType == JsonType.Object)
            {
                return new Dictionary<string, object>(jsonValue.Count);
            }

            return new object[jsonValue.Count];
        }

        private class ToClrCollectionStackInfo
        {
            public ToClrCollectionStackInfo(JsonValue jsonValue, object collection, int currentIndex, Queue<KeyValuePair<string, JsonValue>> iterator)
            {
                this.ParentJsonValue = jsonValue;
                this.CurrentIndex = currentIndex;
                this.Collection = collection;
                this.JsonValueChildren = iterator;
            }

            public object Collection { get; set; }

            public int CurrentIndex { get; set; }

            public Queue<KeyValuePair<string, JsonValue>> JsonValueChildren { get; set; }

            public JsonValue ParentJsonValue { get; set; }
        }
    }
}
