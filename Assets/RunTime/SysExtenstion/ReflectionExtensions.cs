//using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
//using UnityEditor;

namespace EyE.Sys.Reflection
{
    /// \ingroup UnityEyETools
    /// \ingroup UnityEyEToolsPlayer
    /// <summary>
    /// The ReflectionsExtensions Class which provides some additional and convenience Reflection functions.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Simply a Unique private class that the user cannot possibly use as a parameter to the static functions of this class.  Used for optimization.
        /// </summary>
        private static class DummyClass { }

        /// <summary>
        /// Used for optimization.  If the lastBaseClassChecked is the same at the class currently being checked, it uses the cashed lastListResult rather than running the search routine.
        /// </summary>
        private static Type lastBaseClassChecked= typeof(DummyClass);
        /// <summary>
        /// Cashes the results of the last base class used in any of the static functions.  used for optimization.
        /// </summary>
        private static List<Type> lastListResult;
        /// <summary>
        /// Used for optimization to see if the cashed lastListResult can be used, or not.
        /// </summary>
        private static bool lastSortResult=false;
        /// <summary>
        /// Used for optimization to see if the cashed lastListResult can be used, or not.
        /// </summary>
        private static bool lastSortBy = false;
        /// <summary>
        /// Used for optimization to see if the cashed lastListResult can be used, or not.
        /// </summary>
        private static bool firstPass = true;
        /// <summary>
        /// Given a base type, find all classes, in all loaded assemblies, that are descendants of it.
        /// </summary>
        /// <param name="baseClass">Class to find descendants of</param>
        /// <returns>A List of Types, that contains all the descendant classes found.</returns>
        public static List<Type> GetAllDerivedClasses(this Type baseClass)
        {
            return StaticGetAllDerivedClasses(baseClass);
        }
        /// <summary>
        /// This function will find and example all assemblies in the AppDomain.  
        /// It will search them for all types that are derived from the provided baseClass type.
        /// </summary>
        /// <param name="baseClass">This is a system.Type variable that defines the particular type of object we want to find Derivatives of. If null is passed, will find all classes.</param>
        /// <param name="sortResult">Should the list be sorted before it its returned</param>
        /// <param name="sortByNameNotFullName">If the list is to be sorted, should it sort by Name or FullName</param>
        /// <returns>A list of all the derived types found.</returns>
        public static List<Type> StaticGetAllDerivedClasses(Type baseClass, bool sortResult=false, bool sortByNameNotFullName = true)
        {
            if (lastBaseClassChecked == baseClass)
            {
                return lastListResult;
            }
            lastBaseClassChecked = baseClass;
            //  Debug.Log("searching class:" + baseClass.ToString());
                AssemblyName baseClassAssembly=null;
            if(baseClass!=null)
                baseClassAssembly = Assembly.GetAssembly(baseClass).GetName();
            
            Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> workingTypeList = new List<Type>();
            //for each assembly check to see if it references that baseType's assembly.  If so, we add all types found in it to a type list for our final search. (don't want to search inside ALL assemblies, just those that reference the base class's assembly)
            foreach (Assembly asm in allAssemblies)
            {
              //  UnityEngine.Debug.Log(" checking assembly: " + asm.FullName);
                bool checkThisAssembly = false;
                if (baseClass == null) checkThisAssembly = true;
                else
                {
                    if (asm.GetName().FullName == baseClassAssembly.FullName)
                        checkThisAssembly = true;
                    else
                    {
                        AssemblyName[] refrerencedAssemblies = asm.GetReferencedAssemblies();
                        foreach (AssemblyName an in refrerencedAssemblies)
                        {
                            if (an.FullName == baseClassAssembly.FullName)
                            {
                                checkThisAssembly = true;
                                break;
                            }
                        }
                    }
                }
                if (checkThisAssembly)
                {
                   // UnityEngine.Debug.Log("Checking assembly " + asm.FullName);
                    foreach (Type currentType in asm.GetTypes())
                    {
                        if (baseClass == null)
                        {
                           // UnityEngine.Debug.Log(" adding type: " + currentType.Name);
                            workingTypeList.Add(currentType);
                        }
                        else
                        {
                            if (currentType.BaseType != null)
                                if (currentType.IsClass && !currentType.IsAbstract)
                                {
                                    if (currentType.BaseType.IsGenericType)
                                    {
                                        if (currentType.BaseType.GetGenericTypeDefinition() == baseClass)
                                            workingTypeList.Add(currentType);
                                    }
                                    else
                                    {
                                        if (currentType.IsSubclassOf(baseClass) || baseClass.IsAssignableFrom(currentType))
                                            workingTypeList.Add(currentType);
                                    }

                                }
                        }
                    }
                }
            }
            if (sortResult && (firstPass || lastSortResult!=sortResult || lastSortBy != sortByNameNotFullName))
            {
                if (sortByNameNotFullName)
                {
                    workingTypeList.Sort(
                    delegate (Type x, Type y)
                    {
                        if (x == null && y == null) return 0;
                        else if (x == null) return -1;
                        else if (y == null) return 1;
                        else return string.Compare(x.Name,y.Name, System.StringComparison.CurrentCulture);
                    });
                }
                else
                {
                    workingTypeList.Sort(
                       delegate (Type x, Type y)
                       {
                           if (x == null && y == null) return 0;
                           else if (x == null) return -1;
                           else if (y == null) return 1;
                           else return string.Compare(x.FullName,y.FullName, System.StringComparison.CurrentCulture);
                       });

                }

            }
            firstPass = false;
            lastSortResult = sortResult;
            lastListResult = workingTypeList;
            //UnityEngine.Debug.Log("Reflection Found " + objects.Count + " total types");

            return workingTypeList;
        }


        /// <summary>
        /// A Fairly dangerous function to use, it copies all non-static fields and properties from one instance of an object to another object of the same type.
        /// </summary>
        /// <typeparam name="T">This defines the type of both objects.</typeparam>
        /// <param name="original">This is the value that will be read from.</param>
        /// <param name="destination">This is the value that will be written to.</param>
        /// <returns>returns the new destination value</returns>
        public static T CopyMembers<T>(T original, T destination)
        {
            System.Type type = typeof(T);// original.GetType();
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(destination, field.GetValue(original));
            }
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(destination, prop.GetValue(original, null), null);
            }
            return destination;
        }
        /// <summary>
        /// Creates a default/empty instance of an object of the provided type.  Type must have a default constructor.
        /// </summary>
        /// <param name="typeToInstantiate"></param>
        /// <returns></returns>
        static public object InstantiateSystemObjectOfType(System.Type typeToInstantiate)
        {
            if (typeToInstantiate == typeof(string)) return string.Empty;
            return System.Activator.CreateInstance(typeToInstantiate);
        }

        /// <summary>
        /// Creates a delegate that will invoke the function specified in the methodInfo, using the object specified in instance.  No checks are performed, it is assumed that methodInfo represents a valid method of the object's type.
        /// </summary>
        /// <param name="methodInfo"> must represent a parameterless function, that is a member of object's type.</param>
        /// <param name="instance">instance of the object that will be the context of the function when executed.</param>
        /// <returns>a delegate that can invoke the provided function, in the context of the provided object.</returns>
        static public Action CreateActionFromMethodInfoAndInstance(this MethodInfo methodInfo, object instance)
        {
            return delegate () { methodInfo.Invoke(instance, null); };
        }

    }
}