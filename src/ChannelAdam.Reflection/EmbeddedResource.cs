//-----------------------------------------------------------------------
// <copyright file="EmbeddedResource.cs">
//     Copyright (c) 2016-2018 Adam Craven. All rights reserved.
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
            Stream stream = null;
            try
            {
                stream = GetAsStream(assembly, resourceName);
                using (var reader = new StreamReader(stream))
                {
                    stream = null;
                    return reader.ReadToEnd();
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
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
        public static T DeserialiseFromXmlResource<T>(Assembly assembly, string xmlResourceName)
        {
            using (var stream = GetAsStream(assembly, xmlResourceName))
            {
                return stream.DeserialiseFromXml<T>();
            }
        }

        /// <summary>
        /// Deserialises the given type from the embedded XML resource.
        /// </summary>
        /// <typeparam name="T">The type to deserialise the XML into.</typeparam>
        /// <param name="assembly">The assembly that is storing the embedded resource.</param>
        /// <param name="xmlResourceName">Name of the XML embedded resource.</param>
        /// <param name="xmlRootAttribute">XML root attribute override for deserialisation.</param>
        /// <returns>The deserialised object.</returns>
        public static T DeserialiseFromXmlResource<T>(Assembly assembly, string xmlResourceName, XmlRootAttribute xmlRootAttribute)
        {
            using (var stream = GetAsStream(assembly, xmlResourceName))
            {
                return stream.DeserialiseFromXml<T>(xmlRootAttribute);
            }
        }

        /// <summary>
        /// Deserialises the given type from the embedded XML resource.
        /// </summary>
        /// <typeparam name="T">The type to deserialise the XML into.</typeparam>
        /// <param name="assembly">The assembly that is storing the embedded resource.</param>
        /// <param name="xmlResourceName">Name of the XML embedded resource.</param>
        /// <param name="xmlAttributeOverrides">XML attribute overrides for deserialisation.</param>
        /// <returns>The deserialised object.</returns>
        public static T DeserialiseFromXmlResource<T>(Assembly assembly, string xmlResourceName, XmlAttributeOverrides xmlAttributeOverrides)
        {
            using (var stream = GetAsStream(assembly, xmlResourceName))
            {
                return stream.DeserialiseFromXml<T>(xmlAttributeOverrides);
            }
        }
    }
}
