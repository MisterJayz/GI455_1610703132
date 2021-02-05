﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;

namespace ProgramChat
{
    public class WebsocketConnection : MonoBehaviour
    {
        //เมนูก่อนเข้ามา
        string playerName;
        public GameObject login;
        public GameObject inputFieldName;
        public Text name;
        public Text ip;
        public Text port;
        
        //หน้าแชท
        public GameObject chat;
        public GameObject chatPanal;
        public GameObject textObject;
        public GameObject inputFieldMessage;
        public Button clickButton;
        public GameObject displayText;        

        string chatText;
        //public Text showText;               

        public int maxMessage = 20;
        [SerializeField]
        List<Message> messageList = new List<Message>();

        private WebSocket websocket;

        // Start is called before the first frame update
        void Start()
        {
            clickButton.onClick.AddListener(ClickButtonToSend);

            websocket = new WebSocket("ws://127.0.0.1:25500/");

            websocket.OnMessage += OnMessage;            

            //websocket.Connect();            
        }

        // Update is called once per frame
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //{                
            //    websocket.Send("Number : " + Random.Range(0, 99999));
            //}
        }

        public void OnDestroy()
        {
            if (websocket != null)
            {
                websocket.Close();
            }
        }

        public void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {            
            Debug.Log("Message from server : " + messageEventArgs.Data);            
        }

        public void GetInputOnClickButton()
        {
            playerName = inputFieldName.GetComponent<Text>().text;
            Debug.Log("Wellcome " + playerName + " to chat");
            displayText.GetComponent<Text>().text = "Wellcome " + playerName + " to chat";            
            websocket.Connect();
            if (name.text == playerName && ip.text == "127.0.0.1" && port.text == "25500")
            {
                login.SetActive(false);
                chat.SetActive(true);
            }
        }

        public void ClickButtonToSend()
        {           
            chatText = inputFieldMessage.GetComponent<Text>().text;
            //showText.GetComponent<Text>().text = chatText;
            SendMessageToChat(chatText);
            websocket.Send(chatText);            
        }

        public void SendMessageToChat(string text)
        {
            if(messageList.Count >= maxMessage)
            {
                Destroy(messageList[0].textObject.gameObject);
                messageList.Remove(messageList[0]);
            }
            Message newMessage = new Message();
            newMessage.chatText = text;
            GameObject newText = Instantiate(textObject, chatPanal.transform);
            newMessage.textObject = newText.GetComponent<Text>();
            newMessage.textObject.text = newMessage.chatText;            
            messageList.Add(newMessage);
        }

        public void ServerSendBack()
        {

        }
    }    
}

[System.Serializable]
public class Message
{
    public string chatText;
    public Text textObject;
}