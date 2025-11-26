using System;

namespace VirtoCommerce.AssetsModule.Core.Swagger
{
    /// <summary> 
    /// Use this attribute for controllers methods to allow file upload via Swagger 
    /// </summary>
    [Obsolete("Use either IFormFile or IFormFileCollection", DiagnosticId = "VC0012", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions")]
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class UploadFileAttribute : Attribute
    {
        /// <summary>
        /// The parameter name in the resulting swagger doc 
        /// </summary>
        public string Name { get; set; } = "uploadedFile";
        /// <summary>
        /// The parameter description in the resulting swagger doc 
        /// </summary>
        public string Description { get; set; } = "Upload File";
        /// <summary>
        /// Parameter type (only string value supported)
        /// Accordingly to: // https://swagger.io/docs/specification/describing-request-body/file-upload/
        /// </summary>
        public string Type { get; set; } = "string";
        /// <summary>
        /// Set true for required parameter
        /// </summary>
        public bool Required { get; set; } = false;
    }
}
