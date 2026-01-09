namespace Pronia.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pronia.Models;

public class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
{
    public void Configure(EntityTypeBuilder<BasketItem> builder)
    {
       builder.HasOne(b => b.Product).WithMany(p=>p.BasketItems)
            .HasForeignKey(x=>x.ProductId).HasPrincipalKey(x=>x.Id)
            .OnDelete(DeleteBehavior.Cascade);

         builder.HasOne(b => b.AppUser).WithMany(a => a.BasketItems).HasForeignKey(x=>x.AppUserId)
            .HasPrincipalKey(x=>x.Id).OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(options =>
        {
            options.HasCheckConstraint("CK_BasketItem_Count", "[Count] >= 1");
        }
        );

    }
}
