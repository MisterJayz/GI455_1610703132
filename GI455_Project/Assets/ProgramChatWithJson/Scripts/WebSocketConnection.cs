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
            public string showNameInChat;

            public SocketEvent(string eventName, string data, string showNameInChat)
            {
                this.eventName = eventName;
                this.data = data;
                this.showNameInChat = showNameInChat;
            }
        } 
        
        struct Register
        {            
            public string userID;            
            public string userName;
            public string password;
            public string rePassword;

            public Register(string userID, string username, string password, string rePassword)
            {               
                this.userID = userID;
                this.userName = username;
                this.password = password;                
                this.rePassword = rePassword;
            }
        }

        struct Login
        {
            public string userID;
            public string password;

            public Login(string userID, string password)
            {
                this.userID = userID;
                this.password = password;               
            }
        }

        //UI แสดงผล
        public GameObject rootConnection;
        public GameObject rootMessenger;
        public GameObject rootCreateAndJoin;
        public GameObject rootCreateRoom;
        public GameObject rootJoinRoom;
        public GameObject rootPopUp;
        public GameObject rootLoginAndRegister;
        public GameObject rootRegister;
        public GameObject rootInputIDAndPassword;
        public GameObject rootLoginFail;
        public GameObject rootPasswordNotMatch;
        public GameObject rootRegisterFail;
        public GameObject rootRegisterSuccess;

        //public InputField inputUsername;
        public InputField inputText;
        public InputField inputCrateRoom;
        public InputField inputJoinRoom;
        public InputField inputLoginUserID;
        public InputField inputLoginPassword;
        public InputField inputRegisterUserID;
        public InputField inputRegisterName;
        public InputField inputRegisterPassword;
        public InputField inputRegisterRepassword;        
        public Text sendText;
        public Text receiveText;
        public Text showName;
        public Text showRoomName;        
        string _Text2;
        string _name;        

        private WebSocket ws;

        private string tempMessageString;

        public void Start()
        {
            //ใช้โชว์หน้าต่าง UI
            rootConnection.SetActive(true);
            rootMessenger.SetActive(false);
            rootCreateAndJoin.SetActive(false);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(false);
            rootPopUp.SetActive(false);
            rootLoginAndRegister.SetActive(false);
            rootRegister.SetActive(false);
            rootInputIDAndPassword.SetActive(false);
            rootLoginFail.SetActive(false);
            rootPasswordNotMatch.SetActive(false);
            rootRegisterFail.SetActive(false);
            rootRegisterSuccess.SetActive(false);
        }
    

        public void Connect()
        {
            string url = $"ws://127.0.0.1:25500/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();

            //ใช้โชว์หน้าต่าง UI
            ShowLoginAndRegister();
        }

        public void RegisterChat()
        {
            if (ws.ReadyState == WebSocketState.Open && string.IsNullOrEmpty(inputRegisterUserID.text) == false && string.IsNullOrEmpty(inputRegisterPassword.text) == false && string.IsNullOrEmpty(inputRegisterName.text) == false && string.IsNullOrEmpty(inputRegisterRepassword.text) == false)
            {
                if (inputRegisterPassword.text != inputRegisterRepassword.text)
                {
                    rootConnection.SetActive(false);
                    rootMessenger.SetActive(false);
                    rootCreateAndJoin.SetActive(false);
                    rootCreateRoom.SetActive(false);
                    rootJoinRoom.SetActive(false);
                    rootPopUp.SetActive(false);
                    rootLoginAndRegister.SetActive(false);
                    rootRegister.SetActive(false);
                    rootInputIDAndPassword.SetActive(false);
                    rootLoginFail.SetActive(false);
                    rootPasswordNotMatch.SetActive(true);
                    rootRegisterFail.SetActive(false);
                    rootRegisterSuccess.SetActive(false);
                }

                else
                {
                    Register register = new Register(inputRegisterUserID.text, inputRegisterName.text, inputRegisterPassword.text, inputRegisterRepassword.text);

                    string jsonStr = JsonUtility.ToJson(register);

                    SocketEvent socketEvent = new SocketEvent("Register", jsonStr, "");

                    string jsonStr2 = JsonUtility.ToJson(socketEvent);

                    ws.Send(jsonStr2);
                }                
            }            

            else
            {
                rootConnection.SetActive(false);
                rootMessenger.SetActive(false);
                rootCreateAndJoin.SetActive(false);
                rootCreateRoom.SetActive(false);
                rootJoinRoom.SetActive(false);
                rootPopUp.SetActive(false);
                rootLoginAndRegister.SetActive(false);
                rootRegister.SetActive(false);
                rootInputIDAndPassword.SetActive(true);
                rootLoginFail.SetActive(false);
                rootPasswordNotMatch.SetActive(false);
                rootRegisterFail.SetActive(false);
                rootRegisterSuccess.SetActive(false);
            }
        }

        public void LoginChat()
        {            
            if (ws.ReadyState == WebSocketState.Open && string.IsNullOrEmpty(inputLoginUserID.text) == false && string.IsNullOrEmpty(inputLoginPassword.text) == false)
            {
                Login login = new Login(inputLoginUserID.text, inputLoginPassword.text);

                string jsonStr = JsonUtility.ToJson(login);

                SocketEvent socketEvent = new SocketEvent("Login", jsonStr, "");

                string jsonStr2 = JsonUtility.ToJson(socketEvent);

                ws.Send(jsonStr2);                              
            }            

            else
            {
                rootConnection.SetActive(false);
                rootMessenger.SetActive(false);
                rootCreateAndJoin.SetActive(false);
                rootCreateRoom.SetActive(false);
                rootJoinRoom.SetActive(false);
                rootPopUp.SetActive(false);
                rootLoginAndRegister.SetActive(false);
                rootRegister.SetActive(false);
                rootInputIDAndPassword.SetActive(false);
                rootLoginFail.SetActive(true);
                rootPasswordNotMatch.SetActive(false);
                rootRegisterFail.SetActive(false);
                rootRegisterSuccess.SetActive(false);
            }
        }

        public void CreateRoom()
        {
            if (ws.ReadyState == WebSocketState.Open && string.IsNullOrEmpty(inputCrateRoom.text) == false)
            {
                SocketEvent socketEvent = new SocketEvent("CreateRoom", inputCrateRoom.text, "");

                string jsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(jsonStr);
            }
        }        

        public void JoinRoom()
        {
            if(ws.ReadyState == WebSocketState.Open && string.IsNullOrEmpty(inputJoinRoom.text) == false)
            {
                SocketEvent socketEvent = new SocketEvent("JoinRoom", inputJoinRoom.text, "");

                string jsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(jsonStr);
            }
        }

        public void LeaveRoom()
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("LeaveRoom", "", "");

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

            SocketEvent socketEvent = new SocketEvent("SendMessage", inputText.text, _name);            

            string toJsonStr = JsonUtility.ToJson(socketEvent);            

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
                Debug.Log(tempMessageString);
                SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(tempMessageString);                
                if (receiveMessageData.eventName == "CreateRoom") 
                {                    
                    if (receiveMessageData.data == "fail")
                    {
                        //ใช้โชว์หน้าต่าง UI
                        rootConnection.SetActive(false);
                        rootMessenger.SetActive(false);
                        rootCreateAndJoin.SetActive(false);
                        rootCreateRoom.SetActive(false);
                        rootJoinRoom.SetActive(false);
                        rootPopUp.SetActive(true);
                    }
                    else
                    {
                        //ใช้โชว์หน้าต่าง UI
                        rootConnection.SetActive(false);
                        rootMessenger.SetActive(true);
                        rootCreateAndJoin.SetActive(false);
                        rootCreateRoom.SetActive(false);
                        rootJoinRoom.SetActive(false);
                        rootPopUp.SetActive(false);

                        _Text2 = receiveMessageData.data;
                        showRoomName.text = _Text2;
                    }
                }

                else if (receiveMessageData.eventName == "JoinRoom")
                {
                    if (receiveMessageData.data == "Connect")
                    {
                        //ใช้โชว์หน้าต่าง UI
                        rootConnection.SetActive(false);
                        rootMessenger.SetActive(true);
                        rootCreateAndJoin.SetActive(false);
                        rootCreateRoom.SetActive(false);
                        rootJoinRoom.SetActive(false);
                        rootPopUp.SetActive(false);

                        _Text2 = receiveMessageData.showNameInChat;
                        showRoomName.text = _Text2;
                    }
                    else
                    {
                        //ใช้โชว์หน้าต่าง UI
                        rootConnection.SetActive(false);
                        rootMessenger.SetActive(false);
                        rootCreateAndJoin.SetActive(false);
                        rootCreateRoom.SetActive(false);
                        rootJoinRoom.SetActive(false);
                        rootPopUp.SetActive(true);                        
                    }
                }

                else if (receiveMessageData.eventName == "LeaveRoom")
                {
                    //ใช้โชว์หน้าต่าง UI
                    rootConnection.SetActive(false);
                    rootMessenger.SetActive(false);
                    rootCreateAndJoin.SetActive(true);
                    rootCreateRoom.SetActive(false);
                    rootJoinRoom.SetActive(false);
                    rootPopUp.SetActive(false);

                    sendText.text = "";
                    receiveText.text = "";
                }

                else if (receiveMessageData.eventName == "Login")
                {
                    if(receiveMessageData.data == "Error")
                    {
                        rootConnection.SetActive(false);
                        rootMessenger.SetActive(false);
                        rootCreateAndJoin.SetActive(false);
                        rootCreateRoom.SetActive(false);
                        rootJoinRoom.SetActive(false);
                        rootPopUp.SetActive(false);
                        rootLoginAndRegister.SetActive(false);
                        rootRegister.SetActive(false);
                        rootInputIDAndPassword.SetActive(false);
                        rootLoginFail.SetActive(true);
                        rootPasswordNotMatch.SetActive(false);
                        rootRegisterFail.SetActive(false);
                        rootRegisterSuccess.SetActive(false);
                    }
                    else
                    {
                        rootConnection.SetActive(false);
                        rootMessenger.SetActive(false);
                        rootCreateAndJoin.SetActive(true);
                        rootCreateRoom.SetActive(false);
                        rootJoinRoom.SetActive(false);
                        rootPopUp.SetActive(false);
                        rootLoginAndRegister.SetActive(false);
                        rootRegister.SetActive(false);
                        rootInputIDAndPassword.SetActive(false);
                        rootLoginFail.SetActive(false);
                        rootPasswordNotMatch.SetActive(false);
                        rootRegisterFail.SetActive(false);
                        rootRegisterSuccess.SetActive(false);                       

                        _name = receiveMessageData.showNameInChat;
                        showName.text = _name;
                    }
                }

                else if (receiveMessageData.eventName == "Register")
                {
                    if (receiveMessageData.data == "Error")
                    {
                        rootConnection.SetActive(false);
                        rootMessenger.SetActive(false);
                        rootCreateAndJoin.SetActive(false);
                        rootCreateRoom.SetActive(false);
                        rootJoinRoom.SetActive(false);
                        rootPopUp.SetActive(false);
                        rootLoginAndRegister.SetActive(false);
                        rootRegister.SetActive(false);
                        rootInputIDAndPassword.SetActive(false);
                        rootLoginFail.SetActive(false);
                        rootPasswordNotMatch.SetActive(false);
                        rootRegisterFail.SetActive(true);
                        rootRegisterSuccess.SetActive(false);
                    }
                    else
                    {
                        rootConnection.SetActive(false);
                        rootMessenger.SetActive(false);
                        rootCreateAndJoin.SetActive(false);
                        rootCreateRoom.SetActive(false);
                        rootJoinRoom.SetActive(false);
                        rootPopUp.SetActive(false);
                        rootLoginAndRegister.SetActive(false);
                        rootRegister.SetActive(false);
                        rootInputIDAndPassword.SetActive(false);
                        rootLoginFail.SetActive(false);
                        rootPasswordNotMatch.SetActive(false);
                        rootRegisterFail.SetActive(false);
                        rootRegisterSuccess.SetActive(true);
                    }                    
                }

                else if (receiveMessageData.eventName == "SendMessage")
                {
                    if (receiveMessageData.showNameInChat == _name)
                    {
                        sendText.text += "<color=yellow>" + receiveMessageData.showNameInChat + "</color> : " + receiveMessageData.data + "\n";
                        receiveText.text += "\n";
                        print(_name);
                    }

                    else
                    {                        
                        receiveText.text += "<color=blue>" + receiveMessageData.showNameInChat + "</color> : " + receiveMessageData.data + "\n";
                        sendText.text += "\n";
                        print(_name);
                    }
                }
                

                tempMessageString = "";
            }
        }

        public void ShowPanalCreate()
        {
            //ใช้โชว์หน้าต่าง UI
            rootConnection.SetActive(false);
            rootMessenger.SetActive(false);
            rootCreateAndJoin.SetActive(false);
            rootCreateRoom.SetActive(true);
            rootJoinRoom.SetActive(false);
            rootPopUp.SetActive(false);
            rootLoginAndRegister.SetActive(false);
            rootRegister.SetActive(false);
            rootInputIDAndPassword.SetActive(false);
            rootLoginFail.SetActive(false);
            rootPasswordNotMatch.SetActive(false);
            rootRegisterFail.SetActive(false);
            rootRegisterSuccess.SetActive(false);
        }

        public void ShowPanalJoin()
        {
            //ใช้โชว์หน้าต่าง UI
            rootConnection.SetActive(false);
            rootMessenger.SetActive(false);
            rootCreateAndJoin.SetActive(false);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(true);
            rootPopUp.SetActive(false);
            rootLoginAndRegister.SetActive(false);
            rootRegister.SetActive(false);
            rootInputIDAndPassword.SetActive(false);
            rootLoginFail.SetActive(false);
            rootPasswordNotMatch.SetActive(false);
            rootRegisterFail.SetActive(false);
            rootRegisterSuccess.SetActive(false);
        }

        public void ClosePopUp()
        {
            //ใช้โชว์หน้าต่าง UI
            rootPopUp.SetActive(false);
            ShowPanalCreateAndJoin();
        }
        
        public void ShowPanalCreateAndJoin()
        {
            //ใช้โชว์หน้าต่าง UI
            rootConnection.SetActive(false);
            rootMessenger.SetActive(false);
            rootCreateAndJoin.SetActive(true);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(false);
            rootPopUp.SetActive(false);
            rootLoginAndRegister.SetActive(false);
            rootRegister.SetActive(false);
            rootInputIDAndPassword.SetActive(false);
            rootLoginFail.SetActive(false);
            rootPasswordNotMatch.SetActive(false);
            rootRegisterFail.SetActive(false);
            rootRegisterSuccess.SetActive(false);
        }

        public void OnRegister()
        {
            //ใช้โชว์หน้าต่าง UI
            rootConnection.SetActive(false);
            rootMessenger.SetActive(false);
            rootCreateAndJoin.SetActive(false);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(false);
            rootPopUp.SetActive(false);
            rootLoginAndRegister.SetActive(false);
            rootRegister.SetActive(true);
            rootInputIDAndPassword.SetActive(false);
            rootLoginFail.SetActive(false);
            rootPasswordNotMatch.SetActive(false);
            rootRegisterFail.SetActive(false);
            rootRegisterSuccess.SetActive(false);
        }

        public void CloseRegisterSuccess()
        {
            rootRegisterSuccess.SetActive(false);
            ShowLoginAndRegister();
        }

        public void ShowLoginAndRegister()
        {
            rootConnection.SetActive(false);
            rootMessenger.SetActive(false);
            rootCreateAndJoin.SetActive(false);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(false);
            rootPopUp.SetActive(false);
            rootLoginAndRegister.SetActive(true);
            rootRegister.SetActive(false);
            rootInputIDAndPassword.SetActive(false);
            rootLoginFail.SetActive(false);
            rootPasswordNotMatch.SetActive(false);
            rootRegisterFail.SetActive(false);
            rootRegisterSuccess.SetActive(false);
        }

        public void CloseRegisterFail()
        {
            rootRegisterFail.SetActive(false);
            ShowLoginAndRegister();
        }

        public void CloseLoginFail()
        {
            rootConnection.SetActive(false);
            rootMessenger.SetActive(false);
            rootCreateAndJoin.SetActive(false);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(false);
            rootPopUp.SetActive(false);
            rootLoginAndRegister.SetActive(true);
            rootRegister.SetActive(false);
            rootInputIDAndPassword.SetActive(false);
            rootLoginFail.SetActive(false);
            rootPasswordNotMatch.SetActive(false);
            rootRegisterFail.SetActive(false);
            rootRegisterSuccess.SetActive(false);
        }

        public void CloseInputAllFieldAndClosePasswordNotMatch()
        {
            rootConnection.SetActive(false);
            rootMessenger.SetActive(false);
            rootCreateAndJoin.SetActive(false);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(false);
            rootPopUp.SetActive(false);
            rootLoginAndRegister.SetActive(false);
            rootRegister.SetActive(true);
            rootInputIDAndPassword.SetActive(false);
            rootLoginFail.SetActive(false);
            rootPasswordNotMatch.SetActive(false);
            rootRegisterFail.SetActive(false);
            rootRegisterSuccess.SetActive(false);
        }        
    }
}