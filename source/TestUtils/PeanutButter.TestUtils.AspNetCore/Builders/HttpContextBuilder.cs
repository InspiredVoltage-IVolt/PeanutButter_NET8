﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PeanutButter.TestUtils.AspNetCore.Fakes;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace PeanutButter.TestUtils.AspNetCore.Builders;

/// <summary>
/// Builds an HttpContext
/// </summary>
public class HttpContextBuilder : RandomizableBuilder<HttpContextBuilder, HttpContext>
{
    /// <summary>
    /// Constructs the fake http context
    /// </summary>
    /// <returns></returns>
    protected override HttpContext ConstructEntity()
    {
        return new FakeHttpContext();
    }

    /// <summary>
    /// Randomizes the context
    /// </summary>
    /// <returns></returns>
    public override HttpContextBuilder Randomize()
    {
        return WithRequest(
            () => HttpRequestBuilder.CreateWithNoHttpContext()
                .Randomize()
                .Build()
        ).WithResponse(
            HttpResponseBuilder.CreateWithNoHttpContext()
                .Randomize()
                .Build()
        );
    }

    /// <summary>
    /// Constructs the builder
    /// </summary>
    public HttpContextBuilder() : base(Actualize)
    {
        WithFeatures(new FakeFeatureCollection())
            .WithFeature<IHttpResponseFeature>(
                ctx => new FakeHttpResponseFeature(() => ctx.Response)
            )
            .WithFeature<IHttpRequestFeature>(
                ctx => new FakeHttpRequestFeature(() => ctx.Request)
            )
            .WithResponse(
                () => HttpResponseBuilder.CreateWithNoHttpContext()
                    .Build()
            )
            .WithConnection(GetRandom<FakeConnectionInfo>())
            .WithUser(GetRandom<ClaimsPrincipal>())
            .WithRequestServices(new MinimalServiceProvider())
            .WithRequest(
                () => HttpRequestBuilder.CreateWithNoHttpContext()
                    .Build()
            ).WithResponse(
                () => HttpResponseBuilder.CreateWithNoHttpContext()
                    .Build()
            );
    }

    private static void Actualize(HttpContext context)
    {
        WarnIf(context.Response is null, "context.Response is null");
        WarnIf(context.Response?.HttpContext is null, "context.Response.HttpContext is null");
        WarnIf(context.Request is null, "context.Request is null");
        WarnIf(context.Request?.HttpContext is null, "context.Request.HttpContext is null");
    }

    /// <summary>
    /// Sets the RequestServices service provider for the context
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public HttpContextBuilder WithRequestServices(IServiceProvider serviceProvider)
    {
        return With(
            o => o.RequestServices = serviceProvider
        );
    }

    /// <summary>
    /// Sets an Item on the context
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public HttpContextBuilder WithItem(object key, object value)
    {
        return With(
            o => o.Items[key] = value
        );
    }

    /// <summary>
    /// Sets the WebSockets manager on the request
    /// </summary>
    /// <param name="webSocketManager"></param>
    /// <returns></returns>
    public HttpContextBuilder WithWebSockets(WebSocketManager webSocketManager)
    {
        return With<FakeHttpContext>(
            o => o.SetWebSockets(webSocketManager)
        );
    }

    /// <summary>
    /// Adds a feature to the request
    /// </summary>
    /// <param name="feature"></param>
    /// <typeparam name="TFeature"></typeparam>
    /// <returns></returns>
    public HttpContextBuilder WithFeature<TFeature>(TFeature feature)
    {
        return With(
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            o => o.Features.Set<TFeature>(feature)
        );
    }

    /// <summary>
    /// Set a feature via a factory which has access to the HttpContext
    /// </summary>
    /// <param name="factory"></param>
    /// <typeparam name="TFeature"></typeparam>
    /// <returns></returns>
    public HttpContextBuilder WithFeature<TFeature>(Func<HttpContext, TFeature> factory)
    {
        return With(
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            o =>
            {
                if (o.Features is FakeFeatureCollection fake)
                {
                    fake.SetFactory<TFeature>(() => factory(o));
                }
                else
                {
                    // ReSharper disable once RedundantTypeArgumentsOfMethod
                    o.Features.Set<TFeature>(factory(o));
                }
            }
        );
    }

    /// <summary>
    /// Sets the user on the request
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public HttpContextBuilder WithUser(ClaimsPrincipal principal)
    {
        return With(
            o => o.User = principal
        );
    }

