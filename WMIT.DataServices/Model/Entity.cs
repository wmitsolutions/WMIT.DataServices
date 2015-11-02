using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMIT.DataServices.AutoValueVisitorStrategies;
using WMIT.DataServices.Common;
using WMIT.DataServices.Core;
using WMIT.DataServices.Security;
using WMIT.DataServices.Visitors;

namespace WMIT.DataServices.Model
{
    public class Entity : IEntity
    {
        public int Id { get; set; }

        [Access(On = EntityOperation.All, InternalUsage = true, ViolationBehavior = ViolationBehavior.IgnoreUserInput)]
        public bool IsDeleted { get; set; }

        [Access(On = EntityOperation.All, InternalUsage = true, ViolationBehavior = ViolationBehavior.IgnoreUserInput)]
        [AutoValue(On = EntityOperation.Insert, Strategy = typeof(CurrentUsernameStrategy))]
        public string CreatedBy { get; set; }

        [Access(On = EntityOperation.All, InternalUsage = true, ViolationBehavior = ViolationBehavior.IgnoreUserInput)]
        [AutoValue(On = EntityOperation.Insert, Strategy = typeof(DateTimeOffsetNowStrategy))]
        public DateTimeOffset CreatedAt { get; set; }

        [Access(On = EntityOperation.All, InternalUsage = true, ViolationBehavior = ViolationBehavior.IgnoreUserInput)]
        [AutoValue(On = EntityOperation.Update, Strategy = typeof(CurrentUsernameStrategy))]
        public string ModifiedBy { get; set; }

        [Access(On = EntityOperation.All, InternalUsage = true, ViolationBehavior = ViolationBehavior.IgnoreUserInput)]
        [AutoValue(On = EntityOperation.Update, Strategy = typeof(DateTimeOffsetNowStrategy))]
        public DateTimeOffset? ModifiedAt { get; set; }
    }
}
