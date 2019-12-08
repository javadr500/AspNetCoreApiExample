using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreApiExample.Caching;
using AspNetCoreApiExample.Infrastructure;
using AspNetCoreApiExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NetTopologySuite.Geometries;

namespace AspNetCoreApiExample.Controllers.v1
{
    public class GeoController : BaseController
    {
        private readonly IMemoryCache _memoryCache;

        public GeoController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        private string _cashKey()
        {

            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var email = claimsIdentity.FindFirst(ClaimTypes.Email).Value;
                return $"{CacheKeys.Geo}_{email}";

            }

            return "";
        }



    [HttpGet]
    public async Task<AppResult> Get()
    {
        var histories = new List<GeoHistoryModel>();
        _memoryCache.TryGetValue(_cashKey(), out histories);


        return SuccessfullMessage(histories);
    }

    [HttpPost("[action]")]
    public async Task<AppResult> Distance([FromBody] PositionModel model)
    {
        if (ModelState.IsValid == false)
        {
            return ErrorMessage("error validation ");
        }

        if (model.FromLat == null || model.FromLng == null ||
            model.DistLat == null || model.DistLng == null)
        {
            return ErrorMessage("error . enter 2 point latlng ");
        }

        var d = new NetTopologySuite.Operation.Distance.DistanceOp(
            new Point(model.FromLat.Value, model.FromLng.Value),
            new Point(model.DistLat.Value, model.DistLng.Value));
        var distance = d.Distance();

        var histories = new List<GeoHistoryModel>();
        _memoryCache.TryGetValue(_cashKey(), out histories);
        if (histories == null)
            histories = new List<GeoHistoryModel>();

        histories.Add(new GeoHistoryModel()
        {
            Date = DateTime.UtcNow,
            FromLng = model.FromLng,
            FromLat = model.FromLat,
            DistLat = model.DistLat,
            DistLng = model.DistLng,
            Distance = distance
        });
        _memoryCache.Set(_cashKey(), histories);
        return SuccessfullMessage(distance);
    }

}
}