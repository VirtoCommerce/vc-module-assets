using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.AssetsModule.Data.Model;

namespace VirtoCommerce.AssetsModule.Data.MySql
{
    public class AssetEntryEntityConfiguration : IEntityTypeConfiguration<AssetEntryEntity>
    {
        public void Configure(EntityTypeBuilder<AssetEntryEntity> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(128);
            builder.Property(x => x.RelativeUrl).HasMaxLength(640);
        }
    }
}
