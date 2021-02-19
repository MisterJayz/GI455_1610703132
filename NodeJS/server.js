var websocket = require('ws');

var websocketServer = new websocket.Server({port:25500}, ()=>{
    console.log("MisterJayz server is running")
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
                        data: "success"
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
                        data: "Connect"
                    }

                    var toJsonStr = JSON.stringify(resultData) //แปลงกับไปเป็น Str

                    ws.send(toJsonStr); 
                }  
                else
                {
                    //Create Room here.
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

                var resultData = {
                    eventName: toJson.eventName, 
                    data: "Success"
                }
                
                var toJsonStr = JSON.stringify(resultData) //แปลงกับไปเป็น Str

                ws.send(toJsonStr);
            }                      
        });        
    }
    
    console.log('client connected.');

    wsList.push(ws);    

    ws.on("close", ()=>{

        wsList = ArrayRemove(wsList, ws);
        console.log("client disconnected.");


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

function Boardcast(data)
{
    for(var i = 0; i < wsList.length; i++)
    {
        wsList[i].send(data);
    }
}