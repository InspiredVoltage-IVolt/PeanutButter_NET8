﻿using System.IO;
using System.Net;
using Microsoft.AspNetCore.Http;
using PeanutButter.Utils;

namespace PeanutButter.TestUtils.AspNetCore.Fakes;

public class UrlEncodedBodyEncoder : IFormEncoder
{
    public Stream Encode(IFormCollection form)
    {
        var result = new MemoryStream();
        if (form is null)
        {
            return result;
        }

        var isFirst = true;
        foreach (var key in form.Keys)
        {
            if (!isFirst)
            {
                result.AppendString("&");
            }

            isFirst = false;

            result.AppendString($"{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(form[key])}");
        }

        result.Position = 0;
        return result;
    }
}