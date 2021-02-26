const sqlite = require('sqlite3').verbose();

var database = new sqlite.Database('./database/chatDB.db', sqlite.OPEN_CREATE | sqlite.OPEN_READWRITE, (err)=>{

    if(err) throw err;

    console.log("Connect to database");

    var dataFromClient = {
        eventName:"AddMoney",
        data:"08#100"
    }

    var splitStr = dataFromClient.data.split('#');
    var userID = splitStr[0];
    //var password = splitStr[1];
    //var name = splitStr[2];
    var money = parseInt(splitStr[1]);
    
    //var sqlSelect = "SELECT * FROM UserData WHERE UserID ='"+userID+"' AND Password ='"+password+"'"; //Login
    //var sqlInsert = "INSERT INTO UserData (UserID, Password, Name) VALUES ('"+userID+"', '"+password+"', '"+name+"')"; //Register
    //var sqlUpdate = "UPDATE UserData SET Money = '100' WHERE UserID = '"+userID+"'";//Update (Score, Money, บลาๆๆ)    

    /*
    database.all(sqlSelect, (err, rows)=>{

        if(err) 
        {
            console.log("[0]" + err);

            //console.log("Register Fail");
        }
        else
        {
            if(rows.length > 0) //Login Success
            {
                console.log("-------[1]-------");
                console.log(rows);
                console.log("-------[1]-------");
                var callbackMsg = {
                    eventName:"Login",
                    data:rows[0].Name
                }

                var toJsonStr = JSON.stringify(callbackMsg);
                console.log("-------[2]-------");
                console.log(toJsonStr);
                console.log("-------[2]-------");
            }
            else // Login Fail
            {
                var callbackMsg = {
                    eventName:"Login",
                    data:"Fail"
                }

                var toJsonStr = JSON.stringify(callbackMsg);
                console.log("-------[3]-------");
                console.log(toJsonStr);
                console.log("-------[3]-------");
            }           
        }
    });
    */

    /*
    database.all(sqlInsert, (err, rows)=>{

        if(err) //Register Error
        {
            var callbackMsg = {
                eventName:"Register",
                data:"Error"
            }

            var toJsonStr = JSON.stringify(callbackMsg);
            console.log("-------[0]-------");
            console.log(toJsonStr);
            console.log("-------[0]-------");
        }
        else //Register Success
        {
            var callbackMsg = {
                eventName:"Register",
                data:"Success"
            }

            var toJsonStr = JSON.stringify(callbackMsg);
            console.log("-------[1]-------");
            console.log(toJsonStr);
            console.log("-------[1]-------");
        }
    });
    */

    database.all("SELECT Money FROM UserData WHERE UserID = '"+userID+"'", (err, rows)=>{

        if(err) //Error
        {
            var callbackMsg = {
                eventName:"AddMoney",
                data:"Fail"
            }

            var toJsonStr = JSON.stringify(callbackMsg);
            console.log("-------[0]-------");
            console.log(toJsonStr);
            console.log("-------[0]-------");
        }
        else //Success
        { 
            console.log("-------[1]-------");       
            console.log(rows);       
            console.log("-------[1]-------");
            if(rows.length > 0)
            {
                var currentMoney = rows[0].Money;
                currentMoney += money;

                database.all("UPDATE UserData SET Money = '"+currentMoney+"' WHERE UserID = '"+userID+"'", (err, rows)=>{

                    if(err)
                    {
                        var callbackMsg = {
                            eventName:"AddMoney",
                            data:"Fail"
                        }
            
                        var toJsonStr = JSON.stringify(callbackMsg);
                        console.log("-------[3]-------");
                        console.log(toJsonStr);
                        console.log("-------[3]-------");
                    }
                    else
                    {
                        var callbackMsg = {
                            eventName:"AddMoney",
                            data:currentMoney.toString()
                        }
            
                        var toJsonStr = JSON.stringify(callbackMsg);
                        console.log("-------[4]-------");
                        console.log(toJsonStr);
                        console.log("-------[4]-------");
                    }
                });
            }
            else
            {
                var callbackMsg = {
                    eventName:"AddMoney",
                    data:"Fail"
                }
    
                var toJsonStr = JSON.stringify(callbackMsg);
                console.log("-------[2]-------");
                console.log(toJsonStr);
                console.log("-------[2]-------");
            }
        }
    });
});

