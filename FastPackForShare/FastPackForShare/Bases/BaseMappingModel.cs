using FastPackForShare.Default;

namespace FastPackForShare.Bases;

public abstract class BaseMappingModel<TEntityModel> : IEntityTypeConfiguration<TEntityModel> where TEntityModel : BaseEntityModel, new()
{
    protected EntityTypeBuilder<TEntityModel> _builder;

    public abstract void Configure(EntityTypeBuilder<TEntityModel> builder);

    public virtual void ConfigureBase(string tableName)
    {
        _builder.ToTable(tableName);
        _builder.HasKey(x => x.Id);
        _builder.Property(x => x.Id).ValueGeneratedOnAdd().UseIdentityColumn(1, 1).HasColumnOrder(0);
        _builder.Property(x => x.CreatedAt).HasColumnOrder(1);
        _builder.Property(x => x.UpdatedAt).HasColumnOrder(2);
        _builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true).HasColumnOrder(3);
    }

    public virtual void ConfigureBaseWithHistory(string tableName)
    {
        _builder.ToTable(tableName, e => e.IsTemporal(t =>
        {
            t.HasPeriodStart("InicioValidade");
            t.HasPeriodEnd("TerminoValidade");
            t.UseHistoryTable($"{tableName}History");
        }));
    }
}
