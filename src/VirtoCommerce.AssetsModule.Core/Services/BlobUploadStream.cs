using System.IO;
using System.Threading.Tasks;
using VirtoCommerce.AssetsModule.Core.Events;
using VirtoCommerce.AssetsModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.AssetsModule.Core.Services;

public class BlobUploadStream : Stream
{
    private readonly Stream _innerStream;
    private readonly string _blobUrl;
    private readonly string _providerName;
    private readonly IEventPublisher _eventPublisher;

    public BlobUploadStream(Stream innerStream, string blobUrl, string providerName, IEventPublisher eventPublisher)
    {
        _innerStream = innerStream;
        _blobUrl = blobUrl;
        _providerName = providerName;
        _eventPublisher = eventPublisher;
    }

    public override bool CanRead => _innerStream.CanRead;

    public override bool CanSeek => _innerStream.CanSeek;

    public override bool CanWrite => _innerStream.CanWrite;

    public override long Length => _innerStream.Length;

    public override long Position
    {
        get => _innerStream.Position;
        set => _innerStream.Position = value;
    }

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

        if (disposing)
        {
            _innerStream.Dispose();

            RaiseBlobCreatedEvent().GetAwaiter().GetResult();
        }
    }

    protected virtual Task RaiseBlobCreatedEvent()
    {
        if (_eventPublisher is null)
        {
            return Task.CompletedTask;
        }

        var eventData = new BlobEventInfo
        {
            Id = _blobUrl,
            Uri = _blobUrl,
            Provider = _providerName,
        };

        return _eventPublisher.Publish(new BlobCreatedEvent(eventData));
    }
}
