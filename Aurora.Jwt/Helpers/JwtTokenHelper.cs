using System;
using System.Data.SqlTypes;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Utils.Helpers;

namespace Aurora.Jwt.Helpers;

/// <summary>
/// کلاس استاتیک برای تولید و مدیریت JWT Token
/// </summary>
public static class JwtTokenHelper
{
    extension(IHttpContextAccessor accessor)
    {
        public T GetUserId<T>()
        {
            var claim = accessor.GetClaims()
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            
            return claim is null 
                ? default 
                : claim.Value.ConvertTo<T>();
        }

        public T GetUser<T>()
        {
            var props = typeof(T).GetProperties();
            var instance = Activator.CreateInstance<T>();
            var claims = accessor.GetClaims();
            foreach (var prop in props)
            {
                var val = claims.FirstOrDefault(c => c.Type == prop.Name)?.Value;
                if (val is not null)
                    prop.SetValue(instance, Convert.ChangeType(val, prop.PropertyType));
            }
            return instance;
        }

        public IEnumerable<Claim> GetClaims()
            => accessor.HttpContext.User.Claims;

        public string GetClaim(string Key)
        {
            var claim = accessor.GetClaims()
                .FirstOrDefault(c => c.Type == Key);
            
            return claim?.Value;
        }

        public string GetAccessToken()
        {
            var header = accessor.HttpContext?.Request.Headers.FirstOrDefault(x=>x.Key=="Authorization").Value.ToString();
            if (string.IsNullOrEmpty(header) || !header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return string.Empty;
            return header["Bearer ".Length..].Trim();
        }
    }
}
