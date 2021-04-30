using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasqR.TestProject.Microsoft.DependencyInjection.Test3
{
    public class TrackMeInTransient
    {
        public Guid ID { get; set; }
    }

    public class TrackMeInSingleton
    {
        public Guid ID { get; set; }
    }

    public class TrackMeInScope
    {
        public Guid ID { get; set; }
    }

    public class DINestedCmd1 : ITasq<Dictionary<string, Guid>>
    {

    }

    public class DINestedCmd2 : ITasq<Dictionary<string, Guid>>
    {

    }

    public class DINestedCmd3 : ITasq<Dictionary<string, Guid>>
    {

    }



    public class DINestedCmd1Handler : TasqHandler<DINestedCmd1, Dictionary<string, Guid>>
    {
        public readonly ITasqR p_TasqR;
        public readonly TrackMeInScope p_TrackMe;

        public DINestedCmd1Handler(ITasqR tasqR, TrackMeInScope trackMe)
        {
            p_TasqR = tasqR;
            p_TrackMe = trackMe;
        }

        public override Dictionary<string, Guid> Run(DINestedCmd1 request)
        {
            Dictionary<string, Guid> retVal = new Dictionary<string, Guid>();

            var data = p_TasqR.Run(new DINestedCmd2());

            retVal[$"{nameof(DINestedCmd1Handler)}_{nameof(p_TasqR)}"] = p_TasqR.ID;
            retVal[$"{nameof(DINestedCmd1Handler)}_{nameof(p_TrackMe)}"] = p_TrackMe.ID;

            foreach (var key in data.Keys)
            {
                retVal[key] = data[key];
            }

            return retVal;
        }
    }

    public class DINestedCmd2Handler : TasqHandler<DINestedCmd2, Dictionary<string, Guid>>
    {
        public readonly ITasqR p_TasqR;
        public readonly TrackMeInScope p_TrackMe;

        public DINestedCmd2Handler(ITasqR tasqR, TrackMeInScope trackMe)
        {
            p_TasqR = tasqR;
            p_TrackMe = trackMe;
        }

        public override Dictionary<string, Guid> Run(DINestedCmd2 request)
        {
            Dictionary<string, Guid> retVal = new Dictionary<string, Guid>();

            var data = p_TasqR.Run(new DINestedCmd3());

            retVal[$"{nameof(DINestedCmd2Handler)}_{nameof(p_TasqR)}"] = p_TasqR.ID;
            retVal[$"{nameof(DINestedCmd2Handler)}_{nameof(p_TrackMe)}"] = p_TrackMe.ID;

            foreach (var key in data.Keys)
            {
                retVal[key] = data[key];
            }

            return retVal;
        }
    }

    public class DINestedCmd3Handler : TasqHandler<DINestedCmd3, Dictionary<string, Guid>>
    {
        public readonly ITasqR p_TasqR;
        public readonly TrackMeInScope p_TrackMe;

        public DINestedCmd3Handler(ITasqR tasqR, TrackMeInScope trackMe)
        {
            p_TasqR = tasqR;
            p_TrackMe = trackMe;
        }

        public override Dictionary<string, Guid> Run(DINestedCmd3 request)
        {
            Dictionary<string, Guid> retVal = new Dictionary<string, Guid>();

            retVal[$"{nameof(DINestedCmd3Handler)}_{nameof(p_TasqR)}"] = p_TasqR.ID;
            retVal[$"{nameof(DINestedCmd3Handler)}_{nameof(p_TrackMe)}"] = p_TrackMe.ID;

            return retVal;
        }
    }
}
