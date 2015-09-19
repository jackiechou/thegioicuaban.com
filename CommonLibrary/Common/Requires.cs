using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Localization;

namespace CommonLibrary.Common
{
    public static class Requires
    {

        #region "Public Methods"

        //public static void IsTypeOf<T>(string argName, object argValue)
        //{
        //    if (!((argValue) is T))
        //    {
        //        throw new ArgumentException(Localization.GetExceptionMessage("ValueMustBeOfType", "The argument {0} must be of type {1}.", argName, typeof(T).FullName));
        //    }
        //}

        //public static void NotNegative(string argName, int argValue)
        //{
        //    if (argValue < 0)
        //    {
        //        throw new ArgumentOutOfRangeException(argName, Localization.GetExceptionMessage("ValueCannotBeNegative", "The argument {0} cannot be negative.", argName));
        //    }
        //}

        public static void NotNull(string argName, object argValue)
        {
            if (argValue == null)
            {
                throw new ArgumentNullException(argName);
            }
        }

        public static void NotNullOrEmpty(string argName, string argValue)
        {
            if (string.IsNullOrEmpty(argValue))
            {
                throw new ArgumentException(argName);
            }
        }

        //public static void PropertyNotNullOrEmpty(string argName, string argProperty, string propertyValue)
        //{
        //    if (string.IsNullOrEmpty(propertyValue))
        //    {
        //        throw new ArgumentException(argName, Localization.GetExceptionMessage("PropertyCannotBeNullOrEmpty", "The property {1} in object {0} cannot be null or empty.", argName, argProperty));
        //    }
        //}

        //public static void PropertyNotNegative(string argName, string argProperty, int propertyValue)
        //{
        //    if (propertyValue < 0)
        //    {
        //        throw new ArgumentOutOfRangeException(argName, Localization.GetExceptionMessage("PropertyCannotBeNegative", "The property {1} in object {0} cannot be negative.", argName, argProperty));
        //    }
        //}

        //public static void PropertyNotEqualTo<TValue>(string argName, string argProperty, TValue propertyValue, TValue testValue) where TValue : IEquatable<TValue>
        //{
        //    if (propertyValue.Equals(testValue))
        //    {
        //        throw new ArgumentException(argName, Localization.GetExceptionMessage("PropertyNotEqualTo", "The property {1} in object {0} is invalid.", argName, argProperty));
        //    }
        //}

        #endregion

    }

}
