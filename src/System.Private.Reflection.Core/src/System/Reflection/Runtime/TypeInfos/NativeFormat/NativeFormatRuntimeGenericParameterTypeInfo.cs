﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection.Runtime.General;
using System.Reflection.Runtime.MethodInfos;
using System.Reflection.Runtime.TypeInfos;
using System.Reflection.Runtime.CustomAttributes;

using Internal.Reflection.Tracing;

using Internal.Metadata.NativeFormat;

namespace System.Reflection.Runtime.TypeInfos.NativeFormat
{
    internal abstract partial class NativeFormatRuntimeGenericParameterTypeInfo : RuntimeGenericParameterTypeInfo
    {
        protected NativeFormatRuntimeGenericParameterTypeInfo(MetadataReader reader, GenericParameterHandle genericParameterHandle, GenericParameter genericParameter)
            : base(genericParameter.Number)
        {
            Reader = reader;
            GenericParameterHandle = genericParameterHandle;
            _genericParameter = genericParameter;
        }

        public sealed override IEnumerable<CustomAttributeData> CustomAttributes
        {
            get
            {
#if ENABLE_REFLECTION_TRACE
                if (ReflectionTrace.Enabled)
                    ReflectionTrace.TypeInfo_CustomAttributes(this);
#endif

                return RuntimeCustomAttributeData.GetCustomAttributes(Reader, _genericParameter.CustomAttributes);
            }
        }

        public sealed override GenericParameterAttributes GenericParameterAttributes
        {
            get
            {
                return _genericParameter.Flags;
            }
        }

        public sealed override int MetadataToken
        {
            get
            {
                throw new InvalidOperationException(SR.NoMetadataTokenAvailable);
            }
        }

        protected sealed override int InternalGetHashCode()
        {
            return GenericParameterHandle.GetHashCode();
        }

        protected GenericParameterHandle GenericParameterHandle { get; }

        protected MetadataReader Reader { get; }

        public sealed override string InternalGetNameIfAvailable(ref Type rootCauseForFailure)
        {
            if (_genericParameter.Name.IsNull(Reader))
                return string.Empty;
            return _genericParameter.Name.GetString(Reader);
        }

        protected sealed override QTypeDefRefOrSpec[] Constraints
        {
            get
            {
                MetadataReader reader = Reader;
                List<QTypeDefRefOrSpec> constraints = new List<QTypeDefRefOrSpec>();
                foreach (Handle constraintHandle in _genericParameter.Constraints)
                {
                    constraints.Add(new QTypeDefRefOrSpec(reader, constraintHandle));
                }
                return constraints.ToArray();
            }
        }

        private readonly GenericParameter _genericParameter;
    }
}
