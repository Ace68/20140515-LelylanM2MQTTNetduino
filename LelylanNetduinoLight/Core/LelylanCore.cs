namespace LelylanNetduinoLight.Core
{
    public static class LelylanCore
    {
        /* Device Credentials */
        /* set your device id (will be the MQTT client username) */
        public const string DeviceId = "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY";
        /* set your device secret (will be the MQTT client password) */
        public const string DeviceSecret = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";

        /* Sample payload published to lelylan */
        /* Status-id is the basic light status id (http://types.lelylan.com/types/518be107ef539711af000001) */
        // Physycal URI of Device http://nodes.lelylan.com/mqtt/devices/536a38e011cd8039f5000002

        public const string PayloadOn = @"{""properties"":[{""id"":""518be5a700045e1521000001"",""value"":""on""}]}";
        public const string PayloadOff = @"{""properties"":[{""id"":""518be5a700045e1521000001"",""value"":""off""}]}";

        /* MQTT Server IP Address */
        public const string MqttBrokerAddress = "96.126.109.170";

        public static readonly string[] _inTopic = { "devices/536a38e011cd8039f5000002/get" };         // where lelylan updates are received       (subscribe)
        public const string _outTopic = "devices/536a38e011cd8039f5000002/set";                        // where physical updates are published     (publish)
    }
}
