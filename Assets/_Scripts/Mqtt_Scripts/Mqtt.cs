using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;

public class Mqtt : Singleton<Mqtt>
{
    public class MQTTMessage
    {
        public string topic;
        public string message;
    }

    private MqttClient mqttClient;
    private Dictionary<string, string> dic = new Dictionary<string, string>();

    Queue<MQTTMessage> messageQueue = new Queue<MQTTMessage>();

    public void Connect(string uri, string[] topics)
    {
        mqttClient = new MqttClient(uri, 1883, false, null);

        mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

        string clientId = Guid.NewGuid().ToString();
        mqttClient.Connect(clientId);
        Debug.Log("MQTT IsConnected: " + mqttClient.IsConnected);
        foreach (string topic in topics)
        {
            mqttClient.Subscribe(
                new string[] { topic },
                new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }
    }

    public bool HasGotMessage
    {
        get { return messageQueue.Count > 0; }
    }

    public MQTTMessage GetCurrentMessage()
    {
        return messageQueue.Dequeue();
    }

    public void Send(string topic, string message)
    {
        Debug.Log("[Send]Topic: " + topic + ", Message: " + message);
        try
        {
            mqttClient.Publish(topic, Encoding.UTF8.GetBytes(message));
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string topic = e.Topic;
        string message = System.Text.Encoding.UTF8.GetString(e.Message);
        //Debug.Log("Topic: " + topic + ", Message: " + message);
        //dic.Add(topic, message);
        messageQueue.Enqueue(new MQTTMessage() { topic = topic, message = message });
        //MQTTMessage newMessage = new MQTTMessage() { topic = topic, message = message };
    }
}