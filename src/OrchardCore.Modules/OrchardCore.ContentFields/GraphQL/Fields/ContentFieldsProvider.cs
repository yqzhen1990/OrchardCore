using System;
using System.Collections.Generic;
using GraphQL.Resolvers;
using GraphQL.Types;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.GraphQL.Types;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using OrchardCore.ContentManagement.Metadata.Models;

namespace OrchardCore.ContentFields.GraphQL.Fields
{
    public class ContentFieldsProvider : IContentFieldProvider
    {
        private static readonly Dictionary<string, FieldTypeDescriptor> _contentFieldTypeMappings = new()
        {
            {
                nameof(BooleanField),
                new FieldTypeDescriptor
                {
                    Description = "Boolean field",
                    FieldType = typeof(BooleanGraphType),
                    UnderlyingType = typeof(BooleanField),
                    FieldAccessor = field => field.Content.Value,
                }
            },
            {
                nameof(DateField),
                new FieldTypeDescriptor
                {
                    Description = "Date field",
                    FieldType = typeof(DateGraphType),
                    UnderlyingType = typeof(DateField),
                    FieldAccessor = field => field.Content.Value,
                }
            },
            {
                nameof(DateTimeField),
                new FieldTypeDescriptor
                {
                    Description = "Date & time field",
                    FieldType = typeof(DateTimeGraphType),
                    UnderlyingType = typeof(DateTimeField),
                    FieldAccessor = field => field.Content.Value,
                }
            },
            {
                nameof(NumericField),
                new FieldTypeDescriptor
                {
                    Description = "Numeric field",
                    FieldType = typeof(DecimalGraphType),
                    UnderlyingType = typeof(NumericField),
                    FieldAccessor = field => field.Content.Value,
                }
            },
            {
                nameof(TextField),
                new FieldTypeDescriptor
                {
                    Description = "Text field",
                    FieldType = typeof(StringGraphType),
                    UnderlyingType = typeof(TextField),
                    FieldAccessor = field => field.Content.Text,
                }
            },
            {
                nameof(TimeField),
                new FieldTypeDescriptor
                {
                    Description = "Time field",
                    FieldType = typeof(TimeSpanGraphType),
                    UnderlyingType = typeof(TimeField),
                    FieldAccessor = field => field.Content.Value,
                }
            },
            {
                nameof(MultiTextField),
                new FieldTypeDescriptor
                {
                    Description = "Multi text field",
                    FieldType = typeof(ListGraphType<StringGraphType>),
                    UnderlyingType = typeof(MultiTextField),
                    FieldAccessor = field => field.Content.Values,
                }
            }
        };

        public FieldType GetField(ContentPartFieldDefinition field)
        {
            if (!_contentFieldTypeMappings.TryGetValue(field.FieldDefinition.Name, out var value))
            {
                return null;
            }

            var fieldDescriptor = value;
            return new FieldType
            {
                Name = field.Name,
                Description = fieldDescriptor.Description,
                Type = fieldDescriptor.FieldType,
                Resolver = new FuncFieldResolver<ContentElement, object>(context =>
                {
                    // Check if part has been collapsed by trying to get the parent part.
                    ContentElement contentPart = context.Source.Get<ContentPart>(field.PartDefinition.Name);

                    // Part is not collapsed, access field directly.
                    contentPart ??= context.Source;

                    var contentField = contentPart?.Get(fieldDescriptor.UnderlyingType, field.Name);

                    contentField ??= context.Source.Get(fieldDescriptor.UnderlyingType, field.Name);

                    return contentField == null ? null : fieldDescriptor.FieldAccessor(contentField);
                }),
            };
        }

        private sealed class FieldTypeDescriptor
        {
            public string Description { get; set; }
            public Type FieldType { get; set; }
            public Type UnderlyingType { get; set; }
            public Func<ContentElement, object> FieldAccessor { get; set; }
        }
    }
}
