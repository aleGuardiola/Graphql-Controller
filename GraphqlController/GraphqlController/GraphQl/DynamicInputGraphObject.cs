﻿using GraphQL.Types;
using GraphqlController.Attributes;
using GraphqlController.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphqlController.GraphQl
{
    public class DynamicInputGraphObject : InputObjectGraphType
    {
        public DynamicInputGraphObject(IGraphQlTypePool graphTypePool, Type type)
        {
            // get the name
            var nameAttr = type.GetAttribute<NameAttribute>();
            var descAttr = type.GetAttribute<DescriptionAttribute>();

            // set type name and description
            Name = nameAttr?.Name ?? type.Name;
            Description = descAttr?.Description ?? DocXmlHelper.DocReader.GetTypeComments(type).Summary;

            // Generate fields -----------------------------------------------
            // start with the properties
            var properties = type
                // Get all properties with getters
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty)
                // ignore the ones that have the ignore attribute
                .Where(x => x.GetAttribute<IgnoreAttribute>() == null);

            foreach (var property in properties)
            {
                var graphType = graphTypePool.GetInputType(property.PropertyType);
                var descriptionAttr = property.GetAttribute<DescriptionAttribute>();
                var fieldNameAttr = property.GetAttribute<NameAttribute>();
                var isNonNull = property.GetAttribute<NonNullAttribute>() != null;

                // create field
                var field = new FieldType()
                {
                    Name = fieldNameAttr == null ? property.Name : fieldNameAttr.Name,
                    Description = descriptionAttr?.Description ?? DocXmlHelper.DocReader.GetMemberComments(property).Summary,
                    ResolvedType = isNonNull ? new NonNullGraphType(graphType) : graphType,                    
                };

                AddField(field);
            }
        }
    }
}
