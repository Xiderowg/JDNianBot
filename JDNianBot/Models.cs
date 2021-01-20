using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDNianBot
{
    public enum JDTaskType
    {
        Checkin = 0,
        Browse = 1,
        BeMember = 2,
        ShopSignin = 3,
        ShopLottery = 4,
        Unknown = 5
    }

    public class JDTask
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public JDTaskType Type { get; set; }
        /// <summary>
        /// 任务ID
        /// </summary>
        public int TaskID { get; set; }
        /// <summary>
        /// 事项ID
        /// </summary>
        public string[] ItemID { get; set; }
        /// <summary>
        /// 任务等待时间
        /// </summary>
        public int WaitDuration { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        public string ShopSign { get; set; }
    }
}
