using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.DI
{
    public interface IRedis
    {
        public void Init();
        bool Status();
        public void Subscribe(string _channel, Action<RedisChannel, RedisValue> _handler);
        public long Publish(string _channel, string _message);
        public void Unsubscribe(string _channel);

        public void SetValue(string key, string value);
        public string GetValue(string key, string defaultValue);
        public long IncreaseValue(string key);
    }
}
