using ComputerAccessoriesV2.DI;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.Ultilities;
using Coravel.Invocable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.SchedulerTask
{
    public class UpdateProductVisitorCount : IInvocable
    {
        private readonly ComputerAccessoriesV2Context _db;
        private readonly IRedis _redis;

        public UpdateProductVisitorCount(ComputerAccessoriesV2Context db, IRedis redis)
        {
            _db = db;
            _redis = redis;
        }
        public Task Invoke()
        {
            _db.Products.ToList().ForEach(p =>
            {
                try
                {
                    var cacheVisitorCount = int.Parse(_redis.GetValue(Constants.CACHE_PRODUCT_CURRENT_VIEWING_PREFIX + p.Id, "0"));
                    if (cacheVisitorCount <= p.ViewCounts)
                    {
                        _redis.SetValue(Constants.CACHE_PRODUCT_CURRENT_VIEWING_PREFIX + p.Id, p.ViewCounts.ToString());
                    }
                    else
                    {
                        p.ViewCounts = cacheVisitorCount;
                        _db.SaveChanges();
                        _redis.Publish("notify", "Save done");
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            });
            return Task.CompletedTask;
        }
    }
}
