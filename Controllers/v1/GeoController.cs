using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreApiExample.Domain;
using AspNetCoreApiExample.Infrastructure;
using AspNetCoreApiExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Distance;

namespace AspNetCoreApiExample.Controllers.v1
{
    public class GeoController : BaseController
    {
        private readonly MyDBContext _dbContext;

        public GeoController(MyDBContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet]
        public async Task<AppResult> Get()
        {
            var r = await _dbContext.GeoHistories.Where(x => x.UserId == CurrentUserId)
                .ToArrayAsync();

            return SuccessfullMessage(r);
        }

        [HttpPost("[action]")]
        public async Task<AppResult> Distance([FromBody] PositionModel model)
        {
            if (ModelState.IsValid == false) return ErrorMessage("error validation ");

            if (model.FromLat == null || model.FromLng == null ||
                model.DistLat == null || model.DistLng == null)
                return ErrorMessage("error . enter 2 point latlng ");

            var d = new DistanceOp(
                new Point(model.FromLat.Value, model.FromLng.Value),
                new Point(model.DistLat.Value, model.DistLng.Value));
            var distance = d.Distance();

            var histories = new List<GeoHistory>();
            _dbContext.GeoHistories.Add(new GeoHistory
            {
                Date = DateTime.UtcNow,
                FromLng = model.FromLng ?? 0,
                FromLat = model.FromLat ?? 0,
                DistLat = model.DistLat ?? 0,
                DistLng = model.DistLng ?? 0,
                UserId = CurrentUserId,
                Distance = distance
            });
            await _dbContext.SaveChangesAsync();
            return SuccessfullMessage(distance);
        }
    }
}