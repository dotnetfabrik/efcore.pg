using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal;

namespace Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class NpgsqlMethodCallTranslatorProvider : RelationalMethodCallTranslatorProvider
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual NpgsqlLTreeTranslator LTreeTranslator { get; }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public NpgsqlMethodCallTranslatorProvider(
        RelationalMethodCallTranslatorProviderDependencies dependencies,
        IModel model,
        IDbContextOptions contextOptions)
        : base(dependencies)
    {
        var npgsqlOptions = contextOptions.FindExtension<NpgsqlOptionsExtension>() ?? new();
        var supportsMultiranges = npgsqlOptions.PostgresVersionWithoutDefault is null
            || npgsqlOptions.PostgresVersionWithoutDefault.AtLeast(14);

        var sqlExpressionFactory = (NpgsqlSqlExpressionFactory)dependencies.SqlExpressionFactory;
        var typeMappingSource = (NpgsqlTypeMappingSource)dependencies.RelationalTypeMappingSource;
        var jsonTranslator = new NpgsqlJsonPocoTranslator(typeMappingSource, sqlExpressionFactory, model);
        LTreeTranslator = new NpgsqlLTreeTranslator(typeMappingSource, sqlExpressionFactory, model);

        AddTranslators(new IMethodCallTranslator[]
        {
            new NpgsqlArrayTranslator(sqlExpressionFactory, jsonTranslator, npgsqlOptions.UseRedshift),
            new NpgsqlByteArrayMethodTranslator(sqlExpressionFactory),
            new NpgsqlConvertTranslator(sqlExpressionFactory),
            new NpgsqlDateTimeMethodTranslator(typeMappingSource, sqlExpressionFactory),
            new NpgsqlFullTextSearchMethodTranslator(typeMappingSource, sqlExpressionFactory, model),
            new NpgsqlFuzzyStringMatchMethodTranslator(sqlExpressionFactory),
            new NpgsqlJsonDomTranslator(typeMappingSource, sqlExpressionFactory, model),
            new NpgsqlJsonDbFunctionsTranslator(typeMappingSource, sqlExpressionFactory, model),
            new NpgsqlLikeTranslator(sqlExpressionFactory),
            LTreeTranslator,
            new NpgsqlMathTranslator(typeMappingSource, sqlExpressionFactory, model),
            new NpgsqlNetworkTranslator(typeMappingSource, sqlExpressionFactory, model),
            new NpgsqlNewGuidTranslator(sqlExpressionFactory, npgsqlOptions.PostgresVersion),
            new NpgsqlObjectToStringTranslator(typeMappingSource, sqlExpressionFactory),
            new NpgsqlRandomTranslator(sqlExpressionFactory),
            new NpgsqlRangeTranslator(typeMappingSource, sqlExpressionFactory, model, supportsMultiranges),
            new NpgsqlRegexIsMatchTranslator(sqlExpressionFactory),
            new NpgsqlRowValueTranslator(sqlExpressionFactory),
            new NpgsqlStringMethodTranslator(typeMappingSource, sqlExpressionFactory, model),
            new NpgsqlTrigramsMethodTranslator(typeMappingSource, sqlExpressionFactory, model),
        });
    }
}
