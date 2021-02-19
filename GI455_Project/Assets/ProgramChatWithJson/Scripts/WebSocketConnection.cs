using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace ChatWebSocketWithJson
{
    public class WebSocketConnection : MonoBehaviour
    {
        class MessageData
        {
            public string username;
            public string message;            
        }

        struct SocketEvent
        {
            public string eventName;
            public string data;

            public SocketEvent(string eventName, string data)
            {
                this.eventName = eventName;
                this.data = data;
            }
        }

        public GameObject rootConnection;
        public GameObject rootMessenger;
        public GameObject rootCreateandJoin;
        public GameObject rootCreateRoom;
        public GameObject rootJoinroom;
        public GameObject rootPopUp;

        public InputField inputUsername;
        public InputField inputText;
        public InputField inputCrateRoom;
        public InputField inputJoinRoom;
        public Text sendText;
        public Text receiveText;       
        
        private WebSocket ws;

        private string tempMessageString;

        public void Start()
        {
            rootConnection.SetActive(true);
            rootMessenger.SetActive(false);
        }

        public void Connect()
        {
            string url = $"ws://127.0.0.1:25500/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();            

            //ใช้โชว์หน้าต่าง UI
            rootConnection.SetActive(false);
            rootCreateandJoin.SetActive(true);                        
        }

        public void CreateRoom()
        {
            if (ws.ReadyState == WebSocketState.Open && string.IsNullOrEmpty(inputCrateRoom.text) == false)
            {
                SocketEvent socketEvent = new SocketEvent("CreateRoom", inputCrateRoom.text);

                string jsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(jsonStr);
            }
        }        

        public void JoinRoom()
        {
            if(ws.ReadyState == WebSocketState.Open && string.IsNullOrEmpty(inputJoinRoom.text) == false)
            {
                SocketEvent socketEvent = new SocketEvent("JoinRoom", inputJoinRoom.text);

                string jsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(jsonStr);
            }
        }

        public void LeaveRoom()
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("LeaveRoom", "");

                string jsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(jsonStr);
            }
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }
        
        public void SendMessage()
        {
            if (inputText.text == "" || ws.ReadyState != WebSocketState.Open)
                return;

            MessageData messageData = new MessageData();
            messageData.username = inputUsername.text;
            messageData.message = inputText.text;

            string toJsonStr = JsonUtility.ToJson(messageData);            

            ws.Send(toJsonStr);
            inputText.text = "";
        }

        private void OnDestroy()
        {
            if (ws != null)
                ws.Close();
        }

        private void Update()
        {
            UpdateNotifyMessage();
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log(messageEventArgs.Data);
            tempMessageString = messageEventArgs.Data;
        }        

        public void UpdateNotifyMessage()
        {
            if (tempMessageString != null && tempMessageString != "")
            //if(string.IsNullOrEmpty(tempMessageString) == false) ใช้ได้เหมือนกัน
            {
                SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(tempMessageString);                
                if (receiveMessageData.eventName == "CreateRoom") 
                {                    
                    if (receiveMessageData.data == "fail")
                    {
                        //แสดง UI
                        rootPopUp.SetActive(true);
                    }
                    else
                    {
                        rootMessenger.SetActive(true);
                    }
                }

                if (receiveMessageData.eventName == "JoinRoom")
                {
                    if (receiveMessageData.data == "Connect")
                    {
                        //แสดง UI
                        rootMessenger.SetActive(true);
                    }
                    else
                    {
                        rootPopUp.SetActive(true);
                    }
                }

                if (receiveMessageData.eventName == "LeaveRoom")
                {
                    rootMessenger.SetActive(false);
                    rootCreateandJoin.SetActive(true);
                    rootJoinroom.SetActive(false);
                    rootCreateRoom.SetActive(false);
                }

                //MessageData receiveMessageData = JsonUtility.FromJson<MessageData>(tempMessageString);

                //if (receiveMessageData.username == inputUsername.text)
                //{
                //    sendText.text += "<color=yellow>" + receiveMessageData.username + "</color> : " + receiveMessageData.message + "\n";
                //    receiveText.text += "\n";
                //}
                //else
                //{
                //    sendText.text += "\n";
                //    receiveText.text += "<color=blue>" + receiveMessageData.username + "</color> : " + receiveMessageData.message + "\n";
                //}

                tempMessageString = "";
            }
        }

        public void ShowPanalCreate()
        {
            rootCreateandJoin.SetActive(false);
            rootCreateRoom.SetActive(true);
        }

        public void ShowPanalJoin()
        {
            rootCreateandJoin.SetActive(false);
            rootJoinroom.SetActive(true);
        }

        public void ClosePopUP()
        {
            rootPopUp.SetActive(false);            
        }        
    }
}


