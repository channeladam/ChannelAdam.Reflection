//-----------------------------------------------------------------------
// <copyright file="EmbeddedResource.cs">
//     Copyright (c) 2016-2021 Adam Craven. All rights reserved.
// </copyright>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-----------------------------------------------------------------------

namespace ChannelAdam.Reflection
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    using ChannelAdam.Xml;

    public static class EmbeddedResource
    {
        /// <summary>
        /// Gets the embedded resource from the given assembly as a stream.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>The embedded resource as a stream.</returns>
        /// <remarks>Ensure that you dispose of the stream appropriately.</remarks>
        public static Stream GetAsStream(Assembly assembly, string resourceName)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new System.IO.FileNotFoundException($"Cannot find the embedded resource '{resourceName}' in assembly '{assembly.FullName}'.");
            }

            return stream;
        }

        /// <summary>
        /// Gets the string contents of the embedded resource.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>The embedded resource as a string.</returns>
        public static string GetAsString(Assembly assembly, string resourceName)
        {
            Stream? stream = null;
            try
            {
                stream = GetAsStream(assembly, resourceName);
                using var reader = new StreamReader(stream);
                stream = null;
                return reader.ReadToEnd();
            }
            finally
            {
                stream?.Dispose();
            }
        }

        /// <summary>
        /// Gets the XML resource as an XElement.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>The XML resource as an XElement.</returns>
        public static XElement GetXmlResourceAsXElement(Assembly assembly, string resourceName)
        {
            return GetAsString(assembly, resourceName).ToXElement();
        }

        /// <summary>
        /// Deserialises the given type from the embedded XML resource.
        /// </summary>
        /// <typeparam name="T">The type to deserialise the XML into.</typeparam>
        /// <param name="assembly">The assembly that is storing the embedded resource.</param>
        /// <param name="xmlResourceName">Name of the embedded XML resource.</param>
        /// <returns>The deserialised object.</returns>
        public static T? DeserialiseFromXmlResource<T>(Assembly assembly, string xmlResourceName)
        {
            using var stream = GetAsStream(assembly, xmlResourceName);
            return stream.DeserialiseFromXml<T>();
        }

        /// <summary>
        /// Deserialises the given type from the embedded XML resource.
        /// </summary>
        /// <typeparam name="T">The type to deserialise the XML into.</typeparam>
        /// <param name="assembly">The assembly that is storing the embedded resource.</param>
        /// <param name="xmlResourceName">Name of the XML embedded resource.</param>
        /// <param name="xmlRootAttribute">XML root attribute override for deserialisation.</param>
        /// <returns>The deserialised object.</returns>
        public static T? DeserialiseFromXmlResource<T>(Assembly assembly, string xmlResourceName, XmlRootAttribute xmlRootAttribute)
        {
            using var stream = GetAsStream(assembly, xmlResourceName);
            return stream.DeserialiseFromXml<T>(xmlRootAttribute);
        }

        /// <summary>
        /// Deserialises the given type from the embedded XML resource.
        /// </summary>
        /// <typeparam name="T">The type to deserialise the XML into.</typeparam>
        /// <param name="assembly">The assembly that is storing the embedded resource.</param>
        /// <param name="xmlResourceName">Name of the XML embedded resource.</param>
        /// <param name="equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak">Key of the XmlSerializer cache, unique for the given XmlAttributeOverrides, used to avoid XmlSerializer memory leaks. CAUTION: XmlAttributeOverrides.GetHashCode() returns a different value for each instance, even if each instance has the exact same objects - so consider making your own equality key based on what you added to the XmlAttributeOverrides.</param>
        /// <param name="xmlAttributeOverrides">XML attribute overrides for deserialisation.</param>
        /// <returns>The deserialised object.</returns>
        /// <remarks>
        /// <para>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlserializer?view=net-5.0#dynamically-generated-assemblies
        /// Dynamically Generated Assemblies
        /// To increase performance, the XML serialization infrastructure dynamically generates assemblies to serialize and deserialize specified types.
        /// The infrastructure finds and reuses those assemblies.
        /// This behavior occurs only when using the following constructors:
        ///   XmlSerializer.XmlSerializer(Type)
        ///   XmlSerializer.XmlSerializer(Type, String)
        /// If you use any of the other constructors, multiple versions of the same assembly are generated and never unloaded, which results in a memory leak and poor performance.
        /// The easiest solution is to use one of the previously mentioned two constructors.
        /// Otherwise, you must cache the assemblies...
        /// </para>
        /// <para>
        /// ChannelAdam.Xml does the caching for you, but it requires you to specify the key to use in the cache.
        /// CAUTION: XmlAttributeOverrides.GetHashCode() returns a different value for each instance, even if each instance has the exact same objects
        ///   - so consider making your own equality key based on what you added to the XmlAttributeOverrides.
        /// </para>
        /// </remarks>
        public static T? DeserialiseFromXmlResource<T>(Assembly assembly, string xmlResourceName, string equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, XmlAttributeOverrides xmlAttributeOverrides)
        {
            using var stream = GetAsStream(assembly, xmlResourceName);
            return stream.DeserialiseFromXml<T>(equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, xmlAttributeOverrides);
        }
    }
}
