﻿using SecurityAdvisor.Infrastructure.Generic;
using System;
using System.Net;
using System.Text.RegularExpressions;
using static SecurityAdvisor.Infrastructure.Generic.UniversalMethods;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class FalseTimeDT : DetectionTechnique
    {
        private const int HOURS_AMOUNT_FOR_TIMES_DIFFERENCE = 12;
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
                Exceptions.PrintInfoAboutException(e);
                return;
            }

            double timesDeltaInDays = CalcTimesDifferenceInHours(timeInOS, onlineTime);
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

            bool IsDifferentTimes() => timesDeltaInDays > HOURS_AMOUNT_FOR_TIMES_DIFFERENCE;

        }

        private DateTime GetTimeFromInternet()
        {
            WebClient web = new WebClient { Proxy = null };

            string json = web.DownloadString(URI_WITH_TIME);
            Match onlineTimeMatch = Regex.Match(json, @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}"); //Ищем в mycallback({"$id":"1","currentDateTime":"2019-02-14T16:24+01:00","u...
            if (!onlineTimeMatch.Success)
                throw new Exception("Регулярное выражение не обнаружило дату и время в json-е");
            string onlineTime = onlineTimeMatch.Value.Replace('T',' ');
            DateTime onlineTime_DT = DateTime.ParseExact(onlineTime, "yyyy-MM-dd HH:mm", null);
            
            return onlineTime_DT;

        }

        
    }
}
