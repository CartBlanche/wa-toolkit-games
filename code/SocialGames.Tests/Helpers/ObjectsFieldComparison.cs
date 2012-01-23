namespace Microsoft.Samples.SocialGames.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public class ObjectsFieldComparison
    {
        private object firstObject;

        private object secondObject;

        private ObjectsFieldComparison(object firstObject, object secondObject)
        {
            this.firstObject = firstObject;
            this.secondObject = secondObject;
        }

        public static ObjectsFieldComparison Comparing(object firstObject, object secondObject)
        {
            return new ObjectsFieldComparison(firstObject, secondObject);
        }

        public bool DoIt()
        {
            foreach (var firstObjectField in this.firstObject.GetType().GetFields())
            {
                FieldInfo secondObjectField = this.secondObject.GetType().GetField(firstObjectField.Name);
                if (firstObjectField.FieldType == secondObjectField.FieldType)
                {
                    if (firstObjectField.GetValue(this.firstObject) != secondObjectField.GetValue(this.secondObject))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            foreach (var firstObjectProperty in this.firstObject.GetType().GetProperties())
            {
                PropertyInfo secondObjectProperty = this.secondObject.GetType().GetProperty(firstObjectProperty.Name);
                if (firstObjectProperty.PropertyType == secondObjectProperty.PropertyType)
                {
                    bool firstValueIsNull = firstObjectProperty.GetValue(this.firstObject, null) == null;
                    bool secondValueIsNull = secondObjectProperty.GetValue(this.secondObject, null) == null;
                    if ((firstValueIsNull != secondValueIsNull)
                        || (!firstValueIsNull
                            && !firstObjectProperty.GetValue(this.firstObject, null).Equals(secondObjectProperty.GetValue(this.secondObject, null))))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