    /// <summary>
    /// Sets the connection info on the request
    /// </summary>
    /// <param name="connectionInfo"></param>
    /// <returns></returns>
    public HttpContextBuilder WithConnection(
        ConnectionInfo connectionInfo
    )
    {
        return With<FakeHttpContext>(
            o => o.SetConnection(connectionInfo)
        );
    }

    /// <summary>
    /// Sets the response on the context
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    public HttpContextBuilder WithResponse(
        HttpResponse response
    )
    {
        return WithResponse(() => response);
    }

    /// <summary>
    /// Sets the response accessor on the context
    /// </summary>
    /// <param name="accessor"></param>
    /// <returns></returns>
    public HttpContextBuilder WithResponse(
        Func<HttpResponse> accessor
    )
    {
        return With<FakeHttpContext>(
            o => o.SetResponseAccessor(accessor),
            nameof(FakeHttpContext.Response)
        );
    }

    /// <summary>
    /// Sets the request on the context
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public HttpContextBuilder WithRequest(
        HttpRequest request
    )
    {
        return WithRequest(() => request);
    }

    /// <summary>
    /// Sets the request accessor on the context
    /// </summary>
    /// <param name="requestAccessor"></param>
    /// <returns></returns>
    public HttpContextBuilder WithRequest(
        Func<HttpRequest> requestAccessor
    )
    {
        return With<FakeHttpContext>(
            o => o.SetRequestAccessor(requestAccessor),
            nameof(FakeHttpContext.Request)
        );
    }

    /// <summary>
    /// Sets the feature collection on the request
    /// </summary>
    /// <param name="features"></param>
    /// <returns></returns>
    public HttpContextBuilder WithFeatures(
        IFeatureCollection features
    )
    {
        return With<FakeHttpContext>(
            o => o.SetFeatures(features ?? new FakeFeatureCollection())
        );
    }

