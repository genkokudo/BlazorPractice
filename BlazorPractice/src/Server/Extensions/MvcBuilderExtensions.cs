using BlazorPractice.Application.Configurations;
using BlazorPractice.Application.Features.ExtendedAttributes.Commands.AddEdit;
using BlazorPractice.Application.Validators.Features.ExtendedAttributes.Commands.AddEdit;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace BlazorPractice.Server.Extensions
{
    /// <summary>
    /// Startupで使用
    /// FluentValidationの実装クラスをすべて登録する
    /// </summary>
    internal static class MvcBuilderExtensions
    {
        internal static IMvcBuilder AddValidators(this IMvcBuilder builder)
        {
            // FluentValidationの自動登録機能：特定のアセンブリ内のすべてのバリデーターを自動的に登録する
            builder.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AppConfiguration>()); // 型引数これで正しいの？BlazorPractice.Applicationを指定してるってこと？
            return builder;
        }

        /// <summary>
        /// AddEditExtendedAttributeCommandValidatorを実装しているバリデータをまとめて登録する
        /// </summary>
        /// <param name="services"></param>
        internal static void AddExtendedAttributesValidators(this IServiceCollection services)
        {
            #region AddEditExtendedAttributeCommandValidator

            var addEditExtendedAttributeCommandValidatorType = typeof(AddEditExtendedAttributeCommandValidator<,,,>);
            var validatorTypes = addEditExtendedAttributeCommandValidatorType
                .Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.BaseType?.IsGenericType == true)
                .Select(t => new
                {
                    BaseGenericType = t.BaseType,
                    CurrentType = t
                })
                .Where(t => t.BaseGenericType?.GetGenericTypeDefinition() == typeof(AddEditExtendedAttributeCommandValidator<,,,>))
                .ToList();

            foreach (var validatorType in validatorTypes)
            {
                var addEditExtendedAttributeCommandType = typeof(AddEditExtendedAttributeCommand<,,,>).MakeGenericType(validatorType.BaseGenericType.GetGenericArguments());
                var iValidator = typeof(IValidator<>).MakeGenericType(addEditExtendedAttributeCommandType);
                services.AddScoped(iValidator, validatorType.CurrentType);
            }

            #endregion AddEditExtendedAttributeCommandValidator
        }
    }
}