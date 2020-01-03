using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.Ultilities
{
    public class Constants
    {
        public const String REDIS_PS_EMAIL_CHANNEL = "redis_email_channel";
        public const String REDIS_PS_USER_COUNT_PRODUCT_PREFIX_CHANNEL = "redis_viewer_count_product_id_";

        public const String CACHE_PRODUCT_CURRENT_VIEWING_PREFIX = "cache_current_visitor_product_id";
        public const String CACHE_PRODUCT_REVIEW_POINT_PREFIX = "cache_review_start_point_product_id";
        public const String CACHE_PRODUCT_COUNT_NUMBER_REIVEW_PREFIX = "cache_count_number_review_product_id";
    }
}
