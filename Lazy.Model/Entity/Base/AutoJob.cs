using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Lazy.Model.Entity.Base
{
    public class AutoJob : BaseEntityWithSoftDelete
    {
        [MaxLength(50)]
        public string JobGroupName { get; set; }

        [MaxLength(50)]
        public string JobName { get; set; }

        public int? JobStatus { get; set; }

        [MaxLength(50)]
        public string CronExpression { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime? NextStartTime { get; set; }

        public string Remark { get; set; }
    }
}
