using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDNianBot
{
    public class JDTask
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 任务ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 任务等待时间
        /// </summary>
        public int WaitDuration { get; set; }
    }
}
