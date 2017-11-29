using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Schema;
using System.Linq;

namespace AutomaticSwitchTimer.ResponseDTO
{
    public class EventDTO
    {
        public string type { get; set; }
        public string event_type { get; set; }
        public int id { get; set; }
    }

    public class LightChangeAttributes
    {
        public int receivedDups { get; set; }
        public string receivedTS { get; set; }
        public List<string> capabilities { get; set; }
        public bool hidden { get; set; }
        public bool is_zwave_plus { get; set; }
        public string manufacturer_name { get; set; }
        public string friendly_name { get; set; }
        public bool is_failed { get; set; }
        public string query_stage { get; set; }
        public int lastRequestRTT { get; set; }
        public bool is_info_received { get; set; }
        public int max_baud_rate { get; set; }
        public int sentCnt { get; set; }
        public int receivedUnsolicited { get; set; }
        public int averageRequestRTT { get; set; }
        public int retries { get; set; }
        public int averageResponseRTT { get; set; }
        public int node_id { get; set; }
        public int lastResponseRTT { get; set; }
        public int receivedCnt { get; set; }
        public string product_name { get; set; }
        public List<int> neighbors { get; set; }
        public int sentFailed { get; set; }
        public bool is_ready { get; set; }
        public bool is_awake { get; set; }
        public string sentTS { get; set; }
    }

    public class LightChangeOldState
    {
        public DateTime last_changed { get; set; }
        public LightChangeAttributes attributes { get; set; }
        public string state { get; set; }
        public DateTime last_updated { get; set; }
        public string entity_id { get; set; }
    }

    public class LightChangeAttributes2
    {
        public int receivedDups { get; set; }
        public string receivedTS { get; set; }
        public List<string> capabilities { get; set; }
        public bool hidden { get; set; }
        public bool is_zwave_plus { get; set; }
        public string manufacturer_name { get; set; }
        public string friendly_name { get; set; }
        public bool is_failed { get; set; }
        public string query_stage { get; set; }
        public int lastRequestRTT { get; set; }
        public bool is_info_received { get; set; }
        public int max_baud_rate { get; set; }
        public int sentCnt { get; set; }
        public int receivedUnsolicited { get; set; }
        public int averageRequestRTT { get; set; }
        public int retries { get; set; }
        public int averageResponseRTT { get; set; }
        public int node_id { get; set; }
        public int lastResponseRTT { get; set; }
        public int receivedCnt { get; set; }
        public string product_name { get; set; }
        public List<int> neighbors { get; set; }
        public int sentFailed { get; set; }
        public bool is_ready { get; set; }
        public bool is_awake { get; set; }
        public string sentTS { get; set; }
    }

    public class LightChangeNewState
    {
        public DateTime last_changed { get; set; }
        public LightChangeAttributes2 attributes { get; set; }
        public string state { get; set; }
        public DateTime last_updated { get; set; }
        public string entity_id { get; set; }
    }

    public class LightChangeData
    {
        public string entity_id { get; set; }
        public LightChangeOldState old_state { get; set; }
        public LightChangeNewState new_state { get; set; }
    }

    public class LightChangeEvent
    {
        public DateTime time_fired { get; set; }
        public LightChangeData data { get; set; }
        public string origin { get; set; }
        public string event_type { get; set; }
    }

    public class LightChangedWrapper
    {
        public int id { get; set; }
        public LightChangeEvent @event { get; set; }
        public string type { get; set; }
    }

    //TODO: Rename THIS PLEASE
    public enum EventType
    {
        InitialAuthenticationResponseNoPasswordNeeded,
        InitialAuthenticationResponsePasswordNeeded,
        ResultResponse,
        SwitchChangedEvent,
        NotFound
    }
    //RENAME THIS BBBBPPPLLLAAAEESESS
    public static class WhatEventPassedIn
    {
        public static EventType TypeOfObject(JObject toTest)
        {
            //TODO: Write this out to not use the JSchemaGenerator
            var result = EventType.NotFound;
            if (toTest["type"] != null &&
                toTest["type"].Value<string>() == "auth_ok")
            {
                result = EventType.InitialAuthenticationResponseNoPasswordNeeded;
            }
            else if (toTest["typa"] != null &&
                toTest["type"].Value<string>() == "auth_required")
            {
                result = EventType.InitialAuthenticationResponsePasswordNeeded;
            }
            if (2 == 3)
                result = EventType.SwitchChangedEvent;
            if (1 == 2)
                result = EventType.InitialAuthenticationResponseNoPasswordNeeded;
            return result;
        }
    }
}
