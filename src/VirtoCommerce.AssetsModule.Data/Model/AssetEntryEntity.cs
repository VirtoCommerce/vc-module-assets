using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.AssetsModule.Data.Model
{
    public class AssetEntryEntity : AuditableEntity, IDataEntity<AssetEntryEntity, AssetEntry>
    {
        [StringLength(2083)]
        [Required]
        public string RelativeUrl { get; set; }

        [StringLength(128)]
        public string TenantId { get; set; }

        [StringLength(256)]
        public string TenantType { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        [StringLength(1024)]
        [Required]
        public string Name { get; set; }

        [StringLength(128)]
        public string MimeType { get; set; }

        /// <summary>
        /// Language code
        /// </summary>
        [StringLength(10)]
        public string LanguageCode { get; set; }

        public long Size { get; set; }

        /// <summary>
        /// User defined group
        /// </summary>
        [StringLength(64)]
        public string Group { get; set; }

        public virtual AssetEntryEntity FromModel(AssetEntry model, PrimaryKeyResolvingMap pkMap)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            LanguageCode = model.LanguageCode;
            Name = model.BlobInfo.Name;
            MimeType = model.BlobInfo.ContentType;
            RelativeUrl = model.BlobInfo.RelativeUrl;
            Size = model.BlobInfo.Size;
            Group = model.Group;
            Id = model.Id;
            TenantId = model.Tenant?.Id;
            TenantType = model.Tenant?.Type;

            return this;
        }

        public virtual AssetEntry ToModel(AssetEntry asset)
        {
            if (asset == null)
                throw new ArgumentNullException(nameof(asset));

            asset.CreatedBy = CreatedBy;
            asset.CreatedDate = CreatedDate;
            asset.ModifiedBy = ModifiedBy;
            asset.ModifiedDate = ModifiedDate;
            asset.LanguageCode = LanguageCode;
            asset.Group = Group;
            asset.Id = Id;

            if (asset.BlobInfo == null)
            {
                asset.BlobInfo = AbstractTypeFactory<BlobInfo>.TryCreateInstance();
            }
            asset.BlobInfo.Name = Name;
            asset.BlobInfo.ContentType = MimeType;
            asset.BlobInfo.RelativeUrl = RelativeUrl;            
            asset.BlobInfo.Size = Size;

            if (asset.Tenant == null)
            {
                asset.Tenant = AbstractTypeFactory<TenantIdentity>.TryCreateInstance();
            }
            asset.Tenant.Id = TenantId;
            asset.Tenant.Type = TenantType;

            return asset;
        }

        public virtual void Patch(AssetEntryEntity target)
        {
            target.LanguageCode = LanguageCode;
            target.Name = Name;
            target.MimeType = MimeType;
            target.RelativeUrl = RelativeUrl;
            target.Size = Size;
            target.Group = Group;
            target.TenantId = TenantId;
            target.TenantType = TenantType;
        }
    }
}
