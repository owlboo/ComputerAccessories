using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.DI
{
    public class RedisImpl : IRedis
    {
        private const string RedisConnectionString = "35.194.1.21:6379,password=!@#)(*_-*&Ah1~";
        private ConnectionMultiplexer connection;
        
        public RedisImpl()
        {
            Init();
        }

        public string GetValue(string key, string defaultValue)
        {
            try
            {
                var result = connection.GetDatabase().StringGet(key);
                if (result.IsNullOrEmpty)
                {
                    return defaultValue;
                }
                else
                {
                    return result;
                }
            } catch (Exception e)
            {
                return defaultValue;
            }
            
        }

        public void Init()
        {
            try
            {
                connection = ConnectionMultiplexer.Connect(RedisConnectionString);
            }
            catch(Exception)
            {
                //eats
            }
        }

        public long Publish(string _channel, string _message)
        {
            try
            {
                return connection.GetSubscriber().Publish(_channel, _message);
            } catch(Exception e)
            {
                return 0;
            }
        }

        public void SetValue(string key, string value)
        {
            try
            {
                connection.GetDatabase().StringSet(key, value);
            } catch (Exception e)
            {
                //eat for now
            }
        }

        public void Subscribe(string _channel, Action<RedisChannel, RedisValue> _handler)
        {
            connection.GetSubscriber().Subscribe(_channel, _handler);
        }

        public void Unsubscribe(string _channel)
        {
            connection.GetSubscriber().Unsubscribe(_channel);
        }

        public IDatabase GetRedisBD()
        {
            if (connection == null) return null;
            else
            {
                return connection.GetDatabase();
            }
        }
    }
}
