using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LoginPlatform : byte
{
    FromCached,
    Anonymous,
    Google,
    GooglePlay,
    Apple,
    AppleGame,
    Facebook,
    Steam,
    Oculus,
    Username,
    Local
}

public class LoginToken {}

public abstract class PlatformAuth
{
    public abstract LoginPlatform Platform { get; }
    public abstract Task SignUpAsync(LoginToken token);
    public abstract Task SignInAsync(LoginToken token);
    public abstract Task LinkAsync(LoginToken token);
}

public abstract class LoginHandler: IDisposable
{
    private Dictionary<LoginPlatform, PlatformAuth> _platformAuths = new();
    public IReadOnlyDictionary<LoginPlatform, PlatformAuth> PlatformAuths => _platformAuths;

    public abstract string GetUserId();
    public bool IsSessionUserExist() => !string.IsNullOrEmpty(GetUserId());
    public abstract Task InitAsync(CancellationToken token);
    public abstract Task<bool> TryLoginCachedUserAsync(CancellationToken token);
    
    public void RegisterPlatform(PlatformAuth platform)
    {
        if (!_platformAuths.TryAdd(platform.Platform, platform))
        {
            throw new ArgumentException($"Platform {platform.Platform} also registered");
        }
    }

    public Task SignInAsync(LoginPlatform platform, LoginToken token)
    {
        if (!_platformAuths.TryGetValue(platform, out var auth))
        {
            return Task.FromException(new KeyNotFoundException($"Platform {platform} not registered."));
        }

        return auth.SignInAsync(token);
    }
    
    public Task SignUpAsync(LoginPlatform platform, LoginToken token)
    {
        if (!_platformAuths.TryGetValue(platform, out var auth))
        {
            return Task.FromException(new KeyNotFoundException($"Platform {platform} not registered."));
        }

        return auth.SignUpAsync(token);
    }
    
    public Task LinkAsync(LoginPlatform platform, LoginToken token)
    {
        if (!_platformAuths.TryGetValue(platform, out var auth))
        {
            return Task.FromException(new KeyNotFoundException($"Platform {platform} not registered."));
        }

        return auth.LinkAsync(token);
    }

    public virtual void Dispose()
    {
        _platformAuths.Clear();
    }
}

public class LocalPlatform : PlatformAuth
{
    public override LoginPlatform Platform => LoginPlatform.Local;
    public override Task SignUpAsync(LoginToken token)
    {
        return Task.CompletedTask;
    }

    public override Task SignInAsync(LoginToken token)
    {
        return Task.CompletedTask;
    }

    public override Task LinkAsync(LoginToken token)
    {
        return Task.CompletedTask;
    }
}

public class LocalLoginHandler : LoginHandler
{
    public override string GetUserId()
    {
        return string.Empty;
    }

    public override Task InitAsync(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public override Task<bool> TryLoginCachedUserAsync(CancellationToken token)
    {
        return Task.FromResult(true);
    }
}
