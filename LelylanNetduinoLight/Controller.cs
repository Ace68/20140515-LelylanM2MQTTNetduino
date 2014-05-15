#region using
using System;
using System.Net;
using System.Text;

using LelylanNetduinoLight.Core;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using SecretLabs.NETMF.Hardware.Netduino;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
#endregion

namespace LelylanNetduinoLight
{
    public class Controller
    {
        #region private properties
        private readonly byte[] _qosLevels = { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE };

        private static string _clientId;
        private static MqttClient _mqttClient;

        private const bool currentState = false;
        private bool previousState = false;
        private bool readState;
        #endregion

        private OutputPort _ledOnBoard;

        public Controller()
        {
            // Init Client ID
            _clientId = Guid.NewGuid().ToString();
            if (_clientId.Length > 20)
                _clientId = _clientId.Substring(_clientId.Length - 20, 20);

            // create client instance 
            _mqttClient = new MqttClient(IPAddress.Parse(LelylanCore.MqttBrokerAddress));
        }

        public void RunLightMonitor()
        {
            // Get onboard led reference
            this._ledOnBoard = new OutputPort(Pins.ONBOARD_LED, false);

            while (true)
            {
                LelylanConnection();

                // read onboard led status
                bool ledStatus = this._ledOnBoard.Read();

                // publis led status on server lelylan
                if (readState != previousState)
                {
                    LelylanPublish(currentState ? "on" : "off");
                }

                previousState = readState;
            }
        }

        #region lelylan Comunication
        /// <summary>
        /// MQTT lelylan server connection
        /// </summary>
        private void LelylanConnection()
        {
            if (_mqttClient.IsConnected) return;

            // register to message publish/subscribe
            _mqttClient.MqttMsgSubscribed += _mqttClient_MqttMsgSubscribed;
            _mqttClient.MqttMsgUnsubscribed += _mqttClient_MqttMsgUnsubscribed;

            _mqttClient.MqttMsgPublishReceived += _mqttClient_MqttMsgPublishReceived;

            _mqttClient.MqttMsgPublished += _mqttClient_MqttMsgPublished;

            _mqttClient.MqttMsgDisconnected += _mqttClient_MqttMsgDisconnected;

            // connection to lelylan MQTT server
            int connectionResult = _mqttClient.Connect(_clientId, LelylanCore.DeviceId, LelylanCore.DeviceSecret);
            //if (_mqttClient.Connect(_clientId, LelylanCore.DeviceId, LelylanCore.DeviceSecret) != 0) return;

            if (connectionResult != 0) return;

            this.LelylanSubscribe();
            LelylanPublish("off");
        }

        private void LelylanSubscribe()
        {
            _mqttClient.Subscribe(LelylanCore._inTopic, _qosLevels);
        }

        private static void LelylanPublish(string command)
        {
            _mqttClient.Publish(LelylanCore._outTopic,
                command == "on" ? GetBytes(LelylanCore.PayloadOn) : GetBytes(LelylanCore.PayloadOff), 1, true);
        }

        private static byte[] GetBytes(string stringToConvert)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(stringToConvert);
        }

        #region M2MQTT event
        private static void _mqttClient_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            
        }

        private static void _mqttClient_MqttMsgUnsubscribed(object sender, MqttMsgUnsubscribedEventArgs e)
        {

        }

        private static void _mqttClient_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
        }

        private void _mqttClient_MqttMsgDisconnected(object sender, EventArgs e)
        {
        }

        private void _mqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var messageString = new String(Encoding.UTF8.GetChars(e.Message));

            this._ledOnBoard.Write(messageString.IndexOf("on") > 0);

            LelylanPublish(messageString.IndexOf("on") > 0 ? "on" : "off");
        }
        #endregion
        #endregion
    }
}
