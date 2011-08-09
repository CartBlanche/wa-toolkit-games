function NullClient()
{
}

NullClient.prototype.restartPollingTimeout = function()
{
};

function SetBlobEndpoint(blobEndpoint)
{
    window.GetBlobEndpoint = function()
    {
        return blobEndpoint;
    };
}

// Entities - BEGIN

function UserProfileProvider()
{
    this.serverInterface = new ServerInterface(new NullClient());
}

UserProfileProvider.prototype.get = function (callback) {
    var reallyThis = this;
    window.usersCallback = function (userProfile) {
        return callback(userProfile);
    };

    window.getUserProfile = function (userId) {
        reallyThis.serverInterface.sendAjaxJsonpGet("{0}users/{1}?callback=?".format(GetBlobEndpoint(), userId), "usersCallback");
    }; // Get user id
    $.ajax({
        type: "GET",
        url: "/User/Verify",
        success: function (userId) {
            setTimeout(function () {
                getUserProfile(userId);
            }, 1000);
        },
        error: function (req, status, error) {
            alert("Verify User error:" + error);
        }
    });

};
// Entities - END

// Server Interface And Constants - BEGIN

function ServerInterface(parentClient)
{
    this.parentClient = parentClient;
}

ServerInterface.prototype.onAjaxGetError = function(req, status, error)
{
    var errorMessage;
    if (req.responseText == undefined)
    {
        errorMessage = "GET Error:\nreq:" + req + "\nstatus:" + status + "\nerror:" + error;
    }
    else
    {
        errorMessage = "GET Error:\nreq:" + req.responseText + "\nstatus:" + status + "\nerror:" + error;
    }
    alert(errorMessage);
    this.parentClient.restartPollingTimeout();
};
ServerInterface.prototype.onAjaxPostError = function(req, status, error)
{
    var errorMessage;
    if (req.responseText == undefined)
    {
        errorMessage = "POST Error:\nreq:" + req + "\nstatus:" + status + "\nerror:" + error;
    }
    else
    {
        errorMessage = "POST Error:\nreq:" + req.responseText + "\nstatus:" + status + "\nerror:" + error;
    }
    alert(errorMessage);
};
ServerInterface.prototype.sendAjaxJsonCommand = function(type, url, data, onSuccessCallback, onErrorCallback)
{
    var errorCallback = onErrorCallback != null ? onErrorCallback : this.onAjaxPostError;
    $.ajax({
               type: type,
               url: url,
               dataType: "json",
               data: data,
               success: onSuccessCallback,
               error: errorCallback
           });
};
ServerInterface.prototype.sendAjaxCommand = function(type, url, data, onSuccessCallback, onErrorCallback)
{
    var errorCallback = onErrorCallback != null ? onErrorCallback : this.onAjaxPostError;
    $.ajax({
               type: type,
               url: url,
               data: data,
               success: onSuccessCallback,
               error: errorCallback
           });
};
ServerInterface.prototype.sendAjaxJsonPost = function(url, data, onSuccessCallback, onErrorCallback)
{
    this.sendAjaxJsonCommand("POST", url, data, onSuccessCallback, onErrorCallback);
};
ServerInterface.prototype.sendAjaxPost = function(url, data, onSuccessCallback, onErrorCallback)
{
    this.sendAjaxCommand("POST", url, data, onSuccessCallback, onErrorCallback);
};
ServerInterface.prototype.sendAjaxJsonGet = function(url, data, onSuccessCallback, onErrorCallback)
{
    this.sendAjaxJsonCommand("GET", url, data, onSuccessCallback, onErrorCallback);
};
ServerInterface.prototype.sendAjaxJsonpGet = function(url, jsonpCallback)
{
    $.ajax({
               type: "GET",
               url: url,
               dataType: "jsonp",
               jsonpCallback: jsonpCallback,
               error: this.onAjaxGetError
           });
};
ServerInterface.prototype.sendAjaxJsonpGetWithError = function (url, jsonpCallback, error) {
           $.ajax({
               type: "GET",
               url: url,
               dataType: "jsonp",
               jsonpCallback: jsonpCallback,
               error: error
           });
       };
       ServerInterface.prototype.sendAjaxJsonpGetFailingSilently = function (url, jsonpCallback)
{
    $.ajax({
               type: "GET",
               url: url,
               dataType: "jsonp",
               jsonpCallback: jsonpCallback
           });
};
// Server Interface And Constants - END

ServerInterface.prototype.sendAjaxJsonCommand = function(type, url, data, onSuccessCallback, onErrorCallback)
{
    var errorCallback = onErrorCallback != null ? onErrorCallback : this.onAjaxPostError;
    $.ajax({
               type: type,
               url: url,
               dataType: "json",
               data: data,
               success: onSuccessCallback,
               error: errorCallback
           });
};

ServerInterface.prototype.sendAjaxCommand = function(type, url, data, onSuccessCallback, onErrorCallback)
{
    var errorCallback = onErrorCallback != null ? onErrorCallback : this.onAjaxPostError;
    $.ajax({
               type: type,
               url: url,
               data: data,
               success: onSuccessCallback,
               error: errorCallback
           });
};

ServerInterface.prototype.sendAjaxJsonPost = function(url, data, onSuccessCallback, onErrorCallback)
{
    this.sendAjaxJsonCommand("POST", url, data, onSuccessCallback, onErrorCallback);
};

ServerInterface.prototype.sendAjaxPost = function(url, data, onSuccessCallback, onErrorCallback)
{
    this.sendAjaxCommand("POST", url, data, onSuccessCallback, onErrorCallback);
};

ServerInterface.prototype.sendAjaxGet = function (url, onSuccessCallback, onErrorCallback) {
    this.sendAjaxCommand("GET", url, null, onSuccessCallback, onErrorCallback);
};

ServerInterface.prototype.sendAjaxJsonGet = function(url, data, onSuccessCallback, onErrorCallback)
{
    this.sendAjaxJsonCommand("GET", url, data, onSuccessCallback, onErrorCallback);
};

ServerInterface.prototype.sendAjaxJsonpGet = function(url, jsonpCallback)
{
    $.ajax({
               type: "GET",
               url: url,
               dataType: "jsonp",
               jsonpCallback: jsonpCallback,
               error: this.onAjaxGetError
           });
};

ServerInterface.prototype.sendAjaxJsonpGetFailingSilently = function(url, jsonpCallback)
{
    $.ajax({
               type: "GET",
               url: url,
               dataType: "jsonp",
               jsonpCallback: jsonpCallback
           });
};
// Server Interface And Constants - END

String.prototype.format = function()
{
    var s = this,
        i = arguments.length;

    while (i--)
    {
        s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
    }

    return s;
};

function selectObjectIds(collection)
{
    return jQuery.map(collection, function(element)
        {
        return element.Id;
        });
}

jQuery.grepFirst = function(elems, callback)
{
    return jQuery.grep(elems, callback)[0];
};