using BlazorPractice.Domain.Contracts;
using BlazorPractice.Domain.Entities.Misc;

namespace BlazorPractice.Domain.Entities.ExtendedAttributes
{
    public class DocumentExtendedAttribute : AuditableEntityExtendedAttribute<int, int, Document>
    {
    }
}