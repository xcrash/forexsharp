﻿using Quartz;
using System.Collections.Concurrent;

namespace FXSharp.EA.NewsBox
{
    class MagicBoxScheduler
    {
        private IScheduler scheduler;
        private ConcurrentQueue<MagicBoxOrder> orderQueue;

        public MagicBoxScheduler(IScheduler scheduler, ConcurrentQueue<MagicBoxOrder> orderQueue)
        {
            this.scheduler = scheduler;
            this.orderQueue = orderQueue;
        }

        public void Schedule(MagicBoxOrder magicBox)
        {
            var nexttime = magicBox.ExecutingTime;

            var jobDetail = JobBuilder.Create<MagicBoxOrderJob>()
                .WithIdentity(magicBox.Id, "group1")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(magicBox.Id, "group1")
                .StartAt(DateBuilder.TodayAt(nexttime.Hour, nexttime.Minute, nexttime.Second))
                .Build();

            // should group this together in one command. just execute when arrived there

            jobDetail.JobDataMap.Add("queue", orderQueue);
            jobDetail.JobDataMap.Add("orders", magicBox);

            scheduler.ScheduleJob(jobDetail, trigger);
        }

    }
}
