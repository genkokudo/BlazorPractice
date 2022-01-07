using BlazorPractice.Domain.Contracts;
using BlazorPractice.Domain.Entities.Misc;

namespace BlazorPractice.Domain.Entities.ExtendedAttributes
{
    /// <summary>
    /// AuditableEntityExtendedAttributeをDocumentエンティティに限定したもの
    /// なのでDocument専用
    /// </summary>
    public class DocumentExtendedAttribute : AuditableEntityExtendedAttribute<int, int, Document>
    {
    }
}