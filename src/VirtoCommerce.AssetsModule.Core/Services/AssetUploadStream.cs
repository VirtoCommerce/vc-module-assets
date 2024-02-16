using System.IO;
using VirtoCommerce.AssetsModule.Core.Events;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.AssetsModule.Core.Services
{
    public class AssetUploadStream : Stream
    {
        private readonly Stream _innerStream;
        private readonly string _assetProvider;
        private readonly string _blobUrl;
        private readonly IEventPublisher _eventPublisher;

        public AssetUploadStream(string assetProvider, string blobUrl, IEventPublisher eventPublisher, Stream innerStream)
        {
            _assetProvider = assetProvider;
            _blobUrl = blobUrl;
            _eventPublisher = eventPublisher;
            _innerStream = innerStream;
        }

        public override bool CanRead => _innerStream.CanRead;

        public override bool CanSeek => _innerStream.CanSeek;

        public override bool CanWrite => _innerStream.CanWrite;

        public override long Length => _innerStream.Length;

        public override long Position { get => _innerStream.Position; set => _innerStream.Position = value; }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _innerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _innerStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_eventPublisher != null)
            {
                var eventData = new AssetsUploadInfo
                {
                    Id = _blobUrl,
                    Uri = _blobUrl,
                    Provider = _assetProvider
                };

                _eventPublisher.Publish(new AssetUploadEvent([
                    new GenericChangedEntry<AssetsUploadInfo>(eventData, EntryState.Added)])).GetAwaiter().GetResult();
            }
        }
    }
}
