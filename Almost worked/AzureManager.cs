﻿using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Almost_worked
{
    public class AzureManager
    {
        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<Timeline> timelineTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("https://day3app.azurewebsites.net/");

            this.timelineTable = this.client.GetTable<Timeline>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task<List<Timeline>> GetTimelines()
        {
            return await this.timelineTable.ToListAsync();
        }

        public async Task AddTimeline(Timeline timeline)
        {
            await this.timelineTable.InsertAsync(timeline);
        }

        public async Task<Timeline> DelTimeline(String uid)
        {
            List<Timeline> timelines = await this.timelineTable.ToListAsync();
            foreach (Timeline t in timelines)
            {
                if (t.ID.Equals(uid))
                {
                    await this.timelineTable.DeleteAsync(t);
                    return t;
                }
            }
            return null;
        }
    }
}