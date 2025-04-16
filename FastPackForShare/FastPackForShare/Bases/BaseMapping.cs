using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FastPackForShare.Default;

namespace FastPackForShare.Bases
{
    public abstract class BaseMapping<TEntityModel> : IEntityTypeConfiguration<TEntityModel> where TEntityModel : BaseEntityModel, new()
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
    }
}
