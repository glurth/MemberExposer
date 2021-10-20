
using System.Reflection;
using System;
using System.Collections.Generic;
//using UnityEngine; //for debug

// Author: Glurth, 2016

namespace EyE.Sys.Reflection
{

    [System.Serializable]
    public enum MemberBehavior
    {
        fieldLike=1, // read and write a single value of the member type
        readOnly=2, //read-only single value of the member type
        writeOnly=3, // write a single value of the member type
        writeToRead=4,  //write a single value of the member type, and get/read another value of the same type in response
        action=5,  //cannot read or write a value- but CAN perform an action
        other=4
    }

    /// \ingroup UnityEyETools
    /// \ingroup UnityEyEToolsPlayer
    /// <summary>
    /// This Utility class extends the .net MemberInfo class, in order to provide a single method to access both .net PropetyInfo and FieldInfo objects, transparently.
    /// Two functions are provided, GetValue and GetSystemType.  
    /// A NotImplementedException will be thrown if the provided MemberInfo parameter is for any MemberType other than MemberTypes.Field, or MemberTypes.Property.
    /// </summary>
    public static class MemberInfoExtension
    {

        /// <summary>
        /// This is an Extension Method for the MemberInfo class.  It provides specific types of MemberInfo instances, with ability to call GetValue().  
        /// It automatically performs a check to see if the MemberInfo is a FieldInfo or PropertyInfo, and call the appropriate class's GetValue() function.
        /// WARNING: Calling this function on a MemberInfo instance, that is NOT a FieldInfo or PropertyInfo, will raise a NotImplementedException.
        /// </summary>
        /// <param name="memberInfo">this value is automatically set to be the calling MemberInfo's instance.  (Since this is an Extension Method, extending theMemberInfo class)</param>
        /// <param name="forObject">this parameters references the object we will be looking inside.  Is it this instance we will be getting the memberInfo's value from.</param>
        /// <returns>returns an Object that contains the found value.  See FieldInfo's and PropertyInfo's  GetValue() docs for more details:  https://msdn.microsoft.com/en-us/library/system.reflection.fieldinfo.getvalue(v=vs.110).aspx</returns>
        public static object GetValue(this MemberInfo memberInfo, object forObject)
        {
            if (memberInfo == null) throw new System.ArgumentNullException(nameof(memberInfo));
            // Debug.Log("extended GetValue function called. memberInfo:" + memberInfo.Name + " forObject: " + forObject);
            if (forObject == null || forObject.Equals(null)) return null;
          //  Debug.Log("extended GetValue function called. forObject not null");
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
//                    return ((FieldInfo)memberInfo).GetValue(forObject);
                    return (memberInfo as FieldInfo).GetValue(forObject);
                case MemberTypes.Property:
                    {
                        PropertyInfo pi = (memberInfo as PropertyInfo);
                        MethodInfo getMethod = pi.GetGetMethod(true);
                        if(getMethod!=null)
                            return getMethod.Invoke(forObject, null);
                       // Debug.LogWarning("Unable to find Getter function for property member " + memberInfo.Name);
                        throw new NotImplementedException("MemberInfoExtensions: Unable to find public Getter function for property member " + memberInfo.Name);
                        //return pi.GetValue(forObject,null);
                    }
                case MemberTypes.Method:
                    {
                        MethodInfo mi = (memberInfo as MethodInfo);
                        if (mi.GetParameters().Length == 0)
                            if (mi.ReturnType != typeof(void))
                                return mi.Invoke(forObject, null);
                            else
                            {
                                mi.Invoke(forObject, null);
                                return null;
                            }

                        throw new NotImplementedException("MemberInfoExtensions: Unable to find public function for method member " + memberInfo.Name);
                    }
                default:
                    throw new NotImplementedException("MemberInfoExtensions: can only get values for functions that return a value, accessors(properties), and variables (fields)");
            }
        }

