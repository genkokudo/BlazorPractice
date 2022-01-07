namespace BlazorPractice.Domain.Contracts
{
    /// <summary>
    /// EnitityにIDの実装をさせるためのインタフェース
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IEntity<TId> : IEntity
    {
        public TId Id { get; set; }
    }

    /// <summary>
    /// Enitityであることを示すインタフェース
    /// </summary>
    public interface IEntity
    {
    }
}