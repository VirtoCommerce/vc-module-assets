using System;

namespace VirtoCommerce.AssetsModule.Core.Swagger
{
    [Obsolete("Use VirtoCommerce.Platform.Core.Swagger.UploadFileAttribute instead.", DiagnosticId = "VC0012", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class UploadFileAttribute : Attribute
    {
        /// <summary>
        /// Logical name of the file field in the generated OpenAPI schema.
        /// Defaults to <c>"uploadedFile"</c>. This name is used only for
        /// documentation/UI (for example, Swagger UI form field name) and does
        /// not affect how the stream is read in the action.
        /// </summary>
        public string Name { get; set; } = "uploadedFile";

        /// <summary>
        /// Human‑readable description for the file field in the generated
        /// OpenAPI document (for example, tooltip in Swagger UI).
        /// </summary>
        public string Description { get; set; } = "Upload File";

        /// <summary>
        /// OpenAPI schema type for the file property.
        /// For file uploads this should remain <c>"string"</c>; the corresponding
        /// schema formatter will set <c>format = "binary"</c> to indicate a
        /// streamed binary payload.
        /// See: https://swagger.io/docs/specification/describing-request-body/file-upload/
        /// </summary>
        public string Type { get; set; } = "string";

        /// <summary>
        /// Indicates whether the file field is required in the generated
        /// OpenAPI schema. Set to <c>true</c> when a file must always be
        /// provided in the multipart request body.
        /// </summary>
        public bool Required { get; set; } = false;

        /// <summary>
        /// When <c>true</c>, describes the file field as a collection of files
        /// (for example, an array of <c>string</c>/<c>binary</c> items) in the
        /// generated OpenAPI schema, allowing multiple files to be uploaded
        /// under the same logical field name.
        /// </summary>
        public bool AllowMultiple { get; set; } = false;
    }
}