        /// <summary>
        /// This is an Extension Method for the MemberInfo class.  It provides specific types of MemberInfo instances, with ability to call SetValue().  
        /// It automatically performs a check to see if the MemberInfo is a FieldInfo or PropertyInfo, and call the appropriate class's GetValue() function.
        /// WARNING: Calling this function on a MemberInfo instance, that is NOT a FieldInfo or PropertyInfo, will raise a NotImplementedException.
        /// </summary>
        /// <param name="memberInfo">this value is automatically set to be the calling MemberInfo's instance.  (Since this is an Extension Method, extending theMemberInfo class)</param>
        /// <param name="forObject">this parameters references the object we will be looking inside.  Is it this instance we will be getting the memberInfo's value from.</param>
        /// <param name="toValue">object that represents the value to be assigned.</param>
        public static void SetValue(this MemberInfo memberInfo, object forObject, object toValue)
        {
            if (memberInfo == null) throw new System.ArgumentNullException(nameof(memberInfo));
            // Debug.Log("extended SetValue function called. memberInfo:" + memberInfo.Name + " forObject: " + forObject);
            if (forObject == null || forObject.Equals(null)) return;
            //  Debug.Log("extended SetValue function called. forObject not null");
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    {
                        (memberInfo as FieldInfo).SetValue(forObject, (object)toValue);
                        return;
                    }
                case MemberTypes.Property:
                    {
                        PropertyInfo pi = (memberInfo as PropertyInfo);
                        if(pi.CanWrite)
                    //    Debug.Log("assigning value via setting to property: " + pi.Name + " value assigned is: " + toValue);
                            pi.GetSetMethod(true).Invoke(forObject, new object[1] { toValue } );
                        return;
                    }
                case MemberTypes.Method:
                    {
                        MethodInfo mi = (memberInfo as MethodInfo);
                        ParameterInfo[] paramsFound = mi.GetParameters();
                        if (paramsFound.Length == 1)
                        {
                            if (mi.ReturnType == typeof(void))
                                mi.Invoke(forObject, new object[1] { toValue });
                        }
                        else
                        {
                            if (paramsFound.Length == 0)
                                mi.Invoke(forObject, null);
                        }
                        return;// throw new NotImplementedException();
                    }
                default:
                    return;
                    //throw new NotImplementedException();
            }
        }

        /// <summary>
        /// An Extension Method for the MemberInfo class.  It determines the type of properties and field.  For methods, it determines the return type, including void.
        /// It automatically performs a check to see if the MemberInfo is a FieldInfo, PropertyInfo or MethodInfo, and find the type based using those, descendant classes.
        /// WARNING: Calling this function on a MemberInfo instance, that is NOT a FieldInfo, PropertyInfo or MethodInfo, will raise a NotImplementedException.
        /// </summary>
        /// <param name="memberInfo">this value is automatically set to be the calling MemberInfo's instance.  (Since this is an Extension Method, extending theMemberInfo class)</param>
        /// <returns>returns a System.Type that is the type of the member.  See FieldInfo's and PropertyInfo's  FieldType or PropertyType docs for more details: https://msdn.microsoft.com/en-us/library/system.reflection.fieldinfo.fieldtype(v=vs.110).aspx , https://msdn.microsoft.com/en-us/library/system.reflection.fieldinfo.fieldtype(v=vs.110).aspx</returns>
        public static Type GetSystemType(this MemberInfo memberInfo)
        {
            if (memberInfo == null) throw new System.ArgumentNullException(nameof(memberInfo));
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return (memberInfo as FieldInfo).FieldType;//(forObject);
                case MemberTypes.Property:
                    return (memberInfo as PropertyInfo).PropertyType;//(forObject, null);
                case MemberTypes.Method:
                    {
                        Type returnType= (memberInfo as MethodInfo).ReturnType;
                     //   if (returnType == null)
                     //       return typeof(VoidReturnFunction);
                        return (memberInfo as MethodInfo).ReturnType;//(forObject, null);
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Convenience function to find any members in the object's type, with the given name. 
        /// BindingFlags checked: BindingFlags.Static, BindingFlags.Instance, BindingFlags.NonPublic, BindingFlags.Public
        /// </summary>
        /// <param name="instance">object who's type will be checked for the specified member.  May not be null.</param>
        /// <param name="name">name in code of the member to find.</param>
        /// <returns>MemberInfo that indicates which member of the object has the provided name.</returns>
        public static MemberInfo FindMemberByName(object instance, string name)
        {
            //MemberInfo[] allMemb = instance.GetType().GetMembers(BindingFlags.Static|BindingFlags.Instance|BindingFlags.NonPublic| BindingFlags.Public);
            List<MemberInfo> allMemb = TypeExtension.FindAllPublicAndPrivateMembersInObjectsTypeAndBaseTypes(instance);
            foreach (MemberInfo memb in allMemb)
            {
                if (memb.Name == name) return memb;
            }
            return null;
        }

        public static MemberInfo FindMemberByName(Type typeToSearch, string name)
        {
            //MemberInfo[] allMemb = instance.GetType().GetMembers(BindingFlags.Static|BindingFlags.Instance|BindingFlags.NonPublic| BindingFlags.Public);
            List<MemberInfo> allMemb = TypeExtension.FindAllPublicAndPrivateMembersInObjectsTypeAndBaseTypes(typeToSearch);
            foreach (MemberInfo memb in allMemb)
            {
                if (memb.Name == name) return memb;
            }
            return null;
        }

        /// <summary>
        /// Determines if the specified attribute type is assigned to the provided memberInfo
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="attributeType"></param>
        /// <returns>null if no attributes of the specified type are found on the memberInfo.  If one or more is found, the first is returned.</returns>
        static public Attribute GetAttributeOfType(this MemberInfo memberInfo, Type attributeType)
        {

            object[] foundAttributes = memberInfo.GetSystemType().GetCustomAttributes(false);
            foreach (object obj in foundAttributes)
                if (obj.GetType() == attributeType)
                    return (Attribute)obj;
            return null;
        }


        static public MemberBehavior BehavesAs(this MemberInfo memberInfo)
        {
            if (memberInfo == null)
                throw new ArgumentException("null passed to MemberInfoExtension.BehavesAs function");
            /*
                -functions that return their one and only param type, will be considered read/write accessors and equivalent to a "field"
                -functions that return a not-void type and take no params will be considered read-only accessors
                -functions that take no param, and return void will be considered "actions"
                -functions that take a param, and return void will be unusable here.
                -functions that take a param, and return a different type that the param, will be unusable here.
             */
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return MemberBehavior.fieldLike;
                case MemberTypes.Property:
                    PropertyInfo propInfo = (PropertyInfo)memberInfo;
                    if (propInfo.CanWrite)
                    {
                        MethodInfo SetFunc = propInfo.GetSetMethod();
                        if (SetFunc.IsPublic)
                            return MemberBehavior.fieldLike;
                    }
                    return MemberBehavior.readOnly;
                case MemberTypes.Method:
                    MethodInfo methInfo = (MethodInfo)memberInfo;
                    ParameterInfo[] methParams = methInfo.GetParameters();
                    if (methParams.Length > 1) return MemberBehavior.other;
                    if (methParams.Length == 0)
                    {
                        if (methInfo.ReturnType == typeof(void))
                            return MemberBehavior.action; // no param, void return
                        return MemberBehavior.readOnly; // no param - does not return void
                    }
                    //if (methParams.Length == 1) // not needed- by deduction
                    {
                     
                        if (methParams[0].ParameterType == methInfo.ReturnType)
                            return MemberBehavior.writeToRead; // single param and return type are the same
                        return MemberBehavior.writeOnly;
                    }
                default:
                    return MemberBehavior.other;
            }
            //return MemberBehavior.fieldLike;
        }

    }// end MemberInfo extension class

}//namespace