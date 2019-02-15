using System;
using SecurityAdvisor.Infrastructure.Generic;
using static SecurityAdvisor.Infrastructure.Generic.UniversalStringMethods;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class FalseTimeDT : DetectionTechnique
    {
        private const int DAYS_AMOUNT_FOR_TIMES_DIFFERENCE = 1;
        private const string URI_WITH_TIME = @"http://worldclockapi.com/api/jsonp/cet/now?callback=mycallback";

        //Обнаружение происходит путём сверки времени в системе с временем из удалённого источника, если отличается более, чем на 24 часа (пояс не используется), то время
        //установлено не верно.
        //Если нет доступа в Интернет, то время берём локальное. Если время в системе не верное, то выбираем время из Интернета.
        public override void Execute()
        {
            DB db = DB.Load();
            DateTime timeInOS = DateTime.Now;
            DateTime onlineTime;

            try
            {
                onlineTime = GetTimeFromInternet(); //Central Europe Standard Time
            }
            catch (WebException e)
            {
                db.ActualTime = timeInOS;
                Status = DetectionStatus.Error;
                return;
            }

            TimeSpan timesDelta = new TimeSpan(Math.Abs(timeInOS.Ticks - onlineTime.Ticks));
            if (IsDifferentTimes())
            {
                Status = DetectionStatus.Found;
                db.ActualTime = onlineTime;
            }
            else
            {
                Status = DetectionStatus.NotFound;
                db.ActualTime = timeInOS;
            }

            bool IsDifferentTimes() => timesDelta.Days >= DAYS_AMOUNT_FOR_TIMES_DIFFERENCE;

        }

        private DateTime GetTimeFromInternet()
        {
            WebClient web = new WebClient { Proxy = null };

            string json = web.DownloadString(URI_WITH_TIME);
            int referencePoint = json.IndexOf('+'); //Ищем в mycallback({"$id":"1","currentDateTime":"2019-02-14T16:24+01:00","u...
            string onlineTime = GetSubstringByReferencePoint(referencePoint, json, 16, 10) + " " + GetSubstringByReferencePoint(referencePoint, json, 5, 5);
            DateTime onlineTime_DT = DateTime.ParseExact(onlineTime, "yyyy-MM-dd HH:mm", null);
            
            return onlineTime_DT;

        }

        
    }
}
