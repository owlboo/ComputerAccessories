using StackExchange.Redis;
using System;

namespace ComputerAccessoriesV2.DI
{
    public class RedisImpl : IRedis
    {
        private const string RedisConnectionString = "35.194.1.21:6379,password=!@#)(*_-*&Ah1~,connectTimeout=15000";
        private ConnectionMultiplexer connection;
        private bool ConnectStatus = false;

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
                ConnectStatus = true;
            } catch (Exception e)
            {
                ConnectStatus = false;
            }
        }

        public long Publish(string _channel, string _message)
        {
            try
            {
                return connection.GetSubscriber().Publish(_channel, _message);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public void SetValue(string key, string value)
        {
            try
            {
                connection.GetDatabase().StringSet(key, value);
            }
            catch (Exception e)
            {
            
            }
        }

        public void Subscribe(string _channel, Action<RedisChannel, RedisValue> _handler)
        {
            try
            {
                connection.GetSubscriber().Subscribe(_channel, _handler);
            } catch (Exception e)
            {
                //
            }
        }

        public void Unsubscribe(string _channel)
        {
            try
            {
                connection.GetSubscriber().Unsubscribe(_channel);
            }
            catch (Exception e)
            {
                //
            }
        }

        public long IncreaseValue(string key)
        {
            try
            {
                return connection.GetDatabase().StringIncrement(key);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public bool Status()
        {
            return ConnectStatus;
        }
    }
}
