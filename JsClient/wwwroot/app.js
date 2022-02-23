const url = "https://localhost:8334/messages"

let _connect, token;
let users = [];

document.getElementById("login").addEventListener("click", login);
document.getElementById("connect").addEventListener("click", connect);
document.getElementById("disconnect").addEventListener("click", disconnect);
document.getElementById("send").addEventListener("click", send);



async function start() {
    try {
        await _connect.start();
        console.log(`SignalR connected to ${url}`);
    }
    catch (err) {
        console.log(err);
        setTimeout(start, 3000);
    }
};


function buildConnect() {
    _connect = new signalR.HubConnectionBuilder()
        .withUrl(url, {
            accessTokenFactory: () => token
        })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

    _connect.on("UpdateUsersAsync", (users) => {
        users = users;
        const usersList = document.getElementById("usersList");
        while (usersList.firstChild) {
            userList.removeChild(userList.lastChild);
        }

        const ul = document.createElement("ul");
        for (let i = 0; i < users.Length; i++) {
            const li = document.createElement("li");
            li.textContent = users[i];
            ul.appendChild(li);
        }
        usersList.appendChild(ul);
    });

    _connect.on("SendMessageAsync", (user, message) => {
        const li = document.createElement("li");
        li.textContent.fontcolor = 'black';
        li.textContent = `${user}: ${message}`;
        document.getElementById("messagesList").appendChild(li);
    });

    _connect.onclose(async () => {
        document.getElementById("connect").removeAttribute("disabled");
        document.getElementById("disconnect").setAttribute("disabled", "");
        console.log(`SignalR now ${connect.state}`);
    });
}


function login() {
    const name = document.getElementById("name").value;
    const pass = document.getElementById("pass").value;

    if (name && pass) {
        
        let params = {
            "Login": name,
            "Password": pass
        };

        
        const xhr = new XMLHttpRequest();
        xhr.onerror = function () {
            document.getElementById("token").innerText = "Request failed";
        };
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4) {
                try {
                    token = JSON.parse(xhr.responseText).access_token;
                    document.getElementById("token").innerHTML = token;
                    document.getElementById("userToolbar").classList.remove("invisible");
                }
                catch (e) {
                    document.getElementById("token").innerHTML = "token error";
                }
            }
        };
        xhr.open("POST", "https://localhost:8334/api/auth/token", true);

        xhr.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        xhr.send(JSON.stringify(params));
    }
    return;
}


function send() {
    var message = document.getElementById("msgText").value;
    const name = document.getElementById("name").value;
    if (!message || !name) {
        alert("Message and Username is required");
        return;
    }

    _connect.invoke("SendMessageAsync", name, message);
}


function connect() {

    if (!token) {
        const li = document.createElement("li");
        li.textContent = "No token found";
        document.getElementById("messagesList").appendChild(li);
    }

    start();

    document.getElementById("disconnect").removeAttribute("disabled");
    document.getElementById("connect").setAttribute("disabled", "");
    document.getElementById("send-form").classList.remove("invisible");

    return;
}


function disconnect() {

    if (connect && connect.state === 'Connected') {
        console.log("Disconnecting...");
        _connect.stop();

        // Clear token
        document.getElementById("send-form").classList.add("invisible");
    }
    return;
}


buildConnect();