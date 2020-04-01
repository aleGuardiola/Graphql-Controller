﻿using GraphQL.Types;
using GraphqlController.Attributes;
using GraphqlController.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GraphqlController.GraphQl
{
    public class DynamicInterfaceType : InterfaceGraphType
    {
        public DynamicInterfaceType(IGraphQlTypePool graphTypePool, Type type)
        {
            if (!type.IsInterface)
            {
                throw new InvalidOperationException("The type has to be an interface");
            }

            // get the name
            var nameAttr = type.GetAttribute<TypeNameAttribute>();
            var descAttr = type.GetAttribute<TypeDescriptionAttribute>();

            // set type name and description
            Name = nameAttr?.Name ?? type.Name;
            Description = descAttr?.Description ?? DocXmlHelper.DocReader.GetTypeComments(type).Summary;

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty);

            foreach(var property in properties)
            {
                var graphType = graphTypePool.GetGraphType(property.PropertyType);
                var descriptionAttr = property.GetAttribute<FieldDescriptionAttribute>();
                var fieldNameAttr = property.GetAttribute<FieldNameAttribute>();
                var isNonNull = property.GetAttribute<NonNullFieldAttribute>() != null;

                var field = new FieldType()
                {
                    Name = fieldNameAttr == null ? property.Name : fieldNameAttr.Name,
                    Description = descriptionAttr?.Description ?? DocXmlHelper.DocReader.GetMemberComments(property).Summary,
                    ResolvedType = isNonNull ? new NonNullGraphType(graphType) : graphType                   
                };

                AddField(field);
            }
        }
    }
}
