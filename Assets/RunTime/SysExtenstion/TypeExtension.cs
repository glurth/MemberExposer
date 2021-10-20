
using System.Reflection;
using System;
using System.Collections.Generic;
//using UnityEngine; //for debug
//using EyE.Sys.Collections; //for debug
// Author: Glurth, 2016

namespace EyE.Sys.Reflection
{
    /// \ingroup UnityEyETools
    /// \ingroup UnityEyEToolsPlayer
    /// <summary>
    /// Contains functions that operate on a System.Type, or, on a given object's Systsme.Type.
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// Examine this type of the object provided, and finds all it's members, public and private, it then searches parent types for more private members.
        /// </summary>
        /// <param name="instance">object whose type will be analyzed</param>
        /// <param name="memerTypesToInclude">By default this function will return all fields,properties, and methods it find.  Use this parameter to limit the MemberTypes included in the results.</param>
        /// <returns>a list of all MemberInfo's found</returns>
        public static List<MemberInfo> FindAllPublicAndPrivateMembersInObjectsTypeAndBaseTypes(object instance, MemberTypes memerTypesToInclude = MemberTypes.Field | MemberTypes.Method | MemberTypes.Property)
        {
            if (instance == null) throw new System.ArgumentNullException(nameof(instance));
            Type objectType = instance.GetType();
            return FindAllPublicAndPrivateMembersInTypeAndBaseTypes(objectType, memerTypesToInclude);
        }

        /// <summary>
        /// Examine this type of the object provided, and finds all it's members, public and private, it then searches parent types for more private members.
        /// </summary>
        /// <param name="typeToSearch">type that will be analyzed</param>
        /// <param name="memerTypesToInclude">By default this function will return all fields,properties, and methods it find.  Use this parameter to limit the MemberTypes included in the results.</param>
        /// <returns>a list of all MemberInfo's found</returns>
        public static List<MemberInfo> FindAllPublicAndPrivateMembersInTypeAndBaseTypes(this Type typeToSearch, MemberTypes memerTypesToInclude = MemberTypes.Field | MemberTypes.Method | MemberTypes.Property)
        {
            if (typeToSearch == null) throw new System.ArgumentNullException(nameof(typeToSearch));
            List<MemberInfo> foundMembers = new List<MemberInfo>();
            MemberInfo[] allMemb = typeToSearch.GetMembers(//BindingFlags.Static |
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            //UnityEngine.Debug.Log("members of target class:" + typeToSearch + " members: " + allMemb.ToStringEnumerated());
            foreach (MemberInfo m in allMemb)
                if ((m.MemberType & memerTypesToInclude) != 0)
                    foundMembers.Add(m);
            //allMemb[0].MemberType = MemberTypes.
            // foundMembers.AddRange(allMemb);
            List<Type> baseTypesToSearch = new List<Type>();

            for (Type currentType = typeToSearch.BaseType; currentType != null; currentType = currentType.BaseType)
            {

                MemberInfo[] allBaseMembs = currentType.GetMembers(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);// don't include public- these would be visible in objectType
                //UnityEngine.Debug.Log("members of target class:" + currentType + " members: " + allBaseMembs.ToStringEnumerated());                                                                                                                                                    //   Debug.Log("members of target's base class:" + currentType + " members: " + allBaseMembs.ToStringEnumerated<MemberInfo>());
                foreach (MemberInfo m in allBaseMembs)
                    if ((m.MemberType & memerTypesToInclude) != 0)
                        foundMembers.Add(m);
            }
            return foundMembers;
        }

        /// <summary>
        /// Given the type of an enumeration, returns the type of it's elements.
        /// </summary>
        /// <param name="collectionType">A type of an IEnumerable, for which we want to know the elements of.</param>
        /// <returns>The type of the elements, as specified in the type's generic IEnumerable interface.  Returns <code>typeof(object)</code>, when the type does not implement a generic version of IEnumerable</returns>
        /// <exception cref="ArgumentException">Thrown when the type passed in does not implement IEnumerable</exception>
        public static Type GetEnumerableElementType(this Type collectionType)
        {
            if(collectionType==null) throw new ArgumentNullException("Type passed to the extension function GetEnumerableElementType(this Type): may not be null.");
            if (collectionType.IsArray) return collectionType.GetElementType();
            bool isCollection = typeof(System.Collections.IEnumerable).IsAssignableFrom(collectionType);
            if (isCollection)
            {
                Type[] interfaces = collectionType.GetInterfaces();
                foreach (Type interf in interfaces)
                {
                    if (interf.IsGenericType)
                    {
                        if (interf.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IEnumerable<>))
                        {
                            return interf.GetGenericArguments()[0];
                        }
                    }
                }
                return typeof(object);  //if no generic version of IEnumerable is found
            }
            else
                throw new ArrayTypeMismatchException("Type passed to GetEnumerableElementType: " + collectionType + " does not actually implement IEnumerable.");

        }

        public static List<MemberInfo> GetAllPublicFieldlikeReadonlyAndActionMembers(this Type typeToSearch)
        {
            if (typeToSearch == null) throw new System.ArgumentNullException(nameof(typeToSearch));
            List<MemberInfo> foundMembers = new List<MemberInfo>();
            MemberInfo[] allMemb = typeToSearch.GetMembers(//BindingFlags.Static |
               BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            foreach (MemberInfo m in allMemb)
            {
                if(m.BehavesAs()!= MemberBehavior.other)
                    foundMembers.Add(m);
                /*bool doNotAdd = false;
                if (m.MemberType == MemberTypes.Method)
                {
                    MethodInfo methodInfo = (MethodInfo)m;
                    if (methodInfo.ReturnType == typeof(void))
                    {
                        doNotAdd = true;
                    }
                    ParameterInfo[] paramList= methodInfo.GetParameters();
                    if (paramList.Length != 0)
                    {
                        doNotAdd = true;
                        if(paramList.Length == 1)
                            if (methodInfo.GetParameters()[0].ParameterType == methodInfo.ReturnType)
                                doNotAdd = false;
                    }
                }
                if(!doNotAdd) foundMembers.Add(m);*/
            }
            return foundMembers;
        }

        public static bool IsParsable(this Type type)
        {

            if (typeof(IAmParsable).IsAssignableFrom(type))
                return true;
            if (type == typeof(int))
                return true;
            else if (type == typeof(float))
                return true;
            else if (type == typeof(long))
                return true;
            else if (type == typeof(double))
                return true;
            else if (type == typeof(string))
                return true;
            return false;
        }
    }
}//namespace
