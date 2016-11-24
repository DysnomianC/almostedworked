using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.MobileServices;

namespace Almost_worked
{
    public class Timeline
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "baseCurrency")]
        public string BaseCurrency { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        [JsonProperty(PropertyName = "full")]
        public bool Full { get; set; }

        /*[JsonProperty(PropertyName = "disgust")]
        public double Disgust { get; set; }

        [JsonProperty(PropertyName = "fear")]
        public double Fear { get; set; }

        [JsonProperty(PropertyName = "happiness")]
        public double Happiness { get; set; }

        [JsonProperty(PropertyName = "neutral")]
        public double Neutral { get; set; }

        [JsonProperty(PropertyName = "sadness")]
        public double Sadness { get; set; }

        [JsonProperty(PropertyName = "surprise")]
        public double Surprise { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public DateTime Date { get; set; }*/
    }
}