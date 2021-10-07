namespace VirtoCommerce.AssetsModule.Core.Assets
{
    public interface IBlobUrlResolver
    {
        string GetAbsoluteUrl(string blobKey);
    }
}
