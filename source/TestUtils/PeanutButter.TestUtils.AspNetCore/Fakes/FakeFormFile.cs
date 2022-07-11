﻿using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PeanutButter.Utils;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace PeanutButter.TestUtils.AspNetCore.Fakes;

/// <summary>
/// Implements a fake form file
/// </summary>
public class FakeFormFile : IFormFile, IFake
{
    private Stream _content;

    /// <summary>
    /// Default constructor: create an empty form file
    /// </summary>
    public FakeFormFile()
    {
    }

    /// <summary>
    /// Create a form file
    /// </summary>
    /// <param name="content"></param>
    /// <param name="name"></param>
    /// <param name="fileName"></param>
    public FakeFormFile(
        string content,
        string name,
        string fileName
    ) : this(content, name, fileName, null)
    {
    }

    /// <summary>
    /// Create a form file
    /// </summary>
    /// <param name="content"></param>
    /// <param name="name"></param>
    /// <param name="fileName"></param>
    /// <param name="mimeType"></param>
    public FakeFormFile(
        string content,
        string name,
        string fileName,
        string mimeType
    )
        : this(Encoding.UTF8.GetBytes(content), name, fileName, mimeType)
    {
    }

    /// <summary>
    /// Create a form file
    /// </summary>
    /// <param name="content"></param>
    /// <param name="name"></param>
    /// <param name="fileName"></param>
    public FakeFormFile(byte[] content, string name, string fileName)
        : this(content, name, fileName, null)
    {
    }

    /// <summary>
    /// Create a form file
    /// </summary>
    /// <param name="content"></param>
    /// <param name="name"></param>
    /// <param name="fileName"></param>
    /// <param name="mimeType"></param>
    public FakeFormFile(byte[] content, string name, string fileName, string mimeType)
        : this(new MemoryStream(content), name, fileName, mimeType)
    {
    }

    /// <summary>
    /// Create a form file
    /// </summary>
    /// <param name="content"></param>
    /// <param name="name"></param>
    /// <param name="fileName"></param>
    public FakeFormFile(Stream content, string name, string fileName)
        : this(content, name, fileName, null)
    {
    }

    /// <summary>
    /// Create a form file
    /// </summary>
    /// <param name="content"></param>
    /// <param name="name"></param>
    /// <param name="fileName"></param>
    /// <param name="mimeType"></param>
    public FakeFormFile(
        Stream content,
        string name,
        string fileName,
        string mimeType
    )
    {
        _content = content;
        Name = name;
        FileName = fileName ?? name;
        ContentType = mimeType ?? MIMEType.GuessForFileName(FileName);
    }


    /// <inheritdoc />
    public Stream OpenReadStream()
    {
        // provide a new stream so that disposal
        // doesn't trash _content
        return new MemoryStream(
            _content.ReadAllBytes()
        );
    }

    /// <inheritdoc />
    public void CopyTo(Stream target)
    {
        _content.Position = 0;
        _content.CopyTo(target);
    }

    /// <inheritdoc />
    public Task CopyToAsync(
        Stream target,
        CancellationToken cancellationToken = new()
    )
    {
        return _content.CopyToAsync(target);
    }

    /// <inheritdoc />
    public string ContentType { get; set; }

    /// <inheritdoc />
    public string ContentDisposition { get; set; }

    /// <inheritdoc />
    public IHeaderDictionary Headers
    {
        get => _headers ??= new FakeHeaderDictionary();
        set => _headers = value ?? new FakeHeaderDictionary();
    }

    private IHeaderDictionary _headers;

    /// <inheritdoc />
    public long Length => _content.Length;

    /// <inheritdoc />
    public string Name { get; set; }

    /// <inheritdoc />
    public string FileName { get; set; }

    /// <summary>
    /// Sets the content for the file (overwrites)
    /// </summary>
    /// <param name="stream"></param>
    public void SetContent(Stream stream)
    {
        _content = stream;
    }
}