    /// <summary>
    /// Adds a form file to the context
    /// </summary>
    /// <param name="content"></param>
    /// <param name="name"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public HttpContextBuilder WithFormFile(
        string content,
        string name,
        string fileName = null
    )
    {
        return WithRequestMutator(
            o =>
            {
                var request = o.As<FakeHttpRequest>();
                var form = request.Form.As<FakeFormCollection>();
                var files = form.Files.As<FakeFormFileCollection>();
                files.Add(new FakeFormFile(content, name, fileName));
            });
    }


    /// <summary>
    /// Adds a form field to the request
    /// Use this when what you want to do can be accomplished
    /// on the proper IFormCollection type
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public HttpContextBuilder WithFormField(
        string key,
        string value
    )
    {
        return WithFakeRequestFormModifier(
            o => o[key] = value
        );
    }

    /// <summary>
    /// Modifies some aspect of the request form.
    /// Use this when you want access to the functionality of
    /// the underlying fake.
    /// </summary>
    /// <param name="modifier"></param>
    /// <returns></returns>
    public HttpContextBuilder WithFakeRequestFormModifier(
        Action<FakeFormCollection> modifier
    )
    {
        return WithRequestMutator(
            req => modifier(req.Form.As<FakeFormCollection>())
        );
    }

    /// <summary>
    /// Modifies some aspect of the request.
    /// Use this when what you want to do can be achieved
    /// on the proper HttpRequest type
    /// </summary>
    /// <param name="mutator"></param>
    /// <returns></returns>
    public HttpContextBuilder WithRequestMutator(
        Action<HttpRequest> mutator
    )
    {
        return With(
            o => mutator(o.Request)
        );
    }

    /// <summary>
    /// Modifies some aspect of the response.
    /// Use this when what you want to do can be achieved
    /// on the proper HttpResponse type
    /// </summary>
    /// <param name="mutator"></param>
    /// <returns></returns>
    public HttpContextBuilder WithResponseMutator(
        Action<HttpResponse> mutator
    )
    {
        return With(
            o => mutator(o.Response)
        );
    }

    /// <summary>
    /// Modifies some aspect of the request.
    /// Use this when you want access to the functionality of
    /// the underlying fake.
    /// </summary>
    /// <param name="modifier"></param>
    /// <returns></returns>
    public HttpContextBuilder WithFakeRequestModifier(
        Action<FakeHttpRequest> modifier
    )
    {
        return With(
            o => modifier(o.Request.As<FakeHttpRequest>())
        );
    }

    /// <summary>
    /// Set the full url for the request
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public HttpContextBuilder WithUrl(
        Uri uri
    )
    {
        return With(
            o =>
            {
                o.Request.Scheme = uri.Scheme;
                o.Request.Host = IsDefaultPortForScheme(
                    uri.Scheme,
                    uri.Port
                )
                    ? new HostString(uri.Host)
                    : new HostString(uri.Host, uri.Port);
                o.Request.Path = uri.AbsolutePath;
                o.Request.QueryString = new QueryString(uri.Query);
            }
        );
    }

    /// <summary>
    /// Set the full url for the request
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public HttpContextBuilder WithUrl(
        string url
    )
    {
        return WithUrl(new Uri(url));
    }

    private bool IsDefaultPortForScheme(string uriScheme, int uriPort)
    {
        return DefaultPortsPerScheme.TryGetValue(uriScheme, out var defaultPort)
            && defaultPort == uriPort;
    }

    private static readonly Dictionary<string, int> DefaultPortsPerScheme = new(StringComparer.OrdinalIgnoreCase)
    {
        ["http"] = 80,
        ["https"] = 443
    };

    /// <summary>
    /// Register a transient service
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <returns></returns>
    public HttpContextBuilder WithTransientService<TService, TImplementation>()
        where TImplementation : TService
    {
        return WithServicesMutator(provider => provider.Register<TService, TImplementation>());
    }

    /// <summary>
    /// Register a singleton service
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <returns></returns>
    public HttpContextBuilder WithSingletonService<TService, TImplementation>()
        where TImplementation : TService
    {
        return WithServicesMutator(provider => provider.RegisterSingleton<TService, TImplementation>());
    }

    /// <summary>
    /// Register a service instance
    /// </summary>
    /// <param name="service"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    public HttpContextBuilder WithService<TService>(TService service)
    {
        // ReSharper disable once RedundantTypeArgumentsOfMethod
        return WithServicesMutator((_, provider) => provider.RegisterInstance<TService>(service));
    }

    /// <summary>
    /// Register a service factory
    /// </summary>
    /// <param name="factory"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    public HttpContextBuilder WithService<TService>(Func<HttpContext, TService> factory)
    {
        // ReSharper disable once RedundantTypeArgumentsOfMethod
        return WithServicesMutator((ctx, provider) => provider.Register<TService>(() => factory(ctx)));
    }

    /// <summary>
    /// Register a service factory
    /// </summary>
    /// <param name="factory"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    public HttpContextBuilder WithService<TService>(Func<TService> factory)
    {
        // ReSharper disable once RedundantTypeArgumentsOfMethod
        return WithServicesMutator(provider => provider.Register<TService>(() => factory()));
    }

    /// <summary>
    /// Open access to apply mutations to the MinimalServiceProvider, if not overridden.
    /// </summary>
    /// <param name="mutator"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public HttpContextBuilder WithServicesMutator(Action<HttpContext, MinimalServiceProvider> mutator)
    {
        return With(o =>
        {
            if (o.RequestServices is not MinimalServiceProvider provider)
            {
                throw new NotSupportedException(
                    $@"Only the {nameof(MinimalServiceProvider)} provider is supported for service registration
via builder methods. If you're providing your own RequestServices, you'll have to register elsewhere."
                );
            }

            mutator(o, provider);
        });
    }

    /// <summary>
    /// Open access to apply mutations to the MinimalServiceProvider, if not overridden.
    /// </summary>
    /// <param name="mutator"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public HttpContextBuilder WithServicesMutator(Action<MinimalServiceProvider> mutator)
    {
        return With(o =>
        {
            if (o.RequestServices is not MinimalServiceProvider provider)
            {
                throw new NotSupportedException(
                    $@"Only the {nameof(MinimalServiceProvider)} provider is supported for service registration
via builder methods. If you're providing your own RequestServices, you'll have to register elsewhere."
                );
            }

            mutator(provider);
        });
    }

    /// <summary>
    /// Set a request header
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public HttpContextBuilder WithRequestHeader(string key, string value)
    {
        return WithRequestMutator(req => req.Headers[key] = value);
    }

    /// <summary>
    /// Set a request cookie
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public HttpContextBuilder WithRequestCookie(string key, string value)
    {
        return WithRequestMutator(req => req.Cookies.As<FakeRequestCookieCollection>()[key] = value);
    }

    /// <summary>
    /// Set a response header
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public HttpContextBuilder WithResponseHeader(string key, string value)
    {
        return WithResponseMutator(res => res.Headers[key] = value);
    }
}
