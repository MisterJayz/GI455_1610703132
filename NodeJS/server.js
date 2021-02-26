var websocket = require('ws');
const sqlite = require('sqlite3').verbose();

var websocketServer = new websocket.Server({port:25500}, ()=>{
    console.log("MisterJayz server is running")
});

var database = new sqlite.Database('./database/chatDB.db', sqlite.OPEN_CREATE | sqlite.OPEN_READWRITE, (err)=>{

    if(err) throw err;

});

var wsList = [];
var roomList = [];
/*
{
    roomName: "xxxx", 
    wsList: []
}
*/

websocketServer.on("connection", (ws, rq)=>{

    {
        //LobbyZone
        ws.on("message", (data)=>{

            console.log(data);

            var toJson = JSON.parse(data); //แปลงเป็น Json

            //console.log(toJson["eventName"]); ใช้ได้เหมือนกัน 
            //console.log(toJson.eventName);          
            
            if(toJson.eventName == "CreateRoom")//CreateRoom
            {
                console.log("client request CreateRoom [" + toJson.data + "]")
                var isFoundRoom = false;
                for(var i = 0; i < roomList.length; i++)
                {
                    if(roomList[i].roomName == toJson.data)
                    {
                        isFoundRoom = true;
                        break;
                    }
                }

                if(isFoundRoom)
                {
                    //Callback to client : roomName is exist.
                    console.log("Create room : room is found");
                    
                    var resultData = {
                        eventName: toJson.eventName, 
                        data: "fail"
                    }

                    var toJsonStr = JSON.stringify(resultData) //แปลงกับไปเป็น Str

                    ws.send(toJsonStr); 
                }
                else
                {
                    //Create Room here.
                    console.log("Create room : room is not found");

                    var newRoom = {
                        roomName: toJson.data,
                        wsList: []
                    }

                    newRoom.wsList.push(ws);

                    roomList.push(newRoom);

                    var resultData = {
                        eventName: toJson.eventName, 
                        data: toJson.data
                    }

                    var toJsonStr = JSON.stringify(resultData) //แปลงกับไปเป็น Str

                    ws.send(toJsonStr);                   
                }                                
            }

            else if(toJson.eventName == "JoinRoom")//JoinRoom
            {
                console.log("client request JoinRoom [" + toJson.data + "]")
                var isHaveRoom = false;
                for(var i = 0; i < roomList.length; i++)
                {
                    if(roomList[i].roomName == toJson.data)
                    {                        
                        isHaveRoom = true;
                        roomList[i].wsList.push(ws);
                        break;
                    }
                } 
                if(isHaveRoom)
                {
                    //Join Room here.
                    console.log("Join room : Connect Room");

                    var resultData = {
                        eventName: toJson.eventName, 
                        data: "Connect",
                        showNameInChat: toJson.data
                    }

                    var toJsonStr = JSON.stringify(resultData) //แปลงกับไปเป็น Str

                    ws.send(toJsonStr); 
                }  
                else
                {
                    //Join Room Fail.
                    console.log("Join room : Can't Connect room");   

                    var resultData = {
                        eventName: toJson.eventName, 
                        data: "Fail"
                    }
                    
                    var toJsonStr = JSON.stringify(resultData) //แปลงกับไปเป็น Str

                    ws.send(toJsonStr);
                }                      
            }         

            else if(toJson.eventName == "LeaveRoom")//LeaveRoom
            {
                var isLeaveSuccess = false;//Set false to default.
                for(var i = 0; i < roomList.length; i++)
                {
                    for(var j = 0; j < roomList[i].wsList.length; j++)
                    {
                        if(roomList[i].wsList[j] == ws)
                        {
                            
                            roomList[i].wsList.splice(j, 1);

                            if(roomList[i].wsList.length <= 0)
                            {
                                roomList.splice(i ,1)
                            }
                            isLeaveSuccess = true;
                            break;
                        }
                    }                    
                }

                if(isLeaveSuccess)
                {
                    //========== Send callback message to Client ============

                    //ws.send("LeaveRoomSuccess");

                    //I will change to json string like a client side. Please see below
                    var callbackMsg = {
                        eventName:"LeaveRoom",
                        data:"success"
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                

                    console.log("leave room success");
                }

                else
                {
                    //========== Send callback message to Client ============

                    //ws.send("LeaveRoomFail");

                    //I will change to json string like a client side. Please see below
                    var callbackMsg = {
                        eventName:"LeaveRoom",
                        data:"fail"
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    //=======================================================

                    console.log("leave room fail");
                }

                var resultData = {
                    eventName: toJson.eventName, 
                    data: "Success"
                }
                
                var toJsonStr = JSON.stringify(resultData) //แปลงกับไปเป็น Str

                ws.send(toJsonStr);
            } 
            
            else if(toJson.eventName == "Register")//Register
            {                
                
                var toJson2 = JSON.parse(toJson.data);
                console.log("-------[0]-------");
                console.log(toJson2);
                console.log("-------[0]-------");

                //var sqlSelect = "SELECT * FROM UserDataServer WHERE UserID ='"+userID+"' AND Password ='"+password+"'"; //Login
                var sqlInsert = "INSERT INTO UserDataServer (UserID, UserName, Password, RePassword) VALUES ('"+toJson2.userID+"', '"+toJson2.userName+"', '"+toJson2.password+"', '"+toJson2.rePassword+"')"; //Register

                database.all(sqlInsert, (err, rows)=>{

                    if(err) //Register Error
                    {
                        var callbackMsg = {
                            eventName:"Register",
                            data:"Error"
                        }

                        var toJsonStr = JSON.stringify(callbackMsg);
                        console.log("-------[1]-------");
                        console.log(toJsonStr);
                        console.log("-------[1]-------");

                        ws.send(toJsonStr);
                    }

                    else //Register Success
                    {
                        var callbackMsg = {
                            eventName:"Register",
                            data:"Register Success"
                        }

                        var toJsonStr = JSON.stringify(callbackMsg);
                        console.log("-------[2]-------");
                        console.log(toJsonStr);
                        console.log("-------[2]-------");

                        ws.send(toJsonStr);
                    }

                });                
            }

            else if(toJson.eventName == "Login")//Login
            {
                var toJson2 = JSON.parse(toJson.data);

                console.log(toJson2);

                var sqlSelect = "SELECT * FROM UserDataServer WHERE UserID ='"+toJson2.userID+"' AND Password ='"+toJson2.password+"'"; //Login

                database.all(sqlSelect, (err, rows)=>{

                    //console.log(rows);

                    if(rows.length <= 0 ) //Login Error
                    {
                        var callbackMsg = {
                            eventName: "Login",
                            data: "Error"
                        }

                        var toJsonStr = JSON.stringify(callbackMsg);
                        console.log("-------[3]-------");
                        console.log(toJsonStr);
                        console.log("-------[3]-------");

                        ws.send(toJsonStr);
                    }

                    else //Login Success
                    {
                        var callbackMsg = {
                            eventName: "Login",
                            data: "Success",
                            showNameInChat: rows[0].UserName
                        }

                        var toJsonStr = JSON.stringify(callbackMsg);
                        console.log("-------[4]-------");
                        console.log(toJsonStr);
                        console.log("-------[4]-------");

                        ws.send(toJsonStr);
                    }

                });   
            }

            else if(toJson.eventName == "SendMessage")//SendMessage
            {
                var callbackMsg = {
                    eventName: "SendMessage",
                    data: toJson.data,
                    showNameInChat: toJson.showNameInChat                    
                }

                var toJsonStr = JSON.stringify(callbackMsg);                

                Boardcast(ws, data);
            }
        });        
    }
    
    console.log('client connected.');
    console.log('connect to database.');

    wsList.push(ws);    

    ws.on("close", ()=>{

        wsList = ArrayRemove(wsList, ws);
        console.log("client disconnected.");
        console.log("database disconnect.");        

        for(var i = 0; i < roomList.length; i++)
        {        
            for(var j = 0; j < roomList[i].wsList.length; j++)        
            {   
                if(roomList[i].wsList[j] == ws)
                {
                    roomList[i].wsList.splice(j, 1);
                    break;
                }
            }                        
        }        
    });
});

function ArrayRemove(arr, value)
{
    return arr.filter((element)=>{
        return element != value;
    })
}

function Boardcast(ws, data)
{
    var selectRoomIndex = -1;    

    for(var i = 0; i < roomList.length; i++)
    {
        for(var j = 0; j < roomList[i].wsList.length; j++)
        {
            if(ws == roomList[i].wsList[j])
            {
                selectRoomIndex = i;
                break;
            }
        }
    }

    for(var i = 0; i < roomList[selectRoomIndex].wsList.length; i++)
    {
        roomList[selectRoomIndex].wsList[i].send(data);
    }
